using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public class FlowEngine : IMessageGateway, IDisposable
    {
        private readonly ILogger<FlowEngine> _logger;
        private readonly IFlowDataSource _flowDataSource;
        private readonly IEnumerable<IPreAction> _preActions;
        private readonly IDictionary<FlowNodeType, IMessageProcessor> _messageProcessors;
        private readonly IMessagingChannelProvider _messagingChannelProvider;
        private readonly IDictionary<ChannelId, FlowNode> _flowNodes = new Dictionary<ChannelId, FlowNode>();
        private readonly ICollection<object> _subscriptions = new List<object>();

        public FlowEngine(
            ILogger<FlowEngine> logger,
            IFlowDataSource flowDataSource,
            IEnumerable<IMessageProcessor> messageProcessors,
            IEnumerable<IPreAction> preActions,
            IMessagingChannelProvider messagingChannelProvider)
        {
            _logger = logger;
            _flowDataSource = flowDataSource;
            _preActions = preActions;
            _messagingChannelProvider = messagingChannelProvider;

            _messageProcessors = messageProcessors.ToDictionary(m => m.HandledType, m => m);
            SubscribeAllIncomingChannel();
        }

        private void SubscribeAllIncomingChannel()
        {
            foreach (var flowNode in _flowDataSource.GetAllFlowNodes())
            {
                Type? type = FlowEngineResolver.ExpectedPayloadType(flowNode);
                _logger.LogInformation("Add channel subscription to `{ChannelName}` (type:{NodeType}, payload:{PayloadType})", flowNode.InChannelId, flowNode.NodeType, type);
                _flowNodes.Add(flowNode.InChannelId, flowNode);

                GetType().GetMethod(nameof(SubscribeChannel), BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(type ?? typeof(VoidParameter))
                    .Invoke(this, new object?[] { flowNode });
            }
        }

        [Endpoint(inChannelId: EngineChannels.DefaultErrorChannel)]
        public void ErrorChannel(IMessage<MessageFailure> exceptionWithMessage)
        {
            _logger.LogError(exceptionWithMessage.Payload.Exception, "Error occured with message:{Message} and payload:{Payload}.", exceptionWithMessage, exceptionWithMessage.Payload.OriginalMessage);
        }

        [Router(inChannelId: EngineChannels.RouteByHeaderChannel)]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration requires the method to be not static")]
        public string RouteByHeader(IMessageHeaders headers)
        {
            string? routeToChannel = headers.RouteToChannel;
            if (routeToChannel == null)
            {
                throw new EnterpriseIntegrationException("Route-By-Header-Channel used, without defining RouteByHeader MessageHeader.");
            }

            return routeToChannel;
        }

        private void SubscribeChannel<T>(FlowNode flowNode)
        {
            IMessagingChannel channel = _messagingChannelProvider.GetMessagingChannel(flowNode.InChannelId);
            _subscriptions.Add(new ChannelSubscription<T>(this, flowNode, channel));
        }

        public Task SendMessage(ChannelId channelId, IMessage message)
        {
            _logger.LogDebug("SendMessage({Channel}, {Message})", channelId, message);
            return _messagingChannelProvider.GetMessagingChannel(channelId).Send(message);
        }

        public Task Submit<T>(string channel, T messageOrPayload)
        {
            if (messageOrPayload == null)
            {
                throw new ArgumentNullException(nameof(messageOrPayload));
            }

            return messageOrPayload.GetType().IsMessage() 
                ? SendMessage(channel, (IMessage)messageOrPayload)
                : Submit(channel, new MessageHeaders(), messageOrPayload);
        }

        public async Task Submit<T>(string channel, IMessageHeaders headers, T payload)
        {
            await SendMessage(channel, new GenericMessage<T>(headers, payload));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _subscriptions.Clear();
            }
        }

        private async Task ExecutePreActions<T>(FlowNode flowNode, IMessage<T> message)
        {
            await Task.WhenAll(_preActions.Select(a => a.PreProcess(flowNode, message)));
        }

        public Task Send<T>(ChannelId channelId, T payload)
        {
            return Send(channelId, new MessageHeaders(), payload);
        }

        public async Task Send<T>(ChannelId channelId, IMessageHeaders headers, T payload)
        {
            await SendMessage(channelId, new GenericMessage<T>(headers, payload));
        }

        private async Task HandleException(Exception ex, IMessage message, FlowNode flowNode)
        {
            Exception reasonOfError = ex;
            if (ex is TargetInvocationException && ex.InnerException != null)
            {
                reasonOfError = ex.InnerException;
            }

            if (reasonOfError is PayloadTransformationException payloadTransformationException)
            {
                reasonOfError = new PayloadTransformationException(payloadTransformationException.ReceivedType, payloadTransformationException.MethodParameterType, (ChannelId)flowNode.OutChannelId!);
            }

            await SendMessage(message.MessageHeaders.ErrorChannel ?? EngineChannels.DefaultErrorChannel, GenericMessage<MessageFailure>.From(message, new MessageFailure(message, reasonOfError)));
        }

        /// <summary>
        ///     Creates a subscription to a <see cref="IMessagingChannel"/> directing the received messages to 
        ///     the correct processor.
        /// </summary>
        private class ChannelSubscription<T>
        {
            private readonly FlowEngine _engine;
            private readonly FlowNode _flowNode;
            private readonly Lazy<Func<IMessage<T>, Task>> _receiver;

            public ChannelSubscription(FlowEngine engine, FlowNode flowNode, IMessagingChannel messagingChannel)
            {
                _engine = engine;
                _flowNode = flowNode;
                _receiver = new Lazy<Func<IMessage<T>, Task>>(() => Receiver());
                messagingChannel.Subscribe(_receiver.Value);
            }

            private Func<IMessage<T>, Task> Receiver()
            {
                if (!_engine._messageProcessors.ContainsKey(_flowNode.NodeType))
                {
                    throw new EnterpriseIntegrationException($"No MessageProcessor registered for NodeType:{_flowNode.NodeType}.");
                }

                Func<IMessage<T>, FlowNode, SendMessageAsync, Task<IEnumerable<IMessage>>> processor = _engine._messageProcessors[_flowNode.NodeType].Process;
                return async msg =>
                {
                    try
                    {
                        await _engine.ExecutePreActions(_flowNode, msg);
                        await processor(msg, _flowNode, _engine.SendMessage);
                    } 
                    catch (Exception ex)
                    {
                        await _engine.HandleException(ex, msg, _flowNode);
                    }
                };
            }

        }
    }
}

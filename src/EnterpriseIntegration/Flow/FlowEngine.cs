﻿using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public class FlowEngine : IDisposable
    {
        private readonly ILogger<FlowEngine> _logger;
        private readonly IFlowDataSource _flowDataSource;
        private readonly IEnumerable<IPreAction> _preActions;
        private readonly IDictionary<FlowNodeType, IMessageProcessor> _messageProcessors;
        private readonly IMessagingChannelProvider _messagingChannelProvider;
        private readonly IDictionary<string, FlowNode> _flowNodes = new Dictionary<string, FlowNode>();
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
                _logger.LogInformation("Add channel subscription to `{ChannelName}` (type:{NodeType}, payload:{PayloadType})", flowNode.InChannelName, flowNode.NodeType, type);
                _flowNodes.Add(flowNode.InChannelName, flowNode);

                GetType().GetMethod(nameof(SubscribeChannel), BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(type != null ? type : typeof(VoidParameter))
                    .Invoke(this, new object?[] { flowNode });
            }
        }

        [Endpoint(inChannelName: EngineChannels.DefaultErrorChannel)]
        public void ErrorChannel(IMessage<MessageFailure> exceptionWithMessage)
        {
            _logger.LogError(exceptionWithMessage.Payload.exception, "Error occured with message:{Message} and payload:{Payload}.", exceptionWithMessage, exceptionWithMessage.Payload.originalPayload);
        }

        [Router(inChannelName: EngineChannels.RouteByHeaderChannel)]
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
            IMessagingChannel channel = _messagingChannelProvider.GetMessagingChannel(flowNode.InChannelName);
            _subscriptions.Add(new ChannelSubscription<T>(this, flowNode, channel));
        }

        private Task SendMessage(string channel, IMessage message)
        {
            _logger.LogDebug("SendMessage({Channel}, {Message})", channel, message);
            return _messagingChannelProvider.GetMessagingChannel(channel).Send(message);
        }

        public Task Submit<T>(string channel, T payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return payload.GetType().IsMessage() 
                ? SendMessage(channel, (IMessage)payload)
                : Submit(channel, new MessageHeaders(), payload);
        }

        public async Task Submit<T>(string channel, IMessageHeaders headers, T payload)
        {
            await SendMessage(channel, new GenericMessage<T>(headers, payload));
        }

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        public async Task ExecutePreActions<T>(FlowNode flowNode, IMessage<T> message)
        {
            await Task.WhenAll(_preActions.Select(a => a.PreProcess(flowNode, message)));
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
                    await _engine.ExecutePreActions(_flowNode, msg);
                    await processor(msg, _flowNode, _engine.SendMessage);
                };
            }
        }
    }
}

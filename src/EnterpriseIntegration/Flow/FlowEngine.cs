using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public class FlowEngine
    {
        private readonly ILogger _logger;
        private readonly IFlowDataSource _flowDataSource;
        private readonly IMessagingChannelProvider _messagingChannelProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, FlowNode> _flowNodes = new Dictionary<string, FlowNode>();

        public FlowEngine(
            ILogger logger,
            IFlowDataSource flowDataSource, 
            IMessagingChannelProvider messagingChannelProvider,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _flowDataSource = flowDataSource;
            _messagingChannelProvider = messagingChannelProvider;
            _serviceProvider = serviceProvider;

            SubscribeAllIncomingChannel();
        }

        private void SubscribeAllIncomingChannel()
        {
            foreach (var flowNode in _flowDataSource.GetAllFlowNodes())
            {
                Type? type = FlowEngineResolver.ExpectedPayloadType(flowNode);
                _logger.LogInformation("Add channel subscription to `{ChannelName}` (type:{NodeType}, payload:{PayloadType})", flowNode.InChannelName, flowNode.NodeType, type);
                _flowNodes.Add(flowNode.InChannelName, flowNode);
                GetType().GetMethod(nameof(SubscribeChannel), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(type != null ? type : typeof(VoidParameter))
                    .Invoke(this, new object?[] { flowNode });
            }
        }

        [Endpoint(InChannelName = EngineChannels.DefaultErrorChannel)]
        public void ErrorChannel(IMessage<MessageFailure> exceptionWithMessage)
        {
            _logger.LogError("Error occured with message:{Message} and payload:{Payload}.", exceptionWithMessage, exceptionWithMessage.Payload.originalPayload, exceptionWithMessage.Payload.exception);
        }

        [Router(InChannelName = EngineChannels.RouteByHeaderChannel)]
        public string RouteByHeader(IMessageHeaders headers)
        {
            return headers.RouteToChannel;
        }

        private void HandleMessageReceived<T>(IMessage<T> message, FlowNode flowNode)
        {
            _logger.LogDebug("Received Message(Id:{id}) for Node:({nodeName})", message.MessageHeaders.Id, flowNode.Name);
            var result = InvokeFlowNodeMethod(message, flowNode);

            switch (flowNode.NodeType)
            {
                case FlowNodeType.Method:
                    InvokeForwardMessageToNextChannel(flowNode, message, result);
                    break;
                case FlowNodeType.Router:
                    _logger.LogDebug("ROUTER: forwarding message {Message} to {ChannelName}", message, result.ToString());
                    _messagingChannelProvider.GetMessagingChannel(result.ToString()).Send(message).Wait();
                    break;
            }
        }

        private object? InvokeFlowNodeMethod<T>(IMessage<T> message, FlowNode flowNode)
        {
            if (flowNode.MethodInfo.DeclaringType == null)
            {
                throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name}'s Method:{flowNode.MethodInfo.Name} must be defined on a class");
            }
            var parent = _serviceProvider.GetService(flowNode.MethodInfo.DeclaringType);
            if (parent == null)
            {
                throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name}' uses Method:{flowNode.MethodInfo.Name} on Class:{flowNode.MethodInfo.DeclaringType}. No instance of Class:{flowNode.MethodInfo.DeclaringType} is registered in the service provider.");
            }

            var parameterInfos = flowNode.MethodInfo.GetParameters();
            var parameters = new object?[parameterInfos.Length];
            foreach(var parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType == typeof(T))
                {
                    parameters[parameterInfo.Position] = message.Payload;
                }
                else if (parameterInfo.ParameterType == typeof(IMessage<T>))
                {
                    parameters[parameterInfo.Position] = message;
                }
                else if (parameterInfo.ParameterType == typeof(IMessageHeaders))
                {
                    parameters[parameterInfo.Position] = message.MessageHeaders;
                }
                else
                {
                    throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name} on {flowNode.MethodInfo.DeclaringType}.{flowNode.MethodInfo.Name} has an unsupported Parameter: {parameterInfo.ParameterType} {parameterInfo.Name}");
                }
            }

            return flowNode.MethodInfo.Invoke(parent, parameters);
        }

        private void SubscribeChannel<T>(FlowNode flowNode)
        {
            _messagingChannelProvider.GetMessagingChannel(flowNode.InChannelName).Subscribe<T>(msg => HandleMessageReceived(msg, flowNode));
        }

        private void InvokeForwardMessageToNextChannel(FlowNode flowNode, object message, object payload)
        {
            var method = GetType().GetMethod(nameof(ForwardMessageToNextChannel), BindingFlags.Instance | BindingFlags.NonPublic);
            var genericMethod = method.MakeGenericMethod(payload.GetType());
            genericMethod.Invoke(this, new object?[] { flowNode.OutChannelName, message, payload });
        }

        private void ForwardMessageToNextChannel<T>(string channel, IMessageMetaData messageMetaData, T payload)
        {
            _logger.LogDebug("ForwardMessageToNextChannel<{Type}>({Channel}, {MessageMetaData}, {Payload})", typeof(T), channel, messageMetaData, payload);
            SendMessage(channel, GenericMessage<T>.From(messageMetaData, payload)).Wait();
        }

        private Task SendMessage<T>(string channel, IMessage<T> message)
        {
            _logger.LogDebug("SendMessage<{Type}>({Channel}, {Message})", typeof(T), channel, message);
            return _messagingChannelProvider.GetMessagingChannel(channel).Send(message);
        }

        public Task Submit<T>(string channel, T payload)
        {
            return Submit(channel, new MessageHeaders(), payload);
        }

        public async Task Submit<T>(string channel, IMessageHeaders headers, T payload)
        {
            await SendMessage(channel, new GenericMessage<T>(headers, payload));
        }
    }
}

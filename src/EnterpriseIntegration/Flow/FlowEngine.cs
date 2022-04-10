using EnterpriseIntegration.Channels;
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
                _logger.LogInformation("Add channel subscription to `{channelName}`", flowNode.InChannelName);
                GetType().GetMethod(nameof(SubscribeChannel), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(getExpectedPayloadType(flowNode))
                    .Invoke(this, new object?[] { flowNode });
            }
        }

        private void MessageSubscriber<T>(IMessage<T> message, FlowNode flowNode)
        {
            _logger.LogDebug("Received Message(Id:{id}) for Node:({nodeName})", message.Id, flowNode.Name);

            var parent = _serviceProvider.GetService(flowNode.MethodInfo.DeclaringType);
            var result = flowNode.MethodInfo.Invoke(parent, new object?[] { message.Payload });

            switch (flowNode.NodeType)
            {
                case FlowNodeType.Method:
                    GetType().GetMethod(nameof(ForwardMessageToNextChannel), BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(result.GetType())
                        .Invoke(this, new object?[] { flowNode.OutChannelName, message, result });
                    break;
                case FlowNodeType.Router:
                    _messagingChannelProvider.GetMessagingChannel<T>(result.ToString()).Send(message);
                    break;
            }
        }

        private static Type? getExpectedPayloadType(FlowNode flowNode)
        {
            return flowNode.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType;
        }

        private void SubscribeChannel<T>(FlowNode flowNode)
        {
            _messagingChannelProvider.GetMessagingChannel<T>(flowNode.InChannelName).Subscribe(msg => MessageSubscriber(msg, flowNode));
        }

        private void ForwardMessageToNextChannel<T>(string channel, IMessageMetaData messageMetaData, T payload)
        {
            SendMessage(channel, GenericMessage<T>.From(messageMetaData, payload)).Wait();
        }

        private Task SendMessage<T>(string channel, IMessage<T> message)
        {
            return _messagingChannelProvider.GetMessagingChannel<T>(channel).Send(message);
        }

        public async Task Submit<T>(string channel, T payload)
        {
            await SendMessage(channel, new GenericMessage<T> { Payload = payload });
        }
    }
}

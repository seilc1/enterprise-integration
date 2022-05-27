using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.ServiceActivator
{
    public class ServiceActivatorMessageProcessor : InvokingMessageProcessor
    {

        public ServiceActivatorMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override FlowNodeType HandledType => FlowNodeType.ServiceActivator;


        public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
        {
            object? result = InvokeFlowNodeMethod(message, flowNode);
            IMessage resultMessage = AsMessage(message, result);
            await messageSender(flowNode.OutChannelName!, resultMessage);

            return new [] { resultMessage };
        }
    }
}

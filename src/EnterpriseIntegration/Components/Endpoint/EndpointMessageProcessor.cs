using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Endpoint;
public class EndpointMessageProcessor : InvokingMessageProcessor
{
    public EndpointMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override FlowNodeType HandledType => FlowNodeType.Endpoint;

    public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
    {
        InvokeFlowNodeMethod(message, flowNode);

        return Enumerable.Empty<IMessage>();
    }
}

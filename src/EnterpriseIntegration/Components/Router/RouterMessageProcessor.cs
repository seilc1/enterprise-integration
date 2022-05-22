using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Router;
public class RouterMessageProcessor : InvokingMessageProcessor
{
    public RouterMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override FlowNodeType HandledType => FlowNodeType.Router;

    public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
    {
        object? result = InvokeFlowNodeMethod(message, flowNode);

        if (result == null)
        {
            throw new EnterpriseIntegrationException($"Router Method: {flowNode.Name} did not return a channel");
        }

        if (!(result is string))
        {
            throw new EnterpriseIntegrationException($"Router Method: {flowNode.Name} did not return a string");
        }

        await messageSender((string)result, message);

        return new[] { message };
    }
}

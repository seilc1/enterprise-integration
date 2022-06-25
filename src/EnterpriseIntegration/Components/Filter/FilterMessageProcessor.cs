using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Filter
{
    public class FilterMessageProcessor : InvokingMessageProcessor
    {
        public FilterMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override FlowNodeType HandledType => FlowNodeType.Filter;

        public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
        {
            object? result = InvokeFlowNodeMethod(message, flowNode);

            if (result == null)
            {
                throw new EnterpriseIntegrationException("Method did not return any parameter.");
            }

            if (result is FilterResult filterResult)
            {
                if (filterResult.ShouldDiscard)
                {
                    return Enumerable.Empty<IMessage>();
                }

                await messageSender((ChannelId)flowNode.OutChannelId!, message);
                return new[] { message };
            }

            throw new EnterpriseIntegrationException($"Method did return parameter of type:{result.GetType()}, must be {typeof(FilterResult)} for filter.");
        }
    }
}
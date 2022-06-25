using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.History
{
    /// <summary>
    ///     History Service to push the call history into headers of the message.
    /// </summary>
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageHistory.html"/>
    public class HistoryService : IPreAction
    {
        public Task PreProcess(FlowNode flowNode, IMessage message)
        {
            return Task.Run(() => message.MessageHeaders.PushHistoryItem(flowNode.Name));
        }
    }
}
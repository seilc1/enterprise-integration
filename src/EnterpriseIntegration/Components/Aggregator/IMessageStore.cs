using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Aggregator;

public interface IMessageStore
{
    Task AddMessage(IMessage message);

    Task<IMessage> GetMessage(string id);

    Task RemoveMessage(string messageId);

    IAsyncEnumerable<IMessage> GetMessagesByGroupId(string groupId);

    Task RemoveMessagesByGroupId(string groupId);
}

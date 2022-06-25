using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Aggregator;

public interface IMessageStore
{
    Task<ICollection<IMessage>> AddMessage(IMessage message);

    Task<IMessage> GetMessage(string id);

    Task RemoveMessage(string messageId);

    Task RemoveMessagesByGroupId(string groupId);
}

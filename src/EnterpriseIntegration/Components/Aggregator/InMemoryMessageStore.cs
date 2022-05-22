using EnterpriseIntegration.Components.Splitter;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Aggregator;

public class InMemoryMessageStore : IMessageStore
{
    IList<IMessage> messages = new List<IMessage>();

    public Task AddMessage(IMessage message)
    {
        messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<IMessage> GetMessage(string id)
    {
        return Task.FromResult(messages.Single(m => m.MessageHeaders.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
    }

    public async IAsyncEnumerable<IMessage> GetMessagesByGroupId(string groupId)
    {
        foreach (IMessage message in messages.Where(m => m.MessageHeaders.GetMessageGroupId() != null && m.MessageHeaders.GetMessageGroupId()!.Equals(groupId)))
        {
            yield return message;
        }
    }

    public async Task RemoveMessage(string messageId)
    {
        messages.Remove(await GetMessage(messageId));
    }

    public async Task RemoveMessagesByGroupId(string groupId)
    {
        await foreach(IMessage message in GetMessagesByGroupId(groupId))
        {
            messages.Remove(message);
        }
    }
}
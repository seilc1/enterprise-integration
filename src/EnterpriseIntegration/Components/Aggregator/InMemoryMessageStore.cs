using EnterpriseIntegration.Components.Splitter;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Aggregator;

public class InMemoryMessageStore : IMessageStore
{
    private readonly IList<IMessage> messages = new List<IMessage>();

    public Task<IMessage> GetMessage(string id)
    {
        return Task.FromResult(messages.Single(m => m.MessageHeaders.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
    }

    public async IAsyncEnumerable<IMessage> GetMessagesByGroupId(string groupId)
    {
        await foreach (IMessage message in messages.Where(m => m.MessageHeaders.GetMessageGroupId() != null && m.MessageHeaders.GetMessageGroupId()!.Equals(groupId)).ToAsyncEnumerable())
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

    public Task<ICollection<IMessage>> AddMessage(IMessage message)
    {
        lock (this)
        {
            string groupId = message.MessageHeaders.GetMessageGroupId()!;
            messages.Add(message);
            return Task.FromResult((ICollection<IMessage>)messages.Where(m => m.MessageHeaders.GetMessageGroupId() != null && m.MessageHeaders.GetMessageGroupId()!.Equals(groupId)).ToList());
        }
    }
}
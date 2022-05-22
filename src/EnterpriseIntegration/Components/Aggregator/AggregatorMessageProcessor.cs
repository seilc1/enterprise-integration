using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Components.Splitter;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Aggregator;

public class AggregatorMessageProcessor : InvokingMessageProcessor
{
    private readonly IMessageStore _messageStore;

    public AggregatorMessageProcessor(IMessageStore messageStore, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _messageStore = messageStore;
    }

    public override FlowNodeType HandledType => FlowNodeType.Aggregator;

    public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
    {
        string? groupId = message.MessageHeaders.GetMessageGroupId();
        if (groupId == null)
        {
            throw new EnterpriseIntegrationException("Aggregated Message has no groupId present.");
        }

        ICollection<IMessage> receivedMessages = await GetReceivedMessages(groupId);
        receivedMessages.Add(message);
        AggregatorAttribute attribute = (AggregatorAttribute)flowNode.Attribute;

        if (!IsConditionSatisfied(attribute, receivedMessages))
        {
            await _messageStore.AddMessage(message);
            return Enumerable.Empty<IMessage>();
        }


        // TODO: fix
        // object? result = InvokeFlowNodeMethod(receivedMessages, flowNode);
        object? result = InvokeFlowNodeMethod(message, flowNode);

        IMessage resultMessage = AsMessage(message, result);
        resultMessage.MessageHeaders.Id = message.MessageHeaders.GetOriginalId() ?? resultMessage.MessageHeaders.Id;
        resultMessage.MessageHeaders.SetMessageGroupId(null);
        resultMessage.MessageHeaders.SetMessageGroupIndex(null);
        resultMessage.MessageHeaders.SetMessageGroupCount(null);

        await messageSender(flowNode.OutChannelName!, resultMessage);

        return new[] { resultMessage };
    }

    private async Task<ICollection<IMessage>> GetReceivedMessages(string groupId)
    {
        ICollection<IMessage> receivedMessages = new List<IMessage>();
        await foreach (IMessage groupedMessage in _messageStore.GetMessagesByGroupId(groupId))
        {
            receivedMessages.Add(groupedMessage);
        }

        return receivedMessages;
    }

    private bool IsConditionSatisfied(AggregatorAttribute attribute, ICollection<IMessage> messages)
    {
        string? groupCount = messages.First().MessageHeaders.GetMessageGroupCount();
        if (groupCount == null)
        {
            throw new EnterpriseIntegrationException("Received aggregated messages has no group count defined.");
        }

        return messages.Count == int.Parse(groupCount);
    }
}
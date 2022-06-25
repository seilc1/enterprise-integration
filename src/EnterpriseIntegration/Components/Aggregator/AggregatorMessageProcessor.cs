using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
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

        ICollection<IMessage> receivedMessages = await _messageStore.AddMessage(message);
        AggregatorAttribute attribute = (AggregatorAttribute)flowNode.Attribute;

        if (!IsConditionSatisfied(attribute, receivedMessages))
        {
            return Enumerable.Empty<IMessage>();
        }

        object? result = InvokeFlowNodeMethod<T>(receivedMessages, flowNode);
        
        IMessage resultMessage = AsMessage(message, result);
        resultMessage.MessageHeaders.Id = message.MessageHeaders.GetOriginalId() ?? resultMessage.MessageHeaders.Id;
        resultMessage.MessageHeaders.SetMessageGroupId(null);
        resultMessage.MessageHeaders.SetMessageGroupIndex(null);
        resultMessage.MessageHeaders.SetMessageGroupCount(null);

        await messageSender((ChannelId)flowNode.OutChannelId!, resultMessage);

        return new[] { resultMessage };
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

    protected object? InvokeFlowNodeMethod<T>(IEnumerable<IMessage> messages, FlowNode flowNode)
    {
        var parent = GetInstantiatedClassDeclaringMethod(flowNode);

        var parameterInfos = flowNode.MethodInfo.GetParameters();
        var parameters = new object?[parameterInfos.Length];
        foreach (var parameterInfo in parameterInfos)
        {
            if (parameterInfo.ParameterType == typeof(IEnumerable<T>))
            {
                parameters[parameterInfo.Position] = messages.Select(m => (T)m.Payload);
            }
            else if (parameterInfo.ParameterType == typeof(IEnumerable<IMessage<T>>))
            {
                parameters[parameterInfo.Position] = messages;
            }
            else if (parameterInfo.ParameterType == typeof(IEnumerable<IMessageHeaders>))
            {
                parameters[parameterInfo.Position] = messages.Select(m => m.MessageHeaders);
            }
            else
            {
                throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name} on {flowNode.MethodInfo.DeclaringType}.{flowNode.MethodInfo.Name} has an unsupported Parameter: {parameterInfo.ParameterType} {parameterInfo.Name}");
            }
        }

        return flowNode.MethodInfo.Invoke(parent, parameters);
    }
}
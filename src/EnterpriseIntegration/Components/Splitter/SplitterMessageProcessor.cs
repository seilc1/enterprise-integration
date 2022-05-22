using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;
using System.Collections;

namespace EnterpriseIntegration.Components.Splitter;

public class SplitterMessageProcessor : InvokingMessageProcessor
{
    public SplitterMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override FlowNodeType HandledType => FlowNodeType.Splitter;

    public override async Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender)
    {
        IEnumerable? results = InvokeFlowNodeMethod(message, flowNode) as IEnumerable;

        if (results == null)
        {
            throw new EnterpriseIntegrationException($"Result of Splitter: {flowNode.Name} is not an IEnumerable");
        }

        IList<object> payloads = results.Cast<object?>().Where(it => it != null).Select(it => it!).ToList();
        IList<Task> sendTasks = new List<Task>(payloads.Count);
        IList<IMessage> returnResults = new List<IMessage>(payloads.Count);
        int index = 0;
        string groupId = Guid.NewGuid().ToString();
        foreach (object result in payloads)
        {
            IMessage newMessage = AsMessage(message, result);
            newMessage.MessageHeaders.Id = Guid.NewGuid().ToString();
            newMessage.MessageHeaders.SetOriginalId(message.MessageHeaders.Id);
            newMessage.MessageHeaders.SetMessageGroupId(groupId);
            newMessage.MessageHeaders.SetMessageGroupIndex($"{index++}");
            newMessage.MessageHeaders.SetMessageGroupCount(payloads.Count.ToString());

            returnResults.Add(newMessage);
            sendTasks.Add(messageSender(flowNode.OutChannelName!, newMessage));
        }

        await Task.WhenAll(sendTasks);

        return returnResults;
    }
}

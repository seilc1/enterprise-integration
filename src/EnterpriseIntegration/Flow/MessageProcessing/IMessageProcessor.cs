using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Flow.MessageProcessing;

public interface IMessageProcessor
{
    /// <summary>
    ///     Type handled by this Processor.
    /// </summary>
    public FlowNodeType HandledType { get; }

    /// <summary>
    ///     Method to process a <see cref="IMessage"/> and return all information to properly handle it.
    /// </summary>
    public Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender);
}

public delegate Task SendMessageAsync(ChannelId channelId, IMessage message);

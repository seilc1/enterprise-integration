using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    /// <summary>
    /// Channel to send and receive <see cref="IMessage{T}"/>.
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingChannelsIntro.html"/>
    /// </summary>
    public interface IMessagingChannel
    {
        /// <summary>
        ///     Id of the Enterprise Integration Channel.
        /// </summary>
        /// <remarks>
        ///     This might vary from the Name the Channel/Queue/Topic has in the Implementation and is solely used
        ///     by the Enterprise Integration Framework to match the channel.
        /// </remarks>
        public ChannelId ChannelId { get; }

        /// <summary>
        ///     Sends a <see cref="IMessage"/> to the channel, how the message is handled, depends on the implementation (Point-To-Point, Publish-Subscriber, ...).
        /// </summary>
        public Task Send(IMessage message);

        /// <summary>
        ///     Subscribe method, to add a new async subscriber to the channel.
        /// </summary>
        public Task Subscribe<T>(Func<IMessage<T>, Task> subscriber);
    }
}

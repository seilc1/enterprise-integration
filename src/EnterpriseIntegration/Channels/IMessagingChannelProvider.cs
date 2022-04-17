using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    /// <summary>
    ///     Provides a simple channel implementation to send and receive messages
    /// </summary>
    public interface IMessagingChannelProvider
    {
        /// <summary>
        ///     Gets or Creates a <see cref="IMessagingChannel"/> for a channel name.
        /// </summary>
        public IMessagingChannel GetMessagingChannel(string channelName);
    }
}

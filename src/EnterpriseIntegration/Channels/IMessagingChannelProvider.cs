namespace EnterpriseIntegration.Channels
{
    /// <summary>
    ///     Provides a simple channel implementation to send and receive messages
    /// </summary>
    public interface IMessagingChannelProvider
    {
        /// <summary>
        /// Gets or Creates a Channel for the requested <see cref="Type"/> and channelName.
        /// </summary>
        public IMessagingChannel<T> GetMessagingChannel<T>(string channelName);
    }
}

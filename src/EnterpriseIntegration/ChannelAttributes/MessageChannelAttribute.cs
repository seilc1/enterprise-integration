namespace EnterpriseIntegration.ChannelAttributes
{
    /// <summary>
    /// Channel receives and sends an <see cref="Message.IMessage{T}"/>.
    /// </summary>
    public class MessageChannelAttribute : BaseChannelAttribute
    {
        /// <summary>
        /// Name of the next Channel the return value should be forwarded to.
        /// </summary>
        public string OutChannelName { get; init; }
    }
}

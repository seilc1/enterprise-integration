namespace EnterpriseIntegration.Channels
{
    public static class EngineChannels
    {
        /// <summary>
        ///     Default error channel, all exceptions thrown are forwarded to this channel, if no other channel
        ///     is defined with <see cref="Message.IMessageHeaders.ErrorChannel"/>.
        /// </summary>
        public const string DefaultErrorChannel = "ei-error-channel";


        /// <summary>
        ///     Channel which allows forwarding a <see cref="Message.IMessage{T}"/> to a channel, be providing the
        ///     Channelname in the message header field: .
        /// </summary>
        public const string RouteByHeaderChannel = "ei-route-by-header";
    }
}

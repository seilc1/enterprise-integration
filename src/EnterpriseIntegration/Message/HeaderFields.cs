namespace EnterpriseIntegration.Message
{
    public static class HeaderFields
    {
        /// <summary>
        ///     <see cref="Guid"/> based unique Id of the <see cref="IMessage{T}"/>.
        /// </summary>
        public const string MessageId = "EI_MESSAGE_ID";

        /// <summary>
        ///     ISO8601 formatted date string, when the <see cref="IMessage{T}"/> was initially created.
        ///     <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip">ISO8601 round-trip format</see>
        /// </summary>
        public const string MessageCreateDate = "EI_MESSAGE_CREATE_DATE";

        /// <summary>
        ///     Name of the <see cref="Channels.IMessagingChannel"/> to which the message should be forwarded, if an exception in the flow happens.
        /// </summary>
        public const string ErrorChannel = "EI_ERROR_CHANNEL";

        /// <summary>
        ///     By setting this field and sending a message to the channel: <see cref="Channels.EngineChannels.RouteByHeaderChannel"/> the message will be forwarded to
        ///     the defined channel.
        /// </summary>
        public const string RouteToChannel = "EI_ROUTE_TO_CHANNEL";
    }
}

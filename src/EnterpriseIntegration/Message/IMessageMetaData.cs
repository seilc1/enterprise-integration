namespace EnterpriseIntegration.Message
{
    public interface IMessageMetaData
    {        
        /// <summary>
        /// Id of the <see cref="IMessage{T}"/>, which remains the same during the complete flow.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Headers for this Message, which are transported through the complete flow.
        /// </summary>
        public Dictionary<string, string> MessageHeaders { get; }

        /// <summary>
        ///     Bag to store and transport objects with the message.
        ///     The flow will try best effort wise to transport these data, which in some cases is not feasible
        ///     - e.g. serialization when handing over to a queue like rabbitmq.
        ///     Use this only if you are sure that the Channel is able to transport it.
        /// </summary>
        public Dictionary<string, object> MessageBag { get; }
    }
}

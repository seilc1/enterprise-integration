namespace EnterpriseIntegration.Message
{
    public record GenericMessage<T> : IMessage<T>
    {
        public GenericMessage(T payload) : this(new MessageHeaders(), payload)
        { }

        public GenericMessage(IMessageHeaders headers, T payload)
        {
            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            MessageHeaders = headers;
            Payload = payload;
            PayloadType = payload.GetType();
        }

        public T Payload { get; }

        public Dictionary<string, object> MessageBag { get; init; } = new Dictionary<string, object>();

        public IMessageHeaders MessageHeaders { get; init; } = new MessageHeaders();

        public Type PayloadType { get; init; }

        object IMessage.Payload => Payload!;

        public static GenericMessage<T> From(IMessage message, T payload)
        {
            return new GenericMessage<T>(message.MessageHeaders, payload)
            {
                MessageBag = message.MessageBag
            };
        }

        public static GenericMessage<T> WithNewHeaders(IMessage message, T payload)
        {
            return new GenericMessage<T>(Message.MessageHeaders.CopyFrom(message.MessageHeaders), payload)
            {
                MessageBag = message.MessageBag
            };
        }
    }
}

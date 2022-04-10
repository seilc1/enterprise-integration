namespace EnterpriseIntegration.Message
{
    public record GenericMessage<T> : IMessage<T>
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public T? Payload { get; init; }

        public Dictionary<string, string> MessageHeaders { get; init; } = new Dictionary<string, string>();

        public Dictionary<string, object> MessageBag { get; init; } = new Dictionary<string, object>();

        public static GenericMessage<T> From(IMessageMetaData metaData, T payload)
        {
            return new GenericMessage<T>
            {
                Id = metaData.Id,
                Payload = payload,
                MessageHeaders = metaData.MessageHeaders,
                MessageBag = metaData.MessageBag
            };
        }
    }
}

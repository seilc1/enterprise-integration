using System.Diagnostics;

namespace EnterpriseIntegration.Message
{
    public record GenericMessage<T> : IMessage<T>
    {
        public GenericMessage(T payload)
        {
            Debug.Assert(payload != null);

            MessageHeaders.Id = Guid.NewGuid().ToString();
            MessageHeaders.CreatedDate = DateTime.Now;
            Payload = payload;
            PayloadType = Payload.GetType();
        }

        public T Payload { get; init; }

        public Dictionary<string, object> MessageBag { get; init; } = new Dictionary<string, object>();

        public IMessageHeaders MessageHeaders { get; init; } = new MessageHeaders();

        public Type PayloadType { get; init; }

        public static GenericMessage<T> From(IMessageMetaData metaData, T payload)
        {
            return new GenericMessage<T>(payload)
            {
                MessageHeaders = metaData.MessageHeaders,
                MessageBag = metaData.MessageBag
            };
        }
    }
}

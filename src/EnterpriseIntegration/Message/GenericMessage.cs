using System.Diagnostics;

namespace EnterpriseIntegration.Message
{
    public record GenericMessage<T> : IMessage<T>
    {
        public GenericMessage(T payload) : this(new MessageHeaders(), payload)
        { }

        public GenericMessage(IMessageHeaders headers, T? payload)
        {
            MessageHeaders = headers;
            MessageHeaders.Id = Guid.NewGuid().ToString();
            MessageHeaders.CreatedDate = DateTime.Now;
            Payload = payload;
            PayloadType = payload == null ? typeof(object) : payload.GetType();
        }

        public T? Payload { get; init; }

        public Dictionary<string, object> MessageBag { get; init; } = new Dictionary<string, object>();

        public IMessageHeaders MessageHeaders { get; init; } = new MessageHeaders();

        public Type PayloadType { get; init; }

        object IMessage.Payload => Payload;

        public static GenericMessage<T> From(IMessageMetaData metaData, T? payload)
        {
            return new GenericMessage<T>(metaData.MessageHeaders, payload)
            {
                MessageBag = metaData.MessageBag
            };
        }
    }
}

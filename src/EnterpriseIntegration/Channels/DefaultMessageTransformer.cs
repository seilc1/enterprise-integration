using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    public class DefaultMessageTransformer : IMessageTransformer
    {
        public IMessage<T> TransformMessage<T>(object message)
        {
            if (!message.GetType().IsMessage())
            {
                throw new EnterpriseIntegrationException($"param:{nameof(message)} must implement IMessage<T> but is of type:{message.GetType()}.");
            }

            return GenericMessage<T>.From((IMessageMetaData)message, TransformPayload<T>(IMessage<object>.ReflectPayload(message)));
        }

        public static T TransformPayload<T>(object payload)
        {
            if (payload == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(payload, typeof(T));
        }
    }
}

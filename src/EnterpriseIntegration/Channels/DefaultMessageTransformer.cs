using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
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

            return GenericMessage<T>.From((IMessage)message, TransformPayload<T>(IMessage.ReflectPayload(message)));
        }

        public static T? TransformPayload<T>(object payload)
        {
            if (payload == null)
            {
                return default(T);
            }

            if (typeof(T) == typeof(VoidParameter))
            {
                return default(T);
            }

            if (typeof(T) == typeof(object) || typeof(T) == payload.GetType())
            {
                return (T)payload;
            }

            try
            {
                return (T)Convert.ChangeType(payload, typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new PayloadTransformationException(payload.GetType(), typeof(T));
            }
        }
    }
}

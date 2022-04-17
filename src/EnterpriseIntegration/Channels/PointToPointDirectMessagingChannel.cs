using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    public class PointToPointDirectMessagingChannel : IMessagingChannel
    {
        private Action<object>? _subscriber;

        public Task Send<T>(IMessage<T> message)
        {
            return Task.Run(() => _subscriber?.Invoke(message));
        }

        public Task Subscribe<T>(Action<IMessage<T>> subscriber)
        {
            if (_subscriber != null)
            {
                throw new EnterpriseIntegrationException($"{nameof(PointToPointDirectMessagingChannel)} only supports a single subscriber.");
            }

            _subscriber = msg => subscriber.Invoke(transformMessage<T>(msg));
            return Task.CompletedTask;
        }

        public static IMessage<T> transformMessage<T>(object message)
        {
            if (!message.GetType().IsMessage())
            {
                throw new EnterpriseIntegrationException("Not a message");
            }

            return GenericMessage<T>.From((IMessageMetaData)message, transformPayload<T>(message));
        }

        public static T transformPayload<T>(object message)
        {
            return (T)typeof(PointToPointDirectMessagingChannel).GetMethod(nameof(Unpack))
                .MakeGenericMethod(typeof(T), IMessage<object>.ReflectPayloadType(message))
                .Invoke(null, new object?[] { IMessage<object>.ReflectPayload(message) });
        }

        public static ToType Unpack<FromType, ToType>(FromType payload) where ToType : class
        {
            if (payload == null)
            {
                return null;
            }

            if (typeof(FromType).IsAssignableTo(typeof(ToType)))
            {
                return payload as ToType;
            }
            if (typeof(ToType) == typeof(object))
            {
                return payload as ToType;
            }
            if (typeof(ToType) == typeof(string))
            {
                return payload.ToString() as ToType;
            }

            throw new EnterpriseIntegrationException($"unsupported unpack from {typeof(FromType)} to {typeof(ToType)}");
        }
    }
}

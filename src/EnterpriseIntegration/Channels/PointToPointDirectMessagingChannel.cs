using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    public class PointToPointDirectMessagingChannel : IMessagingChannel
    {
        private Action<object>? _subscriber;

        private static IMessageTransformer _transformer = new DefaultMessageTransformer();

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

            _subscriber = msg => subscriber.Invoke(_transformer.TransformMessage<T>(msg));
            return Task.CompletedTask;
        }

    }
}

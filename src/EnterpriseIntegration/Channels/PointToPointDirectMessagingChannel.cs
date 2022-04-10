using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    public class PointToPointDirectMessagingChannel<T> : IMessagingChannel<T>
    {
        private Action<IMessage<T>>? _subscriber;

        public Task Send(IMessage<T> message)
        {
            return Task.Run(() => _subscriber?.Invoke(message));
        }

        public Task Subscribe(Action<IMessage<T>> subscriber)
        {
            if (_subscriber != null)
            {
                throw new EnterpriseIntegrationException($"{nameof(PointToPointDirectMessagingChannel<T>)} only supports a single subscriber.");
            }

            this._subscriber = subscriber;
            return Task.CompletedTask;
        }
    }
}

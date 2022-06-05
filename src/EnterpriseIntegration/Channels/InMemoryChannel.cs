using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    public class InMemoryChannel : IMessagingChannel
    {
        private Func<IMessage, Task>? _subscriber;

        private static IMessageTransformer _transformer = new DefaultMessageTransformer();

        public ChannelId ChannelId { get; }

        public InMemoryChannel(ChannelId id)
        {
            ChannelId = id;
        }

        public async Task Send(IMessage message)
        {
            if (_subscriber != null)
            {
                await _subscriber.Invoke(message);
            }
        }

        public Task Subscribe<T>(Func<IMessage<T>, Task> subscriber)
        {
            if (_subscriber != null)
            {
                throw new EnterpriseIntegrationException($"{nameof(InMemoryChannel)} only supports a single subscriber.");
            }

            _subscriber = msg => subscriber.Invoke(_transformer.TransformMessage<T>(msg));
            return Task.CompletedTask;
        }
    }
}

using System.Collections.Concurrent;

namespace EnterpriseIntegration.Channels
{
    public class InMemoryMessagingChannelProvider : IMessagingChannelProvider
    {
        private ConcurrentDictionary<string, object> MessageChannels { get; init; } = new ConcurrentDictionary<string, object>();

        public IMessagingChannel<T> GetMessagingChannel<T>(string channelName)
        {
            return MessageChannels.GetOrAdd(channelName, CreateMessagingChannel<T>()) as IMessagingChannel<T>;
        }

        private IMessagingChannel<T> CreateMessagingChannel<T>()
        {
            return new PointToPointDirectMessagingChannel<T>();
        }
    }
}

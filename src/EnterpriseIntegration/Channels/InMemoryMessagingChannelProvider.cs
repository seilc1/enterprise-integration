using System.Collections.Concurrent;

namespace EnterpriseIntegration.Channels
{
    public class InMemoryMessagingChannelProvider : IMessagingChannelProvider
    {
        private ConcurrentDictionary<ChannelId, IMessagingChannel> MessageChannels { get; init; }

        public InMemoryMessagingChannelProvider(IEnumerable<IMessagingChannel> preDefinedChannels)
        {
            MessageChannels = new ConcurrentDictionary<ChannelId, IMessagingChannel>(preDefinedChannels.Select(e => KeyValuePair.Create(e.ChannelId, e)));
        }

        private static IMessagingChannel CreateMessagingChannel(ChannelId channelId)
        {
            return new InMemoryChannel(channelId);
        }

        public IMessagingChannel GetMessagingChannel(ChannelId channelId)
        {
            return MessageChannels.GetOrAdd(channelId, CreateMessagingChannel(channelId));
        }

        public void Dispose()
        {
            MessageChannels.Clear();
        }
    }
}

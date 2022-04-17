using EnterpriseIntegration.Message;
using System.Collections.Concurrent;

namespace EnterpriseIntegration.Channels
{
    public class InMemoryMessagingChannelProvider : IMessagingChannelProvider
    {
        private ConcurrentDictionary<string, IMessagingChannel> MessageChannels { get; init; } = new ConcurrentDictionary<string, IMessagingChannel>();

        private static IMessagingChannel CreateMessagingChannel()
        {
            return new PointToPointDirectMessagingChannel();
        }

        public IMessagingChannel GetMessagingChannel(string channelName)
        {
            return MessageChannels.GetOrAdd(channelName, CreateMessagingChannel());
        }
    }
}

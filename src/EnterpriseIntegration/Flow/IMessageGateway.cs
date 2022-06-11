using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Flow;

public interface IMessageGateway
{
    public Task Send<T>(ChannelId channelId, T payload);

    public Task Send<T>(ChannelId channelId, IMessageHeaders headers, T payload);

    public Task SendMessage(ChannelId channelId, IMessage message);
}
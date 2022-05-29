using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Flow;

public interface IMessageGateway
{
    public Task SendPayload<T>(string channel, T payload);

    public Task SendPayload<T>(string channel, IMessageHeaders headers, T payload);

    public Task SendMessage(string channel, IMessage message);
}
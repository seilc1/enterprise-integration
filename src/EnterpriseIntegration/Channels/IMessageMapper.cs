using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels;

public interface IMessageMapper<TReceive, TSend>
{
    Task<TSend> Map(IMessage message);

    Task<IMessage<T>> Map<T>(TReceive result);
}
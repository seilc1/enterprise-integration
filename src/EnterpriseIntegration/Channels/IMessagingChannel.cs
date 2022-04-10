using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    /// <summary>
    /// Channel to send and receive <see cref="IMessage{T}"/>.
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingChannelsIntro.html"/>
    /// </summary>
    public interface IMessagingChannel<T>
    {
        public Task Send(IMessage<T> message);

        public Task Subscribe(Action<IMessage<T>> subscriber);
    }
}

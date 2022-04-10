using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    /// <summary>
    ///     A Point-to-Point Channel ensures that only one receiver consumes any given message. 
    ///     If the channel has multiple receivers, only one of them can successfully consume a particular message. 
    ///     If multiple receivers try to consume a single message, the channel ensures that only one of them succeeds, so the receivers do not have to coordinate with each other. 
    ///     The channel can still have multiple receivers to consume multiple messages concurrently, but only a single receiver consumes any one message.
    ///     <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/PointToPointChannel.html"/>
    /// </summary>
    public class PointToPointInMemoryMessagingChannel<T> : IMessagingChannel<T>
    {
        private IList<Action<IMessage<T>>> Subscribers { get; init; } = new List<Action<IMessage<T>>>();

        public Task Send(IMessage<T> message)
        {
            if (Subscribers.Any())
            {
                _ = Task.Run(() => Subscribers.First().Invoke(message));
            }

            return Task.CompletedTask;
        }

        public Task Subscribe(Action<IMessage<T>> subscriber)
        {
            Subscribers.Add(subscriber);
            return Task.CompletedTask;
        }
    }
}

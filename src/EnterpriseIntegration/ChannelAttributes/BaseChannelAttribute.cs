using EnterpriseIntegration.Flow;

namespace EnterpriseIntegration.ChannelAttributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class BaseChannelAttribute : Attribute
    {
        /// <summary>
        /// Name of the channel, where messages are received for this channel.
        /// </summary>
        public string InChannelName { get; init; }
    }
}

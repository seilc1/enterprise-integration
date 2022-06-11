using EnterpriseIntegration.Channels;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Procudes separate messages from a <see cref="IEnumerable{T}"/> result.
    ///     How the messages are consumed is configured in the 
    /// </summary>
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/Sequencer.html">Splitter</see>
    public class SplitterAttribute : ServiceActivatorAttribute
    {
        public SplitterAttribute(string inChannelId, string outChannelId) : base(inChannelId, outChannelId)
        {
        }
    }
}

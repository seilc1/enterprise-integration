using EnterpriseIntegration.Components.Aggregator;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Consumes and stores <see cref="IMessage"/> until the condition is true.
    /// </summary>
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/Aggregator.html">EIP: Aggregator</see>
    public class AggregatorAttribute : ServiceActivatorAttribute
    {
        public AggregatorAttribute(string inChannelName, string outChannelName) : base(inChannelName, outChannelName) { }

        /// <summary>
        ///     Defines the Maximum <see cref="TimeSpan"/> the aggregator should wait until executing the <see cref="MaxWaitAfterFirstMessageStrategy"/>.
        /// </summary>
        /// <remarks>
        ///     Null(default) means no Timeout, e.g. wait for ever.
        /// </remarks>
        public TimeSpan? MaxWaitAfterFirstMessage { get; init; } = null;

        /// <summary>
        ///     Defines the strategy to be executed, when the <see cref="MaxWaitAfterFirstMessage"/> time expires, (default: <see cref="MaxWaitAfterFirstMessageStrategy.Error"/>.
        /// </summary>
        public MaxWaitAfterFirstMessageStrategy MaxWaitAfterFirstMessageStrategy { get; init; } = MaxWaitAfterFirstMessageStrategy.Error;
    }
}

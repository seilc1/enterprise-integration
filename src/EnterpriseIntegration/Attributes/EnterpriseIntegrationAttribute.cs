namespace EnterpriseIntegration.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class EnterpriseIntegrationAttribute : Attribute
    {
        /// <summary>
        /// Name of the channel, where messages are received for this channel.
        /// </summary>
        public string InChannelName { get; init; }
    }
}

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     This method decides the name of the next step of the flow.
    ///     The Method must return a <see cref="string"/> name of the next channel.
    ///     <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageRouter.html"/>
    /// </summary>
    public class RouterAttribute : EnterpriseIntegrationAttribute
    {
        public RouterAttribute(string inChannelName) : base(inChannelName)
        {
        }
    }
}

using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.Tests.Examples
{
    public class RoutingFlow003
    {
        private readonly ILogger logger;

        public double ValueAtTheEnd { get; private set; }

        public RoutingFlow003(ILogger logger)
        {
            this.logger = logger;
        }

        [ServiceActivator(InChannelName = "003-start", OutChannelName = EngineChannels.RouteByHeaderChannel)]
        public double Start(IMessageHeaders headers)
        {
            headers.RouteToChannel = "003-end";
            return 13.37;
        }

        [Endpoint(InChannelName = "003-end")]
        public void End(double result)
        {
            ValueAtTheEnd = result;
        }
    }
}

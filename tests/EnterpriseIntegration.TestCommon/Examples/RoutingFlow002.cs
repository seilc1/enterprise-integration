using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.TestCommon.Examples;

public class RoutingFlow002
{
    public record Message(string RouteTo);

    [ServiceActivator(inChannelName: "002-start", outChannelName: EngineChannels.RouteByHeaderChannel)]
    public double Start(Message message, IMessageHeaders headers)
    {
        headers.RouteToChannel = message.RouteTo;
        return 13.37;
    }

    [Endpoint(inChannelName: "002-route-001")]
    public void End001(double result)
    {
    }

    [Endpoint(inChannelName: "002-route-002")]
    public void End002(double result)
    {
    }
}

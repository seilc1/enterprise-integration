# .NET 6 Implementation of Enterprise Integration Pattern

Following the definition of [EIP (Enterprise Integration Pattern)](https://www.enterpriseintegrationpatterns.com/patterns/messaging)
inspired by the [Spring Integration](https://spring.io/projects/spring-integration).

This Project does not claim complete implementation but best effort - Feel free to help expand the project

## Quick start

The Framework is working with Attributes to build the Integration Flow:

_Example of a flow. A flow could be split over several classes, the different steps are connected by the name of the channels:_
```C#
using EnterpriseIntegration.ChannelAttributes;

public class ExampleFlow
{
    [MessageChannel(InChannelName = "hello", OutChannelName = "world")]
    public string Hello(string prefix)
    {
        return $"{prefix} hello";
    }

    [MessageChannel(InChannelName = "world", OutChannelName = "random")]
    public string World(string data)
    {
        return $"{data} world";
    }

    [MessageRouter(InChannelName = "random")]
    public string Randomizer(string data)
    {
        return Random.Shared.NextInt64() % 2 == 0 ? "hello" : "end";
    }

    [MessageTerminator(InChannelName = "end")]
    public void End(string data)
    {
        logger.LogInformation($"{data}.");
    }
}
```


_Starting the flow by sending a message:_
```C#
// to be injected by dependency injection
FlowEngine flowEngine;
await flowEngine.Submit("hello", "FLOW:");
```

### Diagram
![EIP Diagram](doc/example-flow.drawio.svg "example flow diagram")

## Channels
see: [EIP: Messaging Channels](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingChannelsIntro.html)

### PointToPointDirectMessagingChannel
see: [EIP: Point to Point Channel](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PointToPointChannel.html)

Is an InMemory Channel allowing to connect two endpoints with eachother. The channel is One-to-One connection, directly moving the return value of one endpoint
to the next endpoint.

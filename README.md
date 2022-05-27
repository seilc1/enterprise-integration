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
    [ServiceActivator(InChannelName = "hello", OutChannelName = "world")]
    public string Hello(string prefix)
    {
        return $"{prefix} hello";
    }

    [ServiceActivator(InChannelName = "world", OutChannelName = "random")]
    public string World(string data)
    {
        return $"{data} world";
    }

    [Router(InChannelName = "random")]
    public string Randomizer(string data)
    {
        return Random.Shared.NextInt64() % 2 == 0 ? "hello" : "end";
    }

    [Endpoint(InChannelName = "end")]
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

# Feature overview

| Feature | Status | Description |
| :-- | :--: | :-- |
| [ServiceActivator](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingAdapter.html) | DONE | Allows to define a method which receives and sends a Message. |
| [Router](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageRouter.html) | DONE | Allows to define the next channel based on Conditions. |
| [Endpoint](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageEndpoint.html) | DONE | Allows to define a method which only receives a Message. |
| [Splitter](https://www.enterpriseintegrationpatterns.com/patterns/messaging/Sequencer.html) | DONE | Allows to split a single Message to several Messages (,to be aggregated again). |
| [Aggregator](https://www.enterpriseintegrationpatterns.com/patterns/messaging/Aggregator.html) | DONE | Allows to aggregate several Messages back into one (after being split).|
| [Filter](https://www.enterpriseintegrationpatterns.com/patterns/messaging/Filter.html) | NOT YET IN | Allows to only continue with a subset of Messages |
| [WireTap](https://www.enterpriseintegrationpatterns.com/patterns/messaging/WireTap.html) | DONE | (PRE/POSTAction) Allows to consume Messages without being part of the flow |
| [History](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageHistory.html) | NOT YET IN | (POSTAction) Allows to Track the History of an Message |

## Components

### WireTap

WireTaps are used to listen to Messages without interrupting the Flow (for testing/debugging).

```C#
// to be injected by dependency injection
IWireTapService wireTapService

IMessage result = null;
// first parameter: name of the channel to be tapped
// second parameter: method to be executed, when a message arrives (in this example it stores the message in a variable)
WireTapId id = _wireTapService.CreateWireTap("name_of_channel", async msg => result = msg);

...

// remove the wiretap, when you are finished, to reduce overhead.
_wireTapService.RemoveWireTap(id);
```

### ServiceActivator

* PreActions
* Execute Method with Received Message from Defined-InChannel
* PostActions
* Send Return-Value to Defined-OutChannel

### Router

* PreActions
* Execute Method with ReceivedMessage from Defined-InChannel
* PostActions
* Send ReceivedMessage to Return-Value

### Endpoint

* PreActions
* Execute Method with Received Message from Defined-InChannel
* PostActions

### Splitter

* PreActions
* Execute Method with Received Message from Defined-InChannel
* PostActions
* Split Return-Value into separate Messages
    * Send each separate Message to Defined-OutChannel

### Aggregator

* PreActions
* Check if Store + New Message satisfy aggregation condition
    * *If:YES*
    * Execute Method with collected Messages
    * PostActions
    * Send Return-Value to Defined-OutChannel

    * *If:NO*
    * Push Message into Store
    * PostActions

### Filter

* PreActions
* Execute Method with Received Message from Defined-InChannel
* PostActions
* Send Return-Value to Defined-OutChannel

## Channels
see: [EIP: Messaging Channels](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingChannelsIntro.html)

### PointToPointDirectMessagingChannel
see: [EIP: Point to Point Channel](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PointToPointChannel.html)

Is an InMemory Channel allowing to connect two endpoints with eachother. The channel is One-to-One connection, directly moving the return value of one endpoint
to the next endpoint.

# Errors

Common Errors and how to fix them:

| Exception | How to solve |
| :--- | :--- |
| TooManyPayloadParameters | The method used as a flow node receiver has too many "payload" parameters. A method should have only one `value` parameter, which could be either any type or a parameter of type `IMessage<>`. In addition it is possible to have an `IMessageHeaders` injected. |


# Architecture

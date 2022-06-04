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
| ErrorHandling | DONE | Exceptions are forwarded to an ErrorChannel |

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

[EIP: Messaging Channels](https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingChannelsIntro.html) are responsible to transport messages between the
different components of the Enterprise Integration Pattern. The default channel used, is an InMemoryChannel invoking other components in the same application. By replacing
such a channel with another implementations (e.g. RabbitMQ), distribution of different applications can be achieved.

### PointToPointDirectMessagingChannel
see: [EIP: Point to Point Channel](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PointToPointChannel.html)

Is an InMemory Channel allowing to connect two endpoints with eachother. The channel is One-to-One connection, directly moving the return value of one endpoint
to the next endpoint.

### RabbitMQChannel (AMQP)

Provides a simple channel implementation using [RabbitMQ](https://www.rabbitmq.com/).

Registration of RabbitMQ
```C#
using EnterpriseIntegation.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace EnterpriseIntegration.RabbitMQ.Tests;
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        IConfiguration config = configBuilder.Build();

        services
            .AddSingleton<ServiceActivatorFlow001>()
            // Enable RabbitMQ Messaging based on json config
            .WithRabbitMQMessaging(config)
            // Provide the channel "001_world" via RabbitMQ
            .WithRabbitMQChannel("001_world")
            .UseEnterpriseIntegration();
    }
}
```

The Registration of a RabbitMQChannel can be configured with additional parameters of the registration method.

# Errors

## Immediate Error Handling

If an Error/Exception happens immediately after handing a Message is pushed through the MessageGateway the Exception will be thrown to the calling Thread/Method.
As soon as the first receiver handled the message, or the message has been pushed to an external channel (Kafka, RabbitMQ...) the flow handles the exception by forwarding
it to the error channel.

## Flow Error Handling / Error Channel

If an Error/Exception happens during the flow, the FlowEngine catches the exception and forwards it to an error channel. The error channel can be defined via the 
message headers - if no error channel is defined, the default error channel is used; with the behaviour to log the exception.

## Common Errors

Common Errors and how to fix them:

| Exception | How to solve |
| :--- | :--- |
| TooManyPayloadParameters | The method used as a flow node receiver has too many "payload" parameters. A method should have only one `value` parameter, which could be either any type or a parameter of type `IMessage<>`. In addition it is possible to have an `IMessageHeaders` injected. |
| PayloadTransformation | The payload of a message did not match the parameter defined in the receiving method. Change the return type of the sending message, or change the parameter of the receiving message |

# Architecture

# Tests

## Unit Tests

## Integration Tests

## Load Tests

Load Tests are setup with [NBomber](https://nbomber.com/) to run scenarios and measure their execution time. To make the starting
and usage of the console app, the Framework [Cocona](https://github.com/mayuki/Cocona) has been used, to give a nice CLI feeling.

### Publish Load Tests

To run the Load tests the must be published

```Powershell
# generating a OS agnostic output
mkdir loadtests
cd loadtests
dotnet publish ..\tests\EnterpriseIntegration.LoadTests --output .
```

### Run Load Tests

To execute the Load tests the published artifact can be started with a scenario parameter.
```Powershell
# running load test for simple scenario
EnterpriseIntegration.LoadTests.exe --scenario Simple
```

the report is by default generated into the folder _reports_
using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace EnterpriseIntegration.TestCommon.Examples;

public class SplitterAggregatorFlow003
{
    private const string ChannelPrefix = "003-";
    private const string SplitChannelPrefix = $"{ChannelPrefix}split-";
    private const string SplitChannel1 = $"{SplitChannelPrefix}001";
    private const string SplitChannel2 = $"{SplitChannelPrefix}002";
    private const string SplitChannel3 = $"{SplitChannelPrefix}003";
    private readonly ILogger<SplitterAggregatorFlow003> _logger;

    public record Message(string RouteTo);

    public SplitterAggregatorFlow003(ILogger<SplitterAggregatorFlow003> logger)
    {
        _logger = logger;
    }

    [Splitter("003-start", "003-step")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration Service Activators must not be static")]
    public IEnumerable<Message> Start()
    {
        return new List<Message>() { new Message(SplitChannel1), new Message(SplitChannel3), new Message(SplitChannel2) };
    }

    [ServiceActivator(inChannelId: "003-step", outChannelId: EngineChannels.RouteByHeaderChannel)]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration Service Activators must not be static")]
    public Message Step(Message message, IMessageHeaders headers)
    {
        headers.RouteToChannel = message.RouteTo;
        return message;
    }

    [ServiceActivator(inChannelId: SplitChannel1, outChannelId: "003-aggregator")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration Service Activators must not be static")]
    public int SplitChannel001()
    {
        return 7;
    }

    [ServiceActivator(inChannelId: SplitChannel2, outChannelId: "003-aggregator")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration Service Activators must not be static")]
    public int SplitChannel002()
    {
        return 11;
    }

    [ServiceActivator(inChannelId: SplitChannel3, outChannelId: "003-aggregator")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enterprise Integration Service Activators must not be static")]
    public int SplitChannel003()
    {
        return 13;
    }

    [Aggregator("003-aggregator", "003-end")]
    public int Aggregate(IEnumerable<int> values)
    {
        _logger.LogInformation("Aggregate({Values})", values);
        return values.Sum();
    }

    [Endpoint(inChannelId: "003-end")]
    public void End001(int result)
    {
        _logger.LogInformation("End001({Result})", result);
    }
}
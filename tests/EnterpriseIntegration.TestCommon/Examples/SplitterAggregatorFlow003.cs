using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;

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

    [Splitter(inChannelName: "003-start", outChannelName: "003-step")]
    public IEnumerable<Message> Start()
    {
        return new List<Message>() { new Message(SplitChannel1), new Message(SplitChannel3), new Message(SplitChannel2) };
    }

    [ServiceActivator(inChannelName: "003-step", outChannelName: EngineChannels.RouteByHeaderChannel)]
    public Message Step(Message message, IMessageHeaders headers)
    {
        headers.RouteToChannel = message.RouteTo;
        return message;
    }

    [ServiceActivator(inChannelName: SplitChannel1, outChannelName: "003-aggregator")]
    public int SplitChannel001()
    {
        return 7;
    }

    [ServiceActivator(inChannelName: SplitChannel2, outChannelName: "003-aggregator")]
    public int SplitChannel002()
    {
        return 11;
    }

    [ServiceActivator(inChannelName: SplitChannel3, outChannelName: "003-aggregator")]
    public int SplitChannel003()
    {
        return 13;
    }

    [Aggregator(inChannelName: "003-aggregator", outChannelName: "003-end")]
    public int Aggregate(IEnumerable<int> values)
    {
        _logger.LogInformation("Aggregate({Values}", values);
        return values.Sum();
    }

    [Endpoint(inChannelName: "003-end")]
    public void End001(int result)
    {
        _logger.LogInformation("End001({Result}", result);
    }
}
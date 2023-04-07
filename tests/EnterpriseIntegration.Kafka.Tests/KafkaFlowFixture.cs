using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit;
using Confluent.Kafka.Admin;

namespace EnterpriseIntegration.Kafka.Tests;

[Collection("Kafka")]
public class KafkaFlowFixture
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IWireTapService _wireTapService;
    private readonly IKafkaConnectionProvider _kafkaConnectionProvider;

    private IMessageGateway Gateway { get; }

    public KafkaFlowFixture(ITestOutputHelper testOutputHelper, IMessageGateway gateway, IWireTapService wireTapService, IKafkaConnectionProvider kafkaConnectionProvider)
    {
        _testOutputHelper = testOutputHelper;
        Gateway = gateway;
        _wireTapService = wireTapService;
        _kafkaConnectionProvider = kafkaConnectionProvider;
    }

    [Fact]
    public async Task SendPayload_ShouldSendMessageToEnd()
    {
        // Arrange
        IMessage? result = null;
        // await Task.Delay(10_000);
        WireTapId id = _wireTapService.CreateWireTap("001_end", msg => Task.Run(() => result = msg));

        // Act
        await Gateway.Send("001_hello", "test:");
        await TestHelper.WaitFor(() => result != null, maxWaitTimeInMilliseconds: 20_000);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(string));
        result!.Payload.ToString().Should().Be("test: hello world");
        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }
}

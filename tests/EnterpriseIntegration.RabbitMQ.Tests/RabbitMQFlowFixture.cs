using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.RabbitMQ.Tests;

[Collection("RabbitMQ")]
public class RabbitMQFlowFixture
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IWireTapService _wireTapService;

    private IMessageGateway Gateway { get; }

    public RabbitMQFlowFixture(ITestOutputHelper testOutputHelper, IMessageGateway gateway, IWireTapService wireTapService)
    {
        _testOutputHelper = testOutputHelper;
        Gateway = gateway;
        _wireTapService = wireTapService;
    }

    [Fact]
    public async Task SendPayload_ShouldSendMessageToEnd()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("001_end", async msg => result = msg);

        // Act
        await Gateway.SendPayload("001_hello", "test:");
        await TestHelper.WaitFor(() => result != null);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(string));
        result!.Payload.ToString().Should().Be("test: hello world");
        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }
}
using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flows;

public class ServiceActivatorFlow001Fixture
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IWireTapService _wireTapService;

    private FlowEngine FlowEngine { get; }

    public ServiceActivatorFlow001Fixture(ITestOutputHelper testOutputHelper, FlowEngine flowEngine, IWireTapService wireTapService)
    {
        _testOutputHelper = testOutputHelper;
        FlowEngine = flowEngine;
        _wireTapService = wireTapService;
    }

    [Fact]
    public async Task Submit_ShouldSendMessageToEnd()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("001_end", async msg => result = msg);

        // Act
        await FlowEngine.Submit("001_hello", "test:");
        await TestHelper.WaitFor(() => result != null);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(string));
        result!.Payload.ToString().Should().Be("test: hello world");
        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }

    [Fact]
    public async Task Submit_ShouldSendMessageToEnd_WhenSubmittingAMessage()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("001_end", async msg => result = msg);
        IMessage sentMessage = new GenericMessage<string>("test:");
        string messageId = sentMessage.MessageHeaders.Id;


        // Act
        await FlowEngine.Submit("001_hello", sentMessage);
        await TestHelper.WaitFor(() => result != null);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(string));
        result!.Payload.ToString().Should().Be("test: hello world");
        result.MessageHeaders.Id.Should().Be(messageId);

        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }

    [Fact]
    public async Task Submit_ShouldSendMultipleMessageToEnd()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("001_end", async msg => result = msg);

        // Act
        for (int i = 0; i < 50; i++)
        {
            result = null;
            await FlowEngine.Submit("001_hello", $"test{i}:");
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();
            result!.PayloadType.Should().Be(typeof(string));
            result!.Payload.ToString().Should().Be($"test{i}: hello world");
            _testOutputHelper.WriteLine($"message: {result}");
        }

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }
}
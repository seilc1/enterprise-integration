using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using EnterpriseIntegration.TestCommon.Examples;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flows;

public class RoutingFlow002Fixture
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IWireTapService _wireTapService;

    private FlowEngine FlowEngine { get; }

    public RoutingFlow002Fixture(ITestOutputHelper testOutputHelper, FlowEngine flowEngine, IWireTapService wireTapService)
    {
        _testOutputHelper = testOutputHelper;
        FlowEngine = flowEngine;
        _wireTapService = wireTapService;
    }

    [Fact]
    public async Task Submit_ShouldRouteMessageToEnd1()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("002-route-001", async msg => result = msg);

        // Act
        await FlowEngine.Submit("002-start", new RoutingFlow002.Message("002-route-001"));
        await TestHelper.WaitFor(() => result != null);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(double));
        result!.Payload.Should().Be(13.37);
        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }

    [Fact]
    public async Task Submit_ShouldRouteMessageToEnd2()
    {
        // Arrange
        IMessage result = null;
        WireTapId id = _wireTapService.CreateWireTap("002-route-002", async msg => result = msg);

        // Act
        await FlowEngine.Submit("002-start", new RoutingFlow002.Message("002-route-002"));
        await TestHelper.WaitFor(() => result != null);

        // Assert
        result.Should().NotBeNull();
        result!.PayloadType.Should().Be(typeof(double));
        result!.Payload.Should().Be(13.37);
        _testOutputHelper.WriteLine($"message: {result}");

        // CleanUp
        _wireTapService.RemoveWireTap(id);
    }
}
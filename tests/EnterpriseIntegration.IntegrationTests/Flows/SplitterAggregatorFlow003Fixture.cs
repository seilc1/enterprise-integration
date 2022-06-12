using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flows
{
    public class SplitterAggregatorFlow003Fixture
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IWireTapService _wireTapService;

        private FlowEngine FlowEngine { get; }

        public SplitterAggregatorFlow003Fixture(ITestOutputHelper testOutputHelper, FlowEngine flowEngine, IWireTapService wireTapService)
        {
            _testOutputHelper = testOutputHelper;
            FlowEngine = flowEngine;
            _wireTapService = wireTapService;
        }

        [Fact]
        public async Task Submit_ShouldRouteMessageToEnd1()
        {
            // Arrange
            IMessage? result = null;
            WireTapId id = _wireTapService.CreateWireTap("003-end", msg => Task.Run(() => result = msg));

            // Act
            await FlowEngine.Submit("003-start", ExamplePayload.CreateRandom());
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();
            result!.PayloadType.Should().Be(typeof(int));
            result!.Payload.Should().Be(31);
            _testOutputHelper.WriteLine($"message: {result}");

            // CleanUp
            _wireTapService.RemoveWireTap(id);
        }
    }
}
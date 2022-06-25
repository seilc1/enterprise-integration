using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using EnterpriseIntegration.TestCommon.Examples;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flows
{
    public class FilterFlowFixture
    {
        private readonly IWireTapService _wireTapService;
        private readonly IMessageGateway _messageGateway;

        public FilterFlowFixture(IMessageGateway messageGateway, IWireTapService wireTapService)
        {
            _messageGateway = messageGateway;
            _wireTapService = wireTapService;
        }

        [Fact]
        public async Task Send_ShouldFilterMessageAndNotForwardIt()
        {
            // Arrange
            IMessage? result = null;
            WireTapId id = _wireTapService.CreateWireTap("filter-end", msg => Task.Run(() => result = msg));
            FilterFlow.FilterMessage message = new(true);

            // Act
            await _messageGateway.Send("filter", message);
            await TestHelper.WaitFor(() => result != null, maxWaitTimeInMilliseconds: 200);

            // Assert
            result.Should().BeNull("Filter removed it");

            // CleanUp
            _wireTapService.RemoveWireTap(id);
        }

        [Fact]
        public async Task Send_ShouldNotFilterMessageAndForwardIt()
        {
            // Arrange
            IMessage? result = null;
            WireTapId id = _wireTapService.CreateWireTap("filter-end", msg => Task.Run(() => result = msg));
            FilterFlow.FilterMessage message = new(false);

            // Act
            await _messageGateway.Send("filter", message);
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull("Filter did not remove it");

            // CleanUp
            _wireTapService.RemoveWireTap(id);
        }
    }
}
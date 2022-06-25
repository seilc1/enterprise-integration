using EnterpriseIntegration.Components.History;
using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Components.History
{
    public class HistoryServiceFixture
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IMessageGateway _messageGateway;
        private readonly IWireTapService _wireTapService;


        public HistoryServiceFixture(ITestOutputHelper testOutputHelper, IMessageGateway messageGateway, IWireTapService wireTapService)
        {
            _testOutputHelper = testOutputHelper;
            _messageGateway = messageGateway;
            _wireTapService = wireTapService;
        }

        [Fact]
        public async Task Send_ShouldProduceHistoryWhenActive()
        {
            // Arrange
            IMessage? result = null;
            WireTapId id = _wireTapService.CreateWireTap("001_end", msg => Task.Run(() => result = msg));

            // Act
            await _messageGateway.Send("001_hello", "test:");
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();

            result!.MessageHeaders.GetHistoryItemCount().Should().Be(3);
            IList<string> history = result.MessageHeaders.GetHistoryItems().ToList();
            history.Count.Should().Be(3);
            history[0].Should().Be("Hello");
            history[1].Should().Be("World");
            history[2].Should().Be("End");

            // CleanUp
            _wireTapService.RemoveWireTap(id);
        }
    }
}
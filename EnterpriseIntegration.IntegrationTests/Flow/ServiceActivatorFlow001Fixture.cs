using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using FluentAssertions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flow
{
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
            await WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();
            result!.PayloadType.Should().Be(typeof(string));
            result!.Payload.ToString().Should().Be("test: hello world");
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
                await WaitFor(() => result != null);

                // Assert
                result.Should().NotBeNull();
                result!.PayloadType.Should().Be(typeof(string));
                result!.Payload.ToString().Should().Be($"test{i}: hello world");
                _testOutputHelper.WriteLine($"message: {result}");
            }

            // CleanUp
            _wireTapService.RemoveWireTap(id);
        }

        public static async Task WaitFor(Func<bool> condition, int checkIntervalInMilliseconds = 10, int maxWaitTimeInMilliseconds = 1000)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (!condition() && watch.ElapsedMilliseconds < maxWaitTimeInMilliseconds)
            {
                await Task.Delay(checkIntervalInMilliseconds);
            }
        }
    }
}
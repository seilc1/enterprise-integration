using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.Tests.Examples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.Tests.Flow
{
    public class FlowEngineFixture
    {
        private readonly IMessagingChannelProvider messagingChannelProvider = new InMemoryMessagingChannelProvider();
        private readonly IFlowDataSource flowDataSource = new AttributeFlowDataSource();
        private readonly IList<IMessageProcessor> messageProcessorMocks = new List<IMessageProcessor>();

        private ILogger<FlowEngine> logger;
        private FlowEngine sut;

        public FlowEngineFixture(ITestOutputHelper testOutputHelper)
        {
            logger = new XUnitLogger<FlowEngine>(testOutputHelper);
            sut = new FlowEngine(logger, flowDataSource, messageProcessorMocks, messagingChannelProvider);
        }

        [Fact]
        public async void ShouldSendSingleMessage()
        {
            // Arrange
            var exampleFlowClass = new ExampleFlow001(logger);

            // Act
            Stopwatch watch = Stopwatch.StartNew();
            var task = sut.Submit("hello", "FLOW:");
            await task;
            watch.Stop();

            Thread.Sleep(50);

            // Assert
            logger.LogDebug($"Completed in: {watch.ElapsedMilliseconds}ms");
            exampleFlowClass.FlowsCompleted.Should().Be(1);
        }

        [Fact]
        public async void ShouldSendMultipleMessage()
        {
            // arrange
            var exampleFlowClass = new ExampleFlow001(logger);
            serviceProviderMock.GetService(exampleFlowClass.GetType()).Returns(exampleFlowClass);

            // act
            Stopwatch watch = Stopwatch.StartNew();
            var task1 = sut.Submit("hello", "FLOW1:");
            var task2 = sut.Submit("hello", "FLOW2:");
            var task3 = sut.Submit("hello", "FLOW3:");
            await Task.WhenAll(task1, task2, task3);
            watch.Stop();

            Thread.Sleep(50);

            logger.LogDebug($"Completed in: {watch.ElapsedMilliseconds}ms");
            exampleFlowClass.FlowsCompleted.Should().Be(3);
        }

        [Fact]
        public async void ShouldCompleteFlowWithDynamicParameters()
        {
            // arrange
            var exampleFlowClass = new ExampleFlow002(logger);
            serviceProviderMock.GetService(exampleFlowClass.GetType()).Returns(exampleFlowClass);

            // act
            Stopwatch watch = Stopwatch.StartNew();
            await sut.Submit("02-hello", "FLOW2:");
            watch.Stop();

            Thread.Sleep(1000);

            logger.LogDebug($"Completed in: {watch.ElapsedMilliseconds}ms");
            exampleFlowClass.FlowsCompleted.Should().Be(1);
        }


        [Fact]
        public async void ShouldUseEngineRouting()
        {
            // arrange
            var exampleFlowClass = new RoutingFlow003(logger);
            serviceProviderMock.GetService(exampleFlowClass.GetType()).Returns(exampleFlowClass);
            serviceProviderMock.GetService(sut.GetType()).Returns(sut);

            // act
            Stopwatch watch = Stopwatch.StartNew();
            await sut.Submit<object>("003-start", null);
            watch.Stop();

            Thread.Sleep(50);

            logger.LogDebug($"Completed in: {watch.ElapsedMilliseconds}ms");
            exampleFlowClass.ValueAtTheEnd.Should().Be(13.37);
        }
    }
}

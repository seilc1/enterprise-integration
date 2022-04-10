﻿using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Tests.Examples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
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
        private readonly IServiceProvider serviceProviderMock = Substitute.For<IServiceProvider>();

        private ILogger logger;
        private FlowEngine sut;

        public FlowEngineFixture(ITestOutputHelper testOutputHelper)
        {
            logger = new XUnitLogger<FlowEngineFixture>(testOutputHelper);
            sut = new FlowEngine(logger, flowDataSource, messagingChannelProvider, serviceProviderMock);
        }

        [Fact]
        public async void ShouldSendSingleMessage()
        {
            // arrange
            var exampleFlowClass = new ExampleFlow001(logger);
            serviceProviderMock.GetService(exampleFlowClass.GetType()).Returns(exampleFlowClass);

            // act
            Stopwatch watch = Stopwatch.StartNew();
            var task = sut.Submit("hello", "FLOW:");
            await task;
            watch.Stop();

            Thread.Sleep(50);

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
    }
}

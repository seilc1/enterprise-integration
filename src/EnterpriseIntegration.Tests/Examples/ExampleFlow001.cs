﻿using EnterpriseIntegration.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace EnterpriseIntegration.Tests.Examples
{
    public class ExampleFlow001
    {
        private readonly ILogger logger;

        private int flowCompletedCounter = 0;

        public int FlowsCompleted { get { return flowCompletedCounter; } }

        public ExampleFlow001(ILogger logger)
        {
            this.logger = logger;
        }

        [ServiceActivator(InChannelName = "hello", OutChannelName = "world")]
        public string Hello(string prefix)
        {
            return $"{prefix} hello";
        }

        [ServiceActivator(InChannelName = "world", OutChannelName = "random")]
        public string World(string data)
        {
            return $"{data} world";
        }

        [Router(InChannelName = "random")]
        public string Randomizer(string data)
        {
            return Random.Shared.NextInt64() % 2 == 0 ? "hello" : "end";
        }

        [Endpoint(InChannelName = "end")]
        public void End(string data)
        {
            logger.LogInformation($"{data}.");
            Interlocked.Increment(ref flowCompletedCounter);
        }
    }
}

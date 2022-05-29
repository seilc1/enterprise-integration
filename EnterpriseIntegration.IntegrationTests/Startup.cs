using EnterpriseIntegration.TestCommon.Examples;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.DependencyInjection.Logging;

namespace EnterpriseIntegration.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ServiceActivatorFlow001>()
                .AddSingleton<RoutingFlow002>()
                .AddSingleton<SplitterAggregatorFlow003>()
                .AddSingleton<ErrorFlow>()
                .UseEnterpriseIntegration();
        }

        public void Configure(IServiceProvider provider)
        {
            XunitTestOutputLoggerProvider.Register(provider);
        }
    }
}
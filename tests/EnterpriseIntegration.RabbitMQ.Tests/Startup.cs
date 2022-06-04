using EnterpriseIntegation.RabbitMQ;
using EnterpriseIntegration.TestCommon.Examples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Xunit.DependencyInjection.Logging;

namespace EnterpriseIntegration.RabbitMQ.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration config = configBuilder.Build();

            services
                .AddSingleton<ServiceActivatorFlow001>()
                .AddSingleton<RoutingFlow002>()
                .AddSingleton<SplitterAggregatorFlow003>()
                .AddSingleton<ErrorFlow>()
                .WithRabbitMQMessaging(config)
                .WithRabbitMQChannel("001_world")
                .UseEnterpriseIntegration();
        }

        public void Configure(IServiceProvider provider)
        {
            XunitTestOutputLoggerProvider.Register(provider);
        }
    }
}
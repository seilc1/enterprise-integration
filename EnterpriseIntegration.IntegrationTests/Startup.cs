using EnterpriseIntegration.Tests.Examples;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIntegration.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ServiceActivatorFlow001>()
                .AddSingleton<ExampleFlow002>()
                .AddSingleton<RoutingFlow003>()
                .UseEnterpriseIntegration();
        }
    }
}
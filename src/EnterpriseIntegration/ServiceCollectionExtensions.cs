using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Flow;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIntegration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds <see cref="FlowEngine"/> with an <see cref="InMemoryMessagingChannelProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        public static IServiceCollection AddEnterpriseIntegration(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IMessagingChannelProvider, InMemoryMessagingChannelProvider>()
                .AddSingleton<FlowEngine>();
        }
    }
}

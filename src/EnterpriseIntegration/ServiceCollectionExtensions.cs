using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Components.Aggregator;
using EnterpriseIntegration.Components.Endpoint;
using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Components.Router;
using EnterpriseIntegration.Components.ServiceActivator;
using EnterpriseIntegration.Components.Splitter;
using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Flow.MessageProcessing;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIntegration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds <see cref="FlowEngine"/> with an <see cref="InMemoryMessagingChannelProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        public static IServiceCollection UseEnterpriseIntegration(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IMessagingChannelProvider, InMemoryMessagingChannelProvider>()
                .AddSingleton<IMessageStore, InMemoryMessageStore>()
                .AddSingleton<IFlowDataSource, AttributeFlowDataSource>()
                .AddSingleton<IMessageTransformer, DefaultMessageTransformer>()
                .UseDefaultMessageProcessors()
                .UseWireTap()
                .AddSingleton<FlowEngine>()
                .AddSingleton<IMessageGateway>(x => x.GetRequiredService<FlowEngine>());
        }

        public static IServiceCollection UseDefaultMessageProcessors(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IMessageProcessor, ServiceActivatorMessageProcessor>()
                .AddSingleton<IMessageProcessor, RouterMessageProcessor>()
                .AddSingleton<IMessageProcessor, EndpointMessageProcessor>()
                .AddSingleton<IMessageProcessor, SplitterMessageProcessor>()
                .AddSingleton<IMessageProcessor, AggregatorMessageProcessor>();
        }

        public static IServiceCollection UseWireTap(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<WiretapService>()
                .AddSingleton<IWireTapService>(c => c.GetRequiredService<WiretapService>())
                .AddSingleton<IPreAction>(c => c.GetRequiredService<WiretapService>());
        }
    }
}

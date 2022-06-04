using EnterpriseIntegration.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIntegation.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection WithRabbitMQMessaging(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection
            .Configure<RabbitMQSettings>(configuration.GetRequiredSection(RabbitMQSettings.ConfigPath))
            .AddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>();
    }

    public static IServiceCollection WithRabbitMQChannel(this IServiceCollection serviceCollection, ChannelId channelId, Action<RabbitMQChannelSettings>? channelSettings = null, IMessageTransformer? messageTransformer = null)
    {
        RabbitMQChannelSettings settings = new RabbitMQChannelSettings(channelId.ToString());
        channelSettings?.Invoke(settings);

        return serviceCollection.AddSingleton<IMessagingChannel>(x => new RabbitMQChannel(
            channelId, 
            x.GetRequiredService<IRabbitMQConnectionProvider>(), 
            settings, 
            messageTransformer ?? x.GetRequiredService<IMessageTransformer>()));
    }
}
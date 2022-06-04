using EnterpriseIntegration.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseIntegation.RabbitMQ;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers RabbitMQ infrastructure to use <see cref="RabbitMQChannel"/>, to be registered with <see cref="WithRabbitMQChannel(IServiceCollection, ChannelId, Action{RabbitMQChannelSettings}?, IMessageTransformer?)"/>.
    /// </summary>
    public static IServiceCollection WithRabbitMQMessaging(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection
            .Configure<RabbitMQSettings>(configuration.GetRequiredSection(RabbitMQSettings.ConfigPath))
            .AddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>();
    }

    /// <summary>
    ///     Registers a <see cref="RabbitMQChannel"/> for a given <see cref="ChannelId"/>.
    /// </summary>
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
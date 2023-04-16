using Confluent.Kafka;
using EnterpriseIntegration.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.Kafka;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers Kafka infrastructure to use <see cref="KafkaChannel"/>, to be registered with <see cref="WithKafkaChannel(IServiceCollection, ChannelId, Action{RabbitMQChannelSettings}?, IMessageTransformer?)"/>.
    /// </summary>
    public static IServiceCollection WithKafkaMessaging(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection
            .Configure<KafkaSettings>(configuration.GetRequiredSection(KafkaSettings.ConfigPath))
            .AddSingleton<IKafkaConnectionProvider, KafkaConnectionProvider>()
            .AddSingleton<IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage>, KafkaMessageMapper>();
    }

    /// <summary>
    ///     Registers a <see cref="KafkaChannel"/> for a given <see cref="ChannelId"/>.
    /// </summary>
    public static IServiceCollection WithKafkaChannel(this IServiceCollection serviceCollection, ChannelId channelId, Action<KafkaChannelSettings>? channelSettings = null, IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage>? mapper = null)
    {
        KafkaChannelSettings settings = new() { TopicName = channelId.ToString() };
        channelSettings?.Invoke(settings);

        return serviceCollection.AddSingleton<IMessagingChannel>(x => new KafkaChannel(
            channelId,
            x.GetRequiredService<IKafkaConnectionProvider>(),
            settings,
            mapper ?? x.GetRequiredService<IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage>>(),
            x.GetRequiredService<ILogger<KafkaChannel>>()));
    }
}

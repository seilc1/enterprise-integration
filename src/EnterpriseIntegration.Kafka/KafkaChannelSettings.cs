namespace EnterpriseIntegration.Kafka;

public class KafkaChannelSettings
{
    public required string TopicName { get; set; }

    /// <summary>
    ///     Flag if the Topic should be created if it does not exist.
    ///     <para>This is only suggested for testing purposes, as a proper defined channel will work alot better</para>
    /// </summary>
    public bool EnsureCreated { get; set; }

    /// <summary>
    ///    Flag if the channel should ensure the Quality of Service (e.g. at least once delivery).
    ///    The Channel will only acknowledge the message after the subscriber has been processed.
    ///    <para>If this is set to false, the channel will auto acknowledge.</para>
    /// </summary>
    public bool EnsureQualityOfService { get; set; } = true;
}

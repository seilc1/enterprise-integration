using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels;

public interface IMessageTransformer
{
    public string Encoding { get; }

    public string ContentType { get; }

    /// <summary>
    ///     Transforms a untyped Message to a typed Message and may transform the payload.
    /// </summary>
    /// <typeparam name="T">Requested result type of the payload.</typeparam>
    /// <param name="message">An <see cref="IMessage{}"/ > with an dynamic payload type.</param>
    public IMessage<T> TransformMessage<T>(object message);

    public Task<T> Deserialize<T>(ReadOnlyMemory<byte> payload);

    public Task<T> Deserialize<T>(string payload);

    public Task<string> SerializeAsString(object payload);

    public Task<ReadOnlyMemory<byte>> SerializeAsByteArray(object payload);
}

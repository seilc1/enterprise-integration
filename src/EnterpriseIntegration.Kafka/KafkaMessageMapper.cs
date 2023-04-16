using Confluent.Kafka;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using System.Text;

namespace EnterpriseIntegration.Kafka;

public class KafkaMessageMapper : IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage>
{
    private readonly Encoding _encoding;

    private readonly IMessageTransformer _messageTransformer;

    public KafkaMessageMapper(IMessageTransformer messageTransformer, Encoding? encoding = null)
    {
        _encoding = encoding ?? Encoding.UTF8;
        _messageTransformer = messageTransformer;
    }

    public async Task<KafkaMessage> Map(IMessage message)
    {
        var serializedPayload = await _messageTransformer.SerializeAsByteArray(message.Payload);
        return new KafkaMessage
        {
            Key = message.MessageHeaders.Id,
            Value = serializedPayload.Span.ToArray(),
            Headers = TransformHeaders(message.MessageHeaders)
        };
    }

    public async Task<IMessage<T>> Map<T>(ConsumeResult<string, byte[]> result)
    {
        T payload = await _messageTransformer.Deserialize<T>(new ReadOnlyMemory<byte>(result.Message.Value));
        return new GenericMessage<T>(TransformHeaders(result.Message.Headers), payload);
    }

    private Headers TransformHeaders(IMessageHeaders originalHeaders)
    {
        var headers = new Headers();
        foreach (var header in originalHeaders)
        {
            headers.Add(header.Key, _encoding.GetBytes(header.Value));
        }
        return headers;
    }

    private IMessageHeaders TransformHeaders(Headers headers)
        => new MessageHeaders(headers.ToDictionary(it => it.Key, it => _encoding.GetString(it.GetValueBytes())));
}

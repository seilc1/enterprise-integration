using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using System.Text;
using System.Text.Json;

namespace EnterpriseIntegration.Channels
{
    public class DefaultMessageTransformer : IMessageTransformer
    {
        private readonly Encoding _encoding = System.Text.Encoding.UTF8;

        public string Encoding => _encoding.EncodingName;

        public string ContentType => "application/json";

        public IMessage<T> TransformMessage<T>(object message)
        {
            if (!message.GetType().IsMessage())
            {
                throw new EnterpriseIntegrationException($"param:{nameof(message)} must implement IMessage<T> but is of type:{message.GetType()}.");
            }

            T? payload = TransformPayload<T>(IMessage.ReflectPayload(message));
            if (payload == null)
            {
                throw new EnterpriseIntegrationException("$failed to transform out payload");
            }

            return GenericMessage<T>.From((IMessage)message, payload);
        }

        public static T? TransformPayload<T>(object payload)
        {
            if (typeof(T) == typeof(VoidParameter))
            {
                return (T)Convert.ChangeType(VoidParameter.Instance, typeof(T));
            }

            if (payload == null)
            {
                return default;
            }

            if (typeof(T) == typeof(object) || typeof(T) == payload.GetType())
            {
                return (T)payload;
            }

            try
            {
                return (T)Convert.ChangeType(payload, typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new PayloadTransformationException(payload.GetType(), typeof(T));
            }
        }

        public async Task<T> Deserialize<T>(ReadOnlyMemory<byte> payload)
        {
            using MemoryStream stream = new(payload.ToArray());
            T? result = await JsonSerializer.DeserializeAsync<T>(stream);

            if (result == null)
            {
                throw new EnterpriseIntegrationException("failed to deserialize payload.");
            }

            return result;
        }

        public async Task<T> Deserialize<T>(string payload)
            => await Deserialize<T>(new ReadOnlyMemory<byte>(_encoding.GetBytes(payload)));

        public async Task<ReadOnlyMemory<byte>> SerializeAsByteArray(object payload)
        {
            using MemoryStream outStream = new();
            await JsonSerializer.SerializeAsync(outStream, payload);
            return outStream.ToArray();
        }

        public async Task<string> SerializeAsString(object payload)
            => _encoding.GetString((await SerializeAsByteArray(payload)).Span);
    }
}

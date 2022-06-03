using RabbitMQ.Client;

namespace EnterpriseIntegation.RabbitMQ
{
    public class RabbitMQBasicProperties : IBasicProperties
    {
        public string? AppId { get; set; }
        public string? ClusterId { get; set; }
        public string? ContentEncoding { get; set; }
        public string? ContentType { get; set; }
        public string? CorrelationId { get; set; }
        public byte DeliveryMode { get; set; }
        public string? Expiration { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
        public string? MessageId { get; set; }
        public bool Persistent { get; set; }
        public byte Priority { get; set; }
        public string? ReplyTo { get; set; }
        public PublicationAddress? ReplyToAddress { get; set; }
        public AmqpTimestamp Timestamp { get; set; }
        public string? Type { get; set; }
        public string? UserId { get; set; }

        public ushort ProtocolClassId { get; set; }

        public string? ProtocolClassName { get; set; }

        public void ClearAppId()
        {
            AppId = null;
        }

        public void ClearClusterId()
        {
            ClusterId = null;
        }

        public void ClearContentEncoding()
        {
            ContentEncoding = null;
        }

        public void ClearContentType()
        {
            ContentType = null;
        }

        public void ClearCorrelationId()
        {
            CorrelationId = null;
        }

        public void ClearDeliveryMode()
        {
            DeliveryMode = 0;
        }

        public void ClearExpiration()
        {
            Expiration = null;
        }

        public void ClearHeaders()
        {
            Headers.Clear();
        }

        public void ClearMessageId()
        {
            MessageId = null;
        }

        public void ClearPriority()
        {
            Priority = 0;
        }

        public void ClearReplyTo()
        {
            ReplyTo = null;
        }

        public void ClearTimestamp()
        {
            Timestamp = new AmqpTimestamp();
        }

        public void ClearType()
        {
            Type = null;
        }

        public void ClearUserId()
        {
            UserId = null;
        }

        public bool IsAppIdPresent()
        {
            return !string.IsNullOrWhiteSpace(AppId);
        }

        public bool IsClusterIdPresent()
        {
            return !string.IsNullOrWhiteSpace(ClusterId);
        }

        public bool IsContentEncodingPresent()
        {
            return !string.IsNullOrWhiteSpace(ContentEncoding);
        }

        public bool IsContentTypePresent()
        {
            return !string.IsNullOrWhiteSpace(ContentType);
        }

        public bool IsCorrelationIdPresent()
        {
            return !string.IsNullOrWhiteSpace(CorrelationId);
        }

        public bool IsDeliveryModePresent()
        {
            return DeliveryMode > 0;
        }

        public bool IsExpirationPresent()
        {
            return !string.IsNullOrWhiteSpace(Expiration);
        }

        public bool IsHeadersPresent()
        {
            return Headers.Any();
        }

        public bool IsMessageIdPresent()
        {
            return !string.IsNullOrWhiteSpace(MessageId);
        }

        public bool IsPriorityPresent()
        {
            return Priority >= 0;
        }

        public bool IsReplyToPresent()
        {
            return !string.IsNullOrWhiteSpace(ReplyTo);
        }

        public bool IsTimestampPresent()
        {
            return true;
        }

        public bool IsTypePresent()
        {
            return !string.IsNullOrWhiteSpace(Type);
        }

        public bool IsUserIdPresent()
        {
            return !string.IsNullOrWhiteSpace(UserId);
        }
    }
}
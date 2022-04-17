namespace EnterpriseIntegration.Message
{
    public interface IMessage<T> : IMessageMetaData
    {
        /// <summary>
        /// Payload of the <see cref="IMessage{T}"/>.
        /// </summary>
        public T Payload { get; init; }

        /// <summary>
        /// Type of the <see cref="Payload"/>, necessary for serialization/deserialization.
        /// </summary>
        public Type PayloadType { get; init; }

        public static Type ReflectPayloadType(object message)
        {
            return (Type)message.GetType().GetProperty(nameof(PayloadType)).GetValue(message);
        }

        public static object ReflectPayload(object message)
        {
            return message.GetType().GetProperty(nameof(Payload)).GetValue(message);
        }
    }
}

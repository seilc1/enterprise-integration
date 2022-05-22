namespace EnterpriseIntegration.Message
{
    public interface IMessage : IMessageMetaData
    {
        /// <summary>
        /// Payload of the <see cref="IMessage"/>.
        /// </summary>
        public object Payload { get; }

        /// <summary>
        /// Type of the <see cref="Payload"/>, necessary for serialization/deserialization.
        /// </summary>
        public Type PayloadType { get; }

        public static Type ReflectPayloadType(object message)
        {
            return (Type)message.GetType().GetProperty(nameof(PayloadType))!.GetValue(message)!;
        }

        public static object ReflectPayload(object message)
        {
            return message.GetType().GetProperty(nameof(Payload))!.GetValue(message)!;
        }
    }

    public interface IMessage<T> : IMessage, IMessageMetaData
    {
        /// <summary>
        /// Payload of the <see cref="IMessage{T}"/>.
        /// </summary>
        public new T Payload { get; }
    }
}

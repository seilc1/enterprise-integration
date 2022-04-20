using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Channels
{
    
    public interface IMessageTransformer
    {
        /// <summary>
        ///     Transforms a untyped Message to a typed Message and may transform the payload.
        /// </summary>
        /// <typeparam name="T">Requested result type of the payload.</typeparam>
        /// <param name="message">An <see cref="IMessage{}"/ > with an dynamic payload type.</param>
        public IMessage<T> TransformMessage<T>(object message);
    }
}

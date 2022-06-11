using EnterpriseIntegration.Errors;

namespace EnterpriseIntegration.Channels
{
    public class PayloadTransformationException : EnterpriseIntegrationException
    {
        internal PayloadTransformationException(Type receivedType, Type methodParameterType) 
            : base($"Failed to transform payload from received type: {receivedType.Name} to method parameter type: {methodParameterType.Name}.")
        {
            ReceivedType = receivedType;
            MethodParameterType = methodParameterType;
        }
        internal PayloadTransformationException(Type receivedType, Type methodParameterType, ChannelId channelId)
            : base($"Failed to transform payload from received type:{receivedType.Name} to method parameter type:{methodParameterType.Name} for channel:{channelId}.")
        {
            ReceivedType = receivedType;
            MethodParameterType = methodParameterType;
            ChannelName = channelId;
        }

        public Type ReceivedType { get; }

        public Type MethodParameterType { get; }

        public ChannelId? ChannelName { get; }
    }
}
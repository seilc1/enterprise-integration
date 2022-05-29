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
        internal PayloadTransformationException(Type receivedType, Type methodParameterType, string channelName)
            : base($"Failed to transform payload from received type:{receivedType.Name} to method parameter type:{methodParameterType.Name} for channel:{channelName}.")
        {
            ReceivedType = receivedType;
            MethodParameterType = methodParameterType;
            ChannelName = channelName;
        }

        public Type ReceivedType { get; }

        public Type MethodParameterType { get; }

        public string? ChannelName { get; }
    }
}
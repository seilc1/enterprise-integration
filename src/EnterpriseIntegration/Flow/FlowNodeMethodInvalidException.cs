using EnterpriseIntegration.Errors;

namespace EnterpriseIntegration.Flow
{
    public class FlowNodeMethodInvalidException : EnterpriseIntegrationException
    {
        public FlowNodeMethodInvalidException(string message) : base(message)
        {
        }

        public static FlowNodeMethodInvalidException TooManyPayloadParameters(FlowNode flowNode)
        {
            return new FlowNodeMethodInvalidException($"TooManyPayloadParameters: FlowNode:{flowNode.Name} with bound method:{flowNode.MethodInfo.DeclaringType?.Name}.{flowNode.MethodInfo.Name} has too many payload parameters defined.");
        }
    }
}

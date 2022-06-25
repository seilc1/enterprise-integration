using EnterpriseIntegration.Message;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public static class FlowEngineResolver
    {
        private static readonly Type IMessageBaseType = typeof(IMessage<>).GetGenericTypeDefinition();

        public static Type ExpectedPayloadType(FlowNode flowNode)
        {
            Type type = ExpectedPayloadTypeForNoneRouter(flowNode);

            if (flowNode.NodeType == FlowNodeType.Router && type == typeof(VoidParameter))
            {
                type = typeof(object);
            }

            if (flowNode.NodeType == FlowNodeType.Aggregator && type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            return type;
        }

        private static Type ExpectedPayloadTypeForNoneRouter(FlowNode flowNode)
        {
            Type? result = null;

            foreach (ParameterInfo parameterInfo in flowNode.MethodInfo.GetParameters())
            {
                if (parameterInfo.ParameterType.IsAssignableTo(typeof(IMessageHeaders)))
                {
                    continue;
                }
                if (result != null)
                {
                    throw FlowNodeMethodInvalidException.TooManyPayloadParameters(flowNode);
                }

                result = IsMessageParameter(parameterInfo) ? parameterInfo.ParameterType.GetGenericArguments()[0] : parameterInfo.ParameterType;
            }

            return result ?? typeof(VoidParameter);
        }

        public static bool IsMessageParameter(ParameterInfo parameterInfo)
        {
            if (!parameterInfo.ParameterType.IsGenericType)
            {
                return false;
            }

            var genericMessage = IMessageBaseType.MakeGenericType(new Type[] { parameterInfo.ParameterType.GetGenericArguments()[0] });
            return parameterInfo.ParameterType.IsAssignableTo(genericMessage);
        }
    }
}

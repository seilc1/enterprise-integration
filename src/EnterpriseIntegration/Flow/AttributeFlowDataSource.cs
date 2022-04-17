using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Errors;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public class AttributeFlowDataSource : IFlowDataSource
    {
        private static readonly Type ROOT_TYPE = typeof(EnterpriseIntegrationAttribute);

        public IEnumerable<MethodInfo> GetAllMethodsWithIntegrationAttribute()
        {
            return GetAssemblies().SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(ROOT_TYPE, true).Length > 0);
        }

        public IEnumerable<FlowNode> GetAllFlowNodes()
        {
            return GetAllMethodsWithIntegrationAttribute().Select(method => ToFlowNode(method));
        }

        private static FlowNode ToFlowNode(MethodInfo methodInfo)
        {
            var integrationAttribute = methodInfo.GetCustomAttributes().Single(a => a.GetType().IsSubclassOf(ROOT_TYPE));

            switch (integrationAttribute)
            {
                case ServiceActivatorAttribute attr:
                    return new FlowNode()
                    {
                        Name = methodInfo.Name,
                        NodeType = FlowNodeType.Method,
                        InChannelName = attr.InChannelName,
                        OutChannelName = attr.OutChannelName,
                        MethodInfo = methodInfo
                    };
                case EndpointAttribute attr:
                    return new FlowNode()
                    {
                        Name = methodInfo.Name,
                        NodeType = FlowNodeType.Terminator,
                        InChannelName = attr.InChannelName,
                        MethodInfo = methodInfo
                    };
                case RouterAttribute attr:
                    return new FlowNode()
                    {
                        Name = methodInfo.Name,
                        NodeType = FlowNodeType.Router,
                        InChannelName = attr.InChannelName,
                        MethodInfo = methodInfo
                    };
                default:
                    throw new EnterpriseIntegrationException($"Unexpected attribute of type: {integrationAttribute.GetType()}");
            }
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }
    }
}

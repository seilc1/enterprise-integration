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
            EnterpriseIntegrationAttribute integrationAttribute = (EnterpriseIntegrationAttribute)methodInfo.GetCustomAttributes().Single(a => a.GetType().IsSubclassOf(ROOT_TYPE));
            FlowNode baseNode = new FlowNode(methodInfo.Name, FlowNodeType.Undefined, integrationAttribute.InChannelName, methodInfo, integrationAttribute);

            switch (integrationAttribute)
            {
                case SplitterAttribute attr:
                    return baseNode with
                    { 
                        NodeType = FlowNodeType.Splitter,
                        OutChannelName = attr.OutChannelName
                    };
                case AggregatorAttribute attr:
                    return baseNode with
                    {
                        NodeType = FlowNodeType.Aggregator,
                        OutChannelName = attr.OutChannelName
                    };
                case ServiceActivatorAttribute attr:
                    return baseNode with
                    {
                        NodeType = FlowNodeType.ServiceActivator,
                        OutChannelName = attr.OutChannelName
                    };
                case EndpointAttribute attr:
                    return baseNode with { NodeType = FlowNodeType.Endpoint };
                case RouterAttribute attr:
                    return baseNode with { NodeType = FlowNodeType.Router };
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

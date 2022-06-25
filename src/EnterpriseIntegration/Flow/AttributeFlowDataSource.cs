using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Components.Filter;
using EnterpriseIntegration.Errors;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EnterpriseIntegration.Flow
{
    public class AttributeFlowDataSource : IFlowDataSource
    {
        private static readonly Type ROOT_TYPE = typeof(EnterpriseIntegrationAttribute);
        private readonly ILogger<AttributeFlowDataSource> _logger;

        public AttributeFlowDataSource(ILogger<AttributeFlowDataSource> logger)
        {
            _logger = logger;
        }

        public static IEnumerable<MethodInfo> GetAllMethodsWithIntegrationAttribute()
        {
            return GetAssemblies().SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(ROOT_TYPE, true).Length > 0);
        }

        public IEnumerable<FlowNode> GetAllFlowNodes()
        {
            return GetAllMethodsWithIntegrationAttribute()
                .Select(ExtractAttribute)
                .Where(IsValidAttributeAssignment)
                .Select(t => ToFlowNode(t.Item1, t.Item2));
        }

        private static FlowNode ToFlowNode(MethodInfo methodInfo, EnterpriseIntegrationAttribute integrationAttribute)
        {
            FlowNode baseNode = new(methodInfo.Name, FlowNodeType.Undefined, integrationAttribute.InChannelId, methodInfo, integrationAttribute);

            return integrationAttribute switch
            {
                SplitterAttribute attr          => baseNode with { NodeType = FlowNodeType.Splitter,            OutChannelId = attr.OutChannelId },
                FilterAttribute attr            => baseNode with { NodeType = FlowNodeType.Filter,              OutChannelId = attr.OutChannelId },
                AggregatorAttribute attr        => baseNode with { NodeType = FlowNodeType.Aggregator,          OutChannelId = attr.OutChannelId },
                ServiceActivatorAttribute attr  => baseNode with { NodeType = FlowNodeType.ServiceActivator,    OutChannelId = attr.OutChannelId },
                EndpointAttribute               => baseNode with { NodeType = FlowNodeType.Endpoint },
                RouterAttribute                 => baseNode with { NodeType = FlowNodeType.Router },
                _                               => throw new EnterpriseIntegrationException($"Unexpected attribute of type: {integrationAttribute.GetType()}"),
            };
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }

        private static Tuple<MethodInfo, EnterpriseIntegrationAttribute> ExtractAttribute(MethodInfo methodInfo)
        {
            return Tuple.Create(methodInfo, (EnterpriseIntegrationAttribute)methodInfo.GetCustomAttributes().Single(a => a.GetType().IsSubclassOf(ROOT_TYPE)));
        }

        private bool IsValidAttributeAssignment(Tuple<MethodInfo, EnterpriseIntegrationAttribute> methodWithAttribute)
        {
            if (!methodWithAttribute.Item2.IsValid(methodWithAttribute.Item1))
            {
                _logger.LogWarning("Method:{Class}.{Method} has Attribute:{} which is not valid.", 
                    methodWithAttribute.Item1.DeclaringType?.Name,
                    methodWithAttribute.Item1.Name,
                    methodWithAttribute.Item2.GetType().Name);
                return false;
            }

            return true;
        }
    }
}

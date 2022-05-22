using EnterpriseIntegration.Attributes;
using System.Reflection;

namespace EnterpriseIntegration.Flow;
public record FlowNode(string Name, FlowNodeType NodeType, string InChannelName, MethodInfo MethodInfo, EnterpriseIntegrationAttribute Attribute)
{
    public string Name { get; init; }

    public FlowNodeType NodeType { get; init; }

    public string InChannelName { get; init; }

    public MethodInfo MethodInfo { get; init; }

    public EnterpriseIntegrationAttribute Attribute { get; init; }

    public string? OutChannelName { get; init; }
}

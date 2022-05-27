using EnterpriseIntegration.Attributes;
using System.Reflection;

namespace EnterpriseIntegration.Flow;
public record FlowNode(string Name, FlowNodeType NodeType, string InChannelName, MethodInfo MethodInfo, EnterpriseIntegrationAttribute Attribute)
{
    public string Name { get; init; } = Name;

    public FlowNodeType NodeType { get; init; } = NodeType;

    public string InChannelName { get; init; } = InChannelName;

    public MethodInfo MethodInfo { get; init; } = MethodInfo;

    public EnterpriseIntegrationAttribute Attribute { get; init; } = Attribute;

    public string? OutChannelName { get; init; }
}

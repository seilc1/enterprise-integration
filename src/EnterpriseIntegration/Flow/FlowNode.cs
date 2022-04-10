using System.Reflection;

namespace EnterpriseIntegration.Flow;
public record FlowNode
{
    public string Name { get; init; }

    public FlowNodeType NodeType { get; init; }

    public string InChannelName { get; init; }

    public string? OutChannelName { get; init; }

    public MethodInfo MethodInfo { get; init; }
}

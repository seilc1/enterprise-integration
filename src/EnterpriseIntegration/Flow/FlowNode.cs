using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Channels;
using System.Reflection;

namespace EnterpriseIntegration.Flow;
public record FlowNode(string Name, FlowNodeType NodeType, ChannelId InChannelId, MethodInfo MethodInfo, EnterpriseIntegrationAttribute Attribute)
{
    public string Name { get; init; } = Name;

    public FlowNodeType NodeType { get; init; } = NodeType;

    public ChannelId InChannelId { get; init; } = InChannelId;

    public MethodInfo MethodInfo { get; init; } = MethodInfo;

    public EnterpriseIntegrationAttribute Attribute { get; init; } = Attribute;

    public ChannelId? OutChannelId { get; init; }
}

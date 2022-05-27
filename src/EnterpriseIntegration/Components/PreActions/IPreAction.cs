using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.PreActions;

public interface IPreAction
{
    public Task PreProcess(FlowNode flowNode, IMessage message);
}
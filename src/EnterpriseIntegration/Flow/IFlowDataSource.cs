
namespace EnterpriseIntegration.Flow
{
    public interface IFlowDataSource
    {
        IEnumerable<FlowNode> GetAllFlowNodes();
    }
}
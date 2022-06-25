namespace EnterpriseIntegration.Flow
{
    public enum FlowNodeType : byte
    {
        Undefined = 0,
        ServiceActivator = 1,
        Router = 2,
        Splitter = 3,
        Aggregator = 4,
        Filter = 5,
        Endpoint = byte.MaxValue,
    }
}

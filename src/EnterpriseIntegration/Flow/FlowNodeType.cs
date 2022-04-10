namespace EnterpriseIntegration.Flow
{
    public enum FlowNodeType : byte
    {
        Undefined = 0,
        Method = 1,
        Router = 2,
        Terminator = byte.MaxValue,
    }
}

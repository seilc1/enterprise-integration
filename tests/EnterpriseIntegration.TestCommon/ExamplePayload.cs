namespace EnterpriseIntegration.TestCommon;
public record ExamplePayload(string Name, double Value)
{
    public static ExamplePayload CreateRandom() => new ExamplePayload(Guid.NewGuid().ToString(), Random.Shared.NextDouble());
}
namespace EnterpriseIntegration.Errors
{
    public class EnterpriseIntegrationException : InvalidOperationException
    {
        public EnterpriseIntegrationException(string message) : base(message)
        {
        }
    }
}

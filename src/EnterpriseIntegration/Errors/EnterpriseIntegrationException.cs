namespace EnterpriseIntegration.Errors
{
    public class EnterpriseIntegrationException : InvalidOperationException
    {
        public EnterpriseIntegrationException(string message) : base(message)
        {
        }

        public EnterpriseIntegrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

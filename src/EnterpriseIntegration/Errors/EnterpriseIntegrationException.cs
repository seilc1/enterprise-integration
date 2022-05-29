namespace EnterpriseIntegration.Errors
{
    public class EnterpriseIntegrationException : InvalidOperationException
    {
        internal EnterpriseIntegrationException(string message) : base(message)
        {
        }

        internal EnterpriseIntegrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

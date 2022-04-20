namespace EnterpriseIntegration.Message;
public record MessageFailure(object originalPayload, Exception exception);
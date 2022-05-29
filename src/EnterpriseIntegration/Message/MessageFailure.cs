namespace EnterpriseIntegration.Message;
public record MessageFailure(IMessage OriginalMessage, Exception Exception);
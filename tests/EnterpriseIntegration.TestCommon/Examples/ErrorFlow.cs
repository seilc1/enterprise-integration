using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.TestCommon.Examples
{
    public class ErrorFlow
    {
        public const string ErrorChannelName = "error_channel";

        private readonly ILogger<ErrorFlow> logger;

        public MessageFailure? LastMessageFailure { get; private set; }

        public void ResetLastMessageFailure()
        {
            LastMessageFailure = null;
        }

        public ErrorFlow(ILogger<ErrorFlow> logger)
        {
            this.logger = logger;
        }

        [Endpoint("error_exception")]
        public void Fail(ExamplePayload payload)
        {
            throw new ArgumentException("Fails because of an Argument");
        }

        [ServiceActivator("error_with_type", "error_exception")]
        public double FailWithType(ExamplePayload payload)
        {
            return payload.Value;
        }

        [Endpoint(ErrorChannelName)]
        public void ErrorChannel(MessageFailure messageFailure)
        {
            logger.LogWarning(messageFailure.Exception, "Received Message Failure for Message:{Message}", messageFailure.OriginalMessage);
            LastMessageFailure = messageFailure;
        }

        [Endpoint("route_error_channel")]
        public void RouteErrorChannel(MessageFailure messageFailure)
        {
            logger.LogWarning(messageFailure.Exception, "Received Message Failure for Message:{Message}", messageFailure.OriginalMessage);
            LastMessageFailure = messageFailure;
        }
    }
}
using EnterpriseIntegration.ChannelAttributes;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace EnterpriseIntegration.Tests.Examples
{
    public class ExampleFlow002
    {
        private const string LoopCounter = "02-loop-counter";
        private readonly ILogger logger;

        private int flowCompletedCounter = 0;

        public int FlowsCompleted { get { return flowCompletedCounter; } }

        public ExampleFlow002(ILogger logger)
        {
            this.logger = logger;
        }

        [MessageChannel(InChannelName = "02-hello", OutChannelName = "02-world")]
        public string Hello002(string prefix)
        {
            return $"{prefix} hello";
        }

        [MessageChannel(InChannelName = "02-world", OutChannelName = "02-loop")]
        public string World002(IMessage<string> message)
        {
            return $"{message.Payload} world";
        }

        [MessageRouter(InChannelName = "02-loop")]
        public string Loop002(IMessageHeaders messageHeaders)
        {
            if (messageHeaders.ContainsKey(LoopCounter))
            {
                int value = int.Parse(messageHeaders[LoopCounter]);

                if (value < 2)
                {
                    messageHeaders[LoopCounter] = $"{++value}";
                    return "02-hello";
                }

                return "02-end";
            }

            messageHeaders[LoopCounter] = "1";
            return "02-hello";
        }

        [MessageTerminator(InChannelName = "02-end")]
        public void End002(IMessage<string> message, IMessageHeaders headers)
        {
            logger.LogInformation($"{message.Payload} for message:{message.MessageHeaders.Id} after {headers[LoopCounter]} loops.");
            Interlocked.Increment(ref flowCompletedCounter);
        }
    }
}

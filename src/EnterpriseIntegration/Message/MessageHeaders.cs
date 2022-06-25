using System.Globalization;
using System.Text;

namespace EnterpriseIntegration.Message
{
    public class MessageHeaders : Dictionary<string, string>, IMessageHeaders
    {
        public MessageHeaders()
        {
            this[HeaderFields.MessageId] = Guid.NewGuid().ToString();
            this[HeaderFields.MessageCreateDate] = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        }

        public MessageHeaders(IDictionary<string, string> data)
        {
            foreach (var entry in data)
            {
                this[entry.Key] = entry.Value;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{ ");
            this.ToList().ForEach(e => stringBuilder.Append($"{{ Key = {e.Key}, Value = {e.Value} }}, "));
            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }

        public static MessageHeaders CopyFrom(IMessageHeaders messageHeaders) => new MessageHeaders(messageHeaders);
    }
}

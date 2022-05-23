using System.Text;

namespace EnterpriseIntegration.Message
{
    public class MessageHeaders : Dictionary<string, string>, IMessageHeaders
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{ ");
            this.ToList().ForEach(e => stringBuilder.Append($"{{ Key = {e.Key}, Value = {e.Value} }}, "));
            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }
    }
}

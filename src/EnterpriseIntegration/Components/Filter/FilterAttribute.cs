using EnterpriseIntegration.Attributes;
using System.Reflection;

namespace EnterpriseIntegration.Components.Filter
{
    /// <summary>
    ///     Method receives a <see cref="Message.IMessage"/> and returns a flag if the message should be
    ///     forwarded to the next <see cref="Channels.IMessagingChannel"/>.
    /// </summary>
    /// <remarks>
    ///     The Method must return a boolean, indicating if the message should be forwarded.
    ///     Result == true -> forward
    ///     Result == false -> filter
    /// </remarks>
    public class FilterAttribute : ServiceActivatorAttribute
    {
        public FilterAttribute(string inChannelId, string outChannelId) : base(inChannelId, outChannelId) { }

        public override bool IsValid(MethodInfo method)
        {
            if (method.ReturnType != typeof(FilterResult))
            {
                return false;
            }

            return base.IsValid(method);
        }
    }
}
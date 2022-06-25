using EnterpriseIntegration.Channels;
using System.Reflection;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Annotates an Method to be used in the enterprise integration implementation, for message handling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class EnterpriseIntegrationAttribute : Attribute
    {
        protected EnterpriseIntegrationAttribute(ChannelId inChannelId)
        {
            InChannelId = inChannelId;
        }

        /// <summary>
        ///     Id of the channel, where messages are received from for this Method.
        /// </summary>
        public ChannelId InChannelId { get; }

        /// <summary>
        ///     Validates that the method, this <see cref="Attribute"/> is applied to, is valid.
        /// </summary>
        public virtual bool IsValid(MethodInfo method)
        {
            return true;
        }
    }
}

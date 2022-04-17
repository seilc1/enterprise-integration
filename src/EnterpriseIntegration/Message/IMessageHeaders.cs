using System.Globalization;

namespace EnterpriseIntegration.Message
{
    public interface IMessageHeaders : IDictionary<string, string>
    {
        /// <summary>
        ///     Id of the <see cref="IMessage{T}"/>, which remains the same during the complete flow.
        /// </summary>
        public string Id
        {
            get { return this[HeaderFields.MessageId]; }
            set { this[HeaderFields.MessageId] = value; }
        }

        /// <summary>
        ///     <see cref="DateTime"/> when the <see cref="IMessage{T}"/> was initially created.
        /// </summary>
        public DateTime CreatedDate
        {
            get { return DateTime.ParseExact(this[HeaderFields.MessageCreateDate], "o", CultureInfo.InvariantCulture); }
            set { this[HeaderFields.MessageCreateDate] = value.ToString("o", CultureInfo.InvariantCulture); }
        }

        /// <summary>
        ///     Name of the <see cref="Channels.IMessagingChannel"/> to which the message should be forwarded, if an exception in the flow happens.
        /// </summary>
        public string? ErrorChannel
        {
            get { return ContainsKey(HeaderFields.ErrorChannel) ? this[HeaderFields.ErrorChannel] : null; }
            set { 
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (ContainsKey(HeaderFields.ErrorChannel))
                    {
                        Remove(HeaderFields.ErrorChannel);
                    }
                }
                else
                {
                    this[HeaderFields.ErrorChannel] = value;
                }
            }
        }
    }
}

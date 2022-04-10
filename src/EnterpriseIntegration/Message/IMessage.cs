using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseIntegration.Message
{
    public interface IMessage<T> : IMessageMetaData
    {
        /// <summary>
        /// Payload of the <see cref="IMessage{T}"/>.
        /// </summary>
        public T? Payload { get; }
    }
}

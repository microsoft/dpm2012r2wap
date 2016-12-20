using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient
{
    public class JsonException : Exception
    {
        /// <summary>
        /// Gets Http Status code
        /// </summary>        
        public System.Net.HttpStatusCode httpStatusCode { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="JsonException"/> class.
        /// Initialized a JsonException with a message, using the default HttpStatusCode of InternalServerError</summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        public JsonException(string message)
            : base(message)
        {
            this.httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonException"/> class.
        /// Initialized a JsonException with a message, and a user specificed HTTP status code.</summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="httpStatusCode">The HTTP Status code t reutrn to the caller</param>
        public JsonException(string message, System.Net.HttpStatusCode httpStatusCode)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonException"/> class.
        /// Initialized a JsonException with a message, and an internal exception (that is logged). Uses the default HttpStatusCode of InternalServerError. </summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="innerException">The inner exception, usefull for logging scenario's</param>
        public JsonException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.httpStatusCode = System.Net.HttpStatusCode.BadRequest;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonException"/> class.
        /// Initialized a JsonException with a message, exception and a caller specificed Http Status Code</summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="innerException">The inner exception, usefull for logging scenario's</param>
        /// <param name="httpStatusCode">The HTTP Status code t reutrn to the caller</param>
        public JsonException(string message, Exception innerException, System.Net.HttpStatusCode httpStatusCode)
            : base(message, innerException)
        {
            this.httpStatusCode = httpStatusCode;
        }
    }
}

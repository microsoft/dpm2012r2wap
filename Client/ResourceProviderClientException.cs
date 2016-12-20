//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient
{
    /// <summary>
    /// Resource Provider Client Library Exception
    /// </summary>
    public class ResourceProviderClientException : Exception
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the error severity.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProviderClientException"/> class.
        /// </summary>
        public ResourceProviderClientException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProviderClientException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ResourceProviderClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProviderClientException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResourceProviderClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProviderClientException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ResourceProviderClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

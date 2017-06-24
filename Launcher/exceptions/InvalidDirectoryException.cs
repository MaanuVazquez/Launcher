using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.exceptions {
    class InvalidDirectoryException : Exception {
        /// <summary>
        /// Just create the exception
        /// </summary>
        public InvalidDirectoryException(): base() {

        }

        /// <summary>
        /// Create the exception with description
        /// </summary>
        /// <param name="message">Exception description</param>
        public InvalidDirectoryException(String message) : base(message) {

        }

        /// <summary>
        /// Create the exception with description and inner cause
        /// </summary>
        /// <param name="message">Exception description</param>
        /// <param name="innerException">Exception inner cause</param>
        public InvalidDirectoryException(String message, Exception innerException) : base(message, innerException) {

        }

        /// <summary>
        /// Create the exception from serialized data.
        /// Usual scenario is when exception is occured somewhere on the remote workstation
        /// and we have to re-create/re-throw the exception on the local machine
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected InvalidDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context) {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TE.Apps.Staging
{
    public class FilesNotEqualException : Exception
    {
        /// <summary>
        /// The message associated with the exception.
        /// </summary>
        private string _message;

        /// <summary>
        /// Gets the message associated with the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Initializes an instance of the <see cref="FilesNotEqualException"/>
        /// exception.
        /// </summary>
        public FilesNotEqualException() { }

        /// <summary>
        /// Initializes an instance of the <see cref="FilesNotEqualException"/>
        /// exception when provided with the exception message.
        /// </summary>
        /// <param name="message">
        /// The message associated with the exception.
        /// </param>
        public FilesNotEqualException(string message) : base(message) { }


        /// <summary>
        /// Initializes an instance of the <see cref="FilesNotEqualException"/>
        /// exception when provided with source and destination file paths and
        /// hashes.
        /// </summary>
        /// <param name="sourcePath">
        /// The source file path.
        /// </param>
        /// <param name="sourceHash">
        /// The source file hash.
        /// </param>
        /// <param name="destinationPath">
        /// The destination file path.
        /// </param>
        /// <param name="destinationHash">
        /// The destination file hash.
        /// </param>
        public FilesNotEqualException(
            string sourcePath,
            string sourceHash,
            string destinationPath,
            string destinationHash)
        {
            _message = 
                $"Hashes don't match: {sourcePath}: {sourceHash}, {destinationPath}: {destinationHash}.";
        }

        /// <summary>
        /// Initializes an instance of the <see cref="FilesNotEqualException"/>
        /// exception when provided with the exception message and the inner
        /// exception.
        /// </summary>
        /// <param name="message">
        /// The message associated with the exception.
        /// </param>
        /// <param name="innerException">
        /// The inner exception associated with this exception.
        /// </param>
        public FilesNotEqualException(
            string message,
            Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes an instance of the <see cref="FilesNotEqualException"/>
        /// exception when provided with serialization information and the
        /// context.
        /// </summary>
        /// <param name="info">
        /// The serialization information.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        protected FilesNotEqualException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}

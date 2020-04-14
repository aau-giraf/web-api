using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.Responses
{
    /// <summary>
    /// Represents an error in the GIRAF backend
    /// </summary>
    public class RESTError
    {
        /// <summary>
        /// Represents an "empty" error. This is used for non-error responses.
        /// </summary>
        public static RESTError EMPTY = new RESTError(ErrorCode.NoError, "", "");

        /// <summary> GIRAF error code </summary>
        [JsonIgnore]
        public ErrorCode ErrorCode;

        /// <summary> Error message </summary>
        public string Message;

        /// <summary> Error details </summary>
        public string Details;

        [EnumDataType(typeof(ErrorCode))]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCode ErrorKey
        {
            get
            {
                return ErrorCode;
            }
        }

        /// <summary>
        /// Creates a new REST error object
        /// </summary>
        /// <param name="errorCode">GIRAF error code</param>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public RESTError(ErrorCode errorCode, string message, string details)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Details = details;
        }
    }
}
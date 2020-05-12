using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.Responses
{
    /// <summary>
    /// Represents an error in the GIRAF backend
    /// </summary>
    public class ErrorResponse
    {
        /// <summary> GIRAF error code </summary>
        [JsonIgnore]
        public ErrorCode ErrorCode;

        /// <summary> Error message </summary>
        public string Message;

        /// <summary> Error details </summary>
        public string Details;

        /// <summary>
        /// GIRAF Error code, this property is used by the serializer
        /// </summary>
        /// <value></value>
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
        public ErrorResponse(ErrorCode errorCode, string message, string details)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Details = details;
        }

        /// <summary>
        /// Creates a new REST error object without details
        /// </summary>
        /// <param name="errorCode">GIRAF error code</param>
        /// <param name="message">Error message</param>
        public ErrorResponse(ErrorCode errorCode, string message)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Details = "";
        }

    }


}
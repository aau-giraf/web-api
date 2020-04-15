using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.Responses
{
    /// <summary>
    /// This class represents a REST Response.
    /// This class should only be used if you know what you are doing, use one of: 
    /// <see cref="RESTSuccessResponse" />, <see cref="RESTErrorResponse" />, <see cref="RESTFailResponse" />
    /// </summary>
    /// <typeparam name="TData">The type of the <c>data</c> object</typeparam>
    public class RESTResponse<TData>
    {
        /// <summary> The HTTP status code </summary>
        /// <seealso>https://en.wikipedia.org/wiki/List_of_HTTP_status_codes</seealso>
        public int Code;

        /// <summary> The status of the response, see available responses in <see cref="RESTResponseStatus" /> </summary>
        [JsonIgnore]
        public RESTResponseStatus ResponseStatus;

        /// <summary> Contains the response body. In the case of "error" or "fail" statuses, it should contain the course or exception name </summary>
        public TData Data;

        public RESTError Error = null;

        /// <summary>
        /// Used by the json converter to convert the enums to strings
        /// </summary>
        [EnumDataType(typeof(RESTResponseStatus))]
        public RESTResponseStatus Status
        {
            get
            {
                return ResponseStatus;
            }
        }

        /// <summary>
        /// Create a new REST response object.
        /// </summary>
        /// <param name="statusCode">The http status code of the response</param>
        /// <param name="status">The status of the response, see available responses in <see cref="RESTResponseStatus" /></param>
        /// <param name="error">The error of a non-successful response</param>
        /// <param name="data">Contains the response body. </param>
        public RESTResponse(int statusCode, RESTResponseStatus status, TData data, RESTError error)
        {
            this.Code = statusCode;
            this.ResponseStatus = status;
            this.Data = data;
            this.Error = error;
        }

        public RESTResponse()
        {
        }
    }

    /// <summary>
    /// This class represents a successful REST response
    /// </summary>
    /// <typeparam name="TData">The type of the <c>data</c> object</typeparam>
    public class RESTSuccessResponse<TData> : RESTResponse<TData> where TData : class
    {
        /// <summary>
        /// Creates a new RESTSuccessResponse, used for successful responses.
        /// Automatically sets the RESTResponseStatus to "success"
        /// </summary>
        /// <param name="data">The response body</param>
        /// <param name="statusCode">The HTTP status code, by default this is <c>200</c> </param>
        public RESTSuccessResponse(int statusCode, TData data) : base(statusCode, RESTResponseStatus.Success, data, null)
        { }

        /// <summary>
        /// Creates a new RESTSuccessResponse, used for successful responses.
        /// Automatically sets the RESTResponseStatus to "success".
        /// Sets the http status code to 200
        /// </summary>
        /// <param name="data">The response body</param>
        public RESTSuccessResponse(TData data) : base(200, RESTResponseStatus.Success, data, null)
        { }
    }

    /// <summary>
    /// This class represents an error REST response. Use this class for HTTP status codes from 400-499
    /// </summary>
    public class RESTErrorResponse : RESTResponse<string>
    {
        /// <summary>
        /// Creates a new RESTErrorResponse, used for error responses.
        /// Automatically sets the RESTResponseStatus to "error"
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="error">The error</param>
        /// <returns></returns>
        public RESTErrorResponse(int statusCode, RESTError error) : base(statusCode, RESTResponseStatus.Error, "", error)
        { }
    }

    /// <summary>
    /// This class represents a fail REST response. Use this class for HTTP status codes from 500-599
    /// </summary>
    public class RESTFailResponse : RESTResponse<string>
    {
        /// <summary>
        /// Creates a new RESTFailResponse, used for fail responses.
        /// Automatically sets the RESTResponseStatus to "fail"
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="error">The error</param>
        /// <returns></returns>
        public RESTFailResponse(int statusCode, RESTError error) : base(statusCode, RESTResponseStatus.Fail, "", error)
        { }
    }
}
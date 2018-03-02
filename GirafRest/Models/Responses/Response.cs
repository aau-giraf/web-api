using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.Responses
{
    public class Response
    {
        public Response()
        {
            Success = true;
            ErrorCode = ErrorCode.NoError;
            ErrorProperties = new string[0];
        }

        public bool Success { get; set; }
        [JsonIgnore]
        public ErrorCode ErrorCode { get; set; }
        public string[] ErrorProperties { get; set; }

        [EnumDataType(typeof(ErrorCode))]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCode ErrorKey
        {
            get
            {
                return ErrorCode;
            }
        }
    }

    public class Response<TData> : Response where TData : class
    {
        public Response(TData data, params string[] missingProperties)
        {
            Data = data;
            ErrorProperties = missingProperties;
        }

        public TData Data { get; set; }
    }
}


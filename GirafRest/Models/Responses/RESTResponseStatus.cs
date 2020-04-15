using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GirafRest.Models.Responses
{
    /// <summary>
    /// Rest response status enum
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RESTResponseStatus
    {
        [EnumMember(Value = "success")]
        Success = 1,
        [EnumMember(Value = "error")]
        Error = 2,
        [EnumMember(Value = "fail")]
        Fail = 3,
    }
}

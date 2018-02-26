﻿namespace GirafRest.Models.Responses
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
        public ErrorCode ErrorCode { get; set; }
        public string[] ErrorProperties { get; set; }

        public string ErrorKey
        {
            get
            {
                return ErrorCode.ToString();
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

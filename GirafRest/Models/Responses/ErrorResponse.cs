namespace GirafRest.Models.Responses
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(ErrorCode errorCode, params string[] missingProperties)
        {
            ErrorCode = errorCode;
            Success = false;
            ErrorProperties = missingProperties;
        }
    }

    public class ErrorResponse<TData> : Response<TData> where TData : class
    {
        public ErrorResponse(ErrorCode errorCode, params string[] properties) : base(null)
        {
            ErrorCode = errorCode;
            Success = false;
            ErrorProperties = properties;
        }
    }
}
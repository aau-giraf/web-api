namespace GirafRest.Models.Responses
{
    /// <summary>
    /// Used for a successful REST response.
    /// Holds its data in the <see cref="Data"/> field
    /// </summary>
    /// <typeparam name="TData">The type of data to hold</typeparam>
    public class SuccessResponse<TData>
    {
        /// <summary>
        /// The data of the response
        /// </summary>
        public TData Data;

        /// <summary>
        /// Create a new SuccessResponse class
        /// </summary>
        /// <param name="data">The data of the response</param>
        public SuccessResponse(TData data)
        {
            this.Data = data;
        }
    }

    /// <summary>
    /// Used for a successful REST response that has text data.
    /// Holds its data in the <see cref="Data"/> field
    /// </summary>
    public class SuccessResponse : SuccessResponse<string>
    {
        /// <summary>
        /// Create a new sucessfull text REST response
        /// </summary>
        /// <param name="data">The data of the response</param>
        public SuccessResponse(string data): base(data)
        { }
    }
}
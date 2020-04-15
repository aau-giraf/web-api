namespace GirafRest.Models.Responses
{
    public class MyResponse<TData>
    {
        public TData Data;

        public MyResponse(TData data)
        {
            this.Data = data;
        }
    }

    public class MyResponse : MyResponse<string>
    {
        public MyResponse(string data): base(data)
        { }
    }
}
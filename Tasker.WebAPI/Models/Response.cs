namespace Tasker.WebAPI.Models
{
    public class Response<TData>
    {
        public TData Data { get; private set; }
        public Response(TData data)
        {
            Data = data;
        }
    }
}

namespace Tasker.WebAPI.Models
{
    public class Response<TData>
    {
        public readonly TData Data;
        public Response(TData data)
        {
            Data = data;
        }
    }
}

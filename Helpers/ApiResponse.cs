namespace UserManagement.Api.Helpers
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public ApiResponse(T data)
        {
            Data = data;
        }
    }

    public class ApiListResponse<T>
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }

        public ApiListResponse(List<T> data, int count)
        {
            Data = data;
            Count = count;
        }
    }
}

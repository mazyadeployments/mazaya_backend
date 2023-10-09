using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Models
{
    public class ApiResponse<T>
    {
        public T data { get; set; }

        private ApiResponse() { }

        public static ApiResponse<T> Response(string message, T data)
        {
            return new ApiResponse<T>
            {
                data = data
            };
        }


    }
}

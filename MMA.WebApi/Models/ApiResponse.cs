using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Models
{
    public class ApiResponse<T>
    {
        public string Version { get; set; }
        //public string Action { get; set; }
        public string UserMessage { get; set; }
        public int Status { get; set; }
        public T data { get; set; }

        private ApiResponse() { }

        public static ApiResponse<T> Response(string message, T data)
        {
            return new ApiResponse<T>
            {
                Status = 200,
                Version = "1.0.0",
               // Action = UserValidationList.ShowBlockPage.ToString(),
                UserMessage = message,
                data = data
            };
        }


    }
}

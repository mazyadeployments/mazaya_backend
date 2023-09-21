using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Models
{
    public class ApiConnectorResponse
    {
        public string Version { get; set; }
        public string Action { get; set; }
        public string UserMessage { get; set; }
        public int Status { get; set; }

        private ApiConnectorResponse() { }

        public static ApiConnectorResponse BlockingResponse(string message)
        {
            return new ApiConnectorResponse
            {
                Status = 200,
                Version = "1.0.0",
                Action = UserValidationList.ShowBlockPage.ToString(),
                UserMessage = message
            };
        }

        public static ApiConnectorResponse ContinuationResponse(string message)
        {
            return new ApiConnectorResponse
            {
                Status = 200,
                Version = "1.0.0",
                Action = UserValidationList.Continue.ToString(),
                UserMessage = message
            };
        }
        public static ApiConnectorResponse ValidationErrorResponse(string message)
        {
            return new ApiConnectorResponse
            {
                Status = 400,
                Version = "1.0.0",
                Action = UserValidationList.ValidationError.ToString(),
                UserMessage = message
            };
        }
    }
}

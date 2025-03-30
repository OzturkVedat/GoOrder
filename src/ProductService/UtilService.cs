using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using System.Text.Json;

namespace ProductService
{
    public class UtilService
    {
        public APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = JsonSerializer.Serialize(new { message })
            };
        }

        public APIGatewayProxyResponse CreateDataResponse<T>(HttpStatusCode statusCode, T data)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = JsonSerializer.Serialize(data),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };
        }

    }
}

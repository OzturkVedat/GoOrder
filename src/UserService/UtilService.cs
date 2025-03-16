using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserService
{
    public class UtilService
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;

        public UtilService(IAmazonSimpleSystemsManagement ssmClient)
        {
            _ssmClient = ssmClient;
        }
        public APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = JsonSerializer.Serialize(new { message })
            };
        }
        public async Task<string> GetParameter(string paramName)
        {
            try
            {
                var request = new GetParameterRequest
                {
                    Name = paramName,
                    WithDecryption = true
                };

                var response = await _ssmClient.GetParameterAsync(request);
                return response.Parameter.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching {paramName} from SSM", ex);
            }
        }

        
    }
}

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using System.Net;
using System.Text.Json;
using PaymentService.Models;

namespace PaymentService
{
    public class UtilService
    {

        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly IAmazonSimpleNotificationService _snsClient;

        public UtilService(IAmazonSimpleSystemsManagement ssmClient, IAmazonSimpleNotificationService snsClient)
        {
            _ssmClient = ssmClient;
            _snsClient = snsClient;
        }

        public APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = JsonSerializer.Serialize(new { message })
            };
        }

        public async Task<ResultModel> GetParameter(string paramName, ILambdaContext context)
        {
            try
            {
                var request = new GetParameterRequest
                {
                    Name = paramName,
                    WithDecryption = true
                };

                var response = await _ssmClient.GetParameterAsync(request);
                return new SuccessDataResult<string>(response.Parameter.Value);
            }
            catch (AmazonSimpleSystemsManagementException ex)
            {
                context.Logger.LogError($"Error fetching {paramName} from SSM", ex);
                return new FailureResult("Error while fetching SSM parameter.");
            }
        }

        public async Task<ResultModel> PublishMessageToSns(string topicArn, object message, ILambdaContext context)
        {
            try
            {
                var snsMessage = JsonSerializer.Serialize(message);

                var pubRequest = new PublishRequest
                {
                    TopicArn = topicArn,
                    Message = snsMessage
                };
                var response = await _snsClient.PublishAsync(pubRequest);
                context.Logger.Log($"Message published to SNS with MessageId: {response.MessageId}");
                return new SuccessResult("Message published successfully.");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                context.Logger.LogError($"Error publishing message to SNS", ex);
                return new FailureResult("Error publishing message to SNS.");
            }
        }

    }
}

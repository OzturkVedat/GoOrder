using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.Lambda.Core;
using OrderService.Models;
using System.Text.Json;
using Amazon.SimpleNotificationService.Model;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;

namespace OrderService
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
                /// change

                var response = await _ssmClient.GetParameterAsync(request);
                return new SuccessDataResult<string>(response.Parameter.Value);
            }
            catch (AmazonSimpleSystemsManagementException ex)
            {
                context.Logger.LogError($"Error fetching param from SSM: ", ex);
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

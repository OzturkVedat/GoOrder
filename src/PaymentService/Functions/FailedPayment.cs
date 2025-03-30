using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using PaymentService.Models;
using System.Net;
using System.Text.Json;

namespace PaymentService.Functions;

public class FailedPayment
{
    private readonly UtilService _utilService;
    public FailedPayment() : this(
        new UtilService(new AmazonSimpleSystemsManagementClient(), new AmazonSimpleNotificationServiceClient())
        )
    { }
    public FailedPayment(UtilService utilService)
    {
        _utilService = utilService;
    }

    /// <summary>
    /// Handles failed payment notifications from SNS
    /// </summary>
    /// <param name="snsEvent">The SNS event containing the payment failure message</param>
    /// <param name="context">Lambda execution context</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public APIGatewayProxyResponse FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Received {snsEvent.Records.Count} payment failure messages.");

        foreach(var record in snsEvent.Records)
        {
            try
            {
                var failedPayment = JsonSerializer.Deserialize<PaymentRequest>(record.Sns.Message);
                context.Logger.LogError($"Payment Failed: OrderId {failedPayment.OrderId}, Reason: {failedPayment.Status}");
                _utilService.CreateResponse(HttpStatusCode.BadRequest, "Payment failed.");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error processing SNS message: {ex.Message}");
                _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Payment failed.");
            }
        }
    }

}


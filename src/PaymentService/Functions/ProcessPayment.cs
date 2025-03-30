using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using PaymentService.Models;
using System.Net;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PaymentService.Functions;

public class ProcessPayment
{
    private readonly UtilService _utilService;

    public ProcessPayment() : this(
        new UtilService(new AmazonSimpleSystemsManagementClient(), new AmazonSimpleNotificationServiceClient())
        )
    { }
    public ProcessPayment(UtilService utilService)
    {
        _utilService = utilService;
    }


    /// <summary>
    /// Lambda function that processes payment requests from SQS
    /// </summary>
    /// <param name="sqsEvent">The SQS event containing payment messages</param>
    /// <param name="context">Lambda context with information about the execution environment</param>
    /// <returns>Async task</returns>
    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {sqsEvent.Records.Count} records");
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var record in sqsEvent.Records)
        {
            PaymentRequest? paymentRequest = null;
            try
            {
                var snsMessageWrapper = JsonSerializer.Deserialize<SnsWrapper>(record.Body);
                if (snsMessageWrapper == null || string.IsNullOrEmpty(snsMessageWrapper.Message))
                {
                    context.Logger.LogError("Invalid SQS message format.");
                    continue;   // skip this record
                }
                paymentRequest = JsonSerializer.Deserialize<PaymentRequest>(snsMessageWrapper.Message, jsonOptions);
                if (paymentRequest == null)
                {
                    context.Logger.LogError("Failed to deserialize PaymentRequest.");
                    continue;
                }
                var paymentResult = await ProcessPaymentAsync(paymentRequest);
                if (paymentResult)
                {
                    var topicArnResult = await _utilService.GetParameter("/goorder/topic-arn/payment-confirmed", context);
                    if (topicArnResult is SuccessDataResult<string> topicArn)
                    {
                        paymentRequest.Status = "Paid";
                        await _utilService.PublishMessageToSns(topicArn.Data, paymentRequest, context);
                    }
                }
                else
                    await PublishPaymentFailed(paymentRequest, context);
            }
            catch (AmazonServiceException ex)
            {
                context.Logger.LogError($"SQS error while processing payment request: {ex.Message}");
                if (paymentRequest != null)
                    await PublishPaymentFailed(paymentRequest, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Unexpected error when processing payment request: {ex.Message}");
                if (paymentRequest != null)
                    await PublishPaymentFailed(paymentRequest, context);
            }
        }

    }

    private async Task<bool> ProcessPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(200);        // simulate latency
        return true;        // for demo purposes always return true
    }

    private async Task PublishPaymentFailed(PaymentRequest payReq, ILambdaContext context)
    {
        var topicArnResult = await _utilService.GetParameter("/goorder/topic-arn/payment-failed", context);
        if (topicArnResult is SuccessDataResult<string> topicArn)
            await _utilService.PublishMessageToSns(topicArn.Data, payReq, context);
    }
}

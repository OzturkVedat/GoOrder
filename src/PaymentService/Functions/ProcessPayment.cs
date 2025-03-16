using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Amazon.SQS;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PaymentService.Functions;

public class ProcessPayment
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly IAmazonSimpleSystemsManagement _ssmClient;

    private readonly UtilService _utilService;
    public ProcessPayment()
    {
        _snsClient= new AmazonSimpleNotificationServiceClient();
        _ssmClient = new AmazonSimpleSystemsManagementClient();

        _utilService = new UtilService(_ssmClient, _snsClient);
    }


    /// <summary>
    /// Lambda function that processes payment requests from SQS
    /// </summary>
    /// <param name="sqsEvent">The SQS event containing payment messages</param>
    /// <param name="context">Lambda context with information about the execution environment</param>
    /// <returns>Async task</returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(sqs request, ILambdaContext context)
    {


    }
}

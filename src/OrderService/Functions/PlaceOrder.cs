using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using OrderService.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OrderService.Functions;

public class PlaceOrder
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly IAmazonSimpleSystemsManagement _ssmClient;
    private readonly UtilService _utilService;


    private readonly IAmazonDynamoDB _dynamoClient;
    private readonly string _productsTableName = "Products";
    private readonly string _ordersTableName = "Orders";
    public PlaceOrder()
    {
        _ssmClient = new AmazonSimpleSystemsManagementClient();
        _snsClient= new AmazonSimpleNotificationServiceClient();
        _utilService = new UtilService(_ssmClient, _snsClient);

        _dynamoClient = new AmazonDynamoDBClient();
    }

    /// <summary>
    /// This function handles the API Gateway request to place an order.
    /// </summary>
    /// <param name="request">The order request for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>An HTTP response with the order result.</returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogLine("Received order request: " + request.Body);

            var orderReq = JsonSerializer.Deserialize<CreateOrderRequest>(request.Body);
            var priceResult = await GetTotalPriceFromDb(orderReq.Items, context);
            if (priceResult is FailureResult priceFail)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { message = priceFail.Message })
                };
            }

            var price = priceResult as SuccessDataResult<decimal>;
            var order = new OrderModel
            {
                UserId = orderReq.UserId,
                Items = orderReq.Items,
                TotalPrice = price.Data
            };
            var valErrors = ValidateOrder(order);
            if (valErrors.Any())
            {
                context.Logger.LogError($"Order validation failed: {string.Join(", ", valErrors)}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { message = "Invalid order", errors = valErrors })
                };
            }

            var dbResult = await SaveOrderToDb(order, context);
            if (dbResult is FailureResult saveFail)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { message = saveFail.Message })
                };
            }

            var paramResult = _utilService.GetParameter("goorder/topic-arn/payment-required", context).GetAwaiter().GetResult();
            if( paramResult is not SuccessDataResult<string> topicResult)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { message = "An unexpected error occured" })
                };
            }          
            var paymentMessage = new
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                Status = order.Status
            };
            var pubResult = await _utilService.PublishMessageToSns(topicResult.Data, paymentMessage, context);
            if (pubResult.IsSuccess)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(new { message = "Order placed successfully" })
                };
            }
            // process

        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Unexpected error: {ex.Message}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { message = "Internal server error" })
            };
        }
    }

    private List<string> ValidateOrder(OrderModel? order)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(order.UserId))
            errors.Add("UserId is required.");

        if (order.Items == null || !order.Items.Any())
            errors.Add("At least one item must be included in the order.");

        if (order.TotalPrice <= 0)
            errors.Add("TotalPrice must be a positive value.");

        return errors;
    }

    private async Task<ResultModel> GetTotalPriceFromDb(Dictionary<string, int> items, ILambdaContext context)
    {
        var keys = items.Keys.Select(itemId => new Dictionary<string, AttributeValue>
        {
            { "ItemId", new AttributeValue{ S = itemId } }
        }).ToList();    // converting to dynamo compatible format

        var request = new BatchGetItemRequest
        {
            RequestItems = new Dictionary<string, KeysAndAttributes>
            {
                { _productsTableName, new KeysAndAttributes { Keys = keys } }
            }
        };

        try
        {
            var response = await _dynamoClient.BatchGetItemAsync(request);      // fetch the batch and process
            decimal total = 0;

            if (response.Responses.ContainsKey(_productsTableName))
            {
                foreach (var item in response.Responses[_productsTableName])
                {
                    var itemId = item["ItemId"].S;
                    if (item.ContainsKey("Price") && decimal.TryParse(item["Price"].N, out decimal price))
                        total += price * items[itemId];  // Multiply by quantity

                    else
                    {
                        context.Logger.Log($"Price not found for item: {itemId}");
                        return new FailureResult("Invalid operation. Please check your cart.");
                    }
                }
            }
            return new SuccessDataResult<decimal>(total);
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogError($"Dynamo exception thrown: {ex}");
            return new FailureResult("An error occured while processing your order. Please try again later.");
        }
    }

    private async Task<ResultModel> SaveOrderToDb(OrderModel order, ILambdaContext context)
    {
        try
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "OrderId", new AttributeValue { S = order.OrderId } },
                { "UserId", new AttributeValue { S = order.UserId } },
                { "TotalPrice", new AttributeValue { N = order.TotalPrice.ToString() } },
                { "CreatedAt", new AttributeValue { S = order.CreatedAt } },
                { "Items", new AttributeValue { S = JsonSerializer.Serialize(order.Items) } }
            };
            var request = new PutItemRequest
            {
                TableName = _ordersTableName,
                Item = item
            };
            await _dynamoClient.PutItemAsync(request);
            return new SuccessResult("Order successully added.");
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogError($"Dynamo exception thrown: {ex}");
            return new FailureResult("An error occured while processing your order. Please try again later.");
        }
    }

}

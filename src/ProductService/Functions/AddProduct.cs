using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ProductService.Models;
using System.Net;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProductService.Functions;

public class AddProduct
{
    private readonly string _dynamoTableName = "GoOrderTable";
    private readonly IAmazonDynamoDB _dynamoClient;

    private readonly UtilService _utilService;

    public AddProduct() : this(new AmazonDynamoDBClient(), new UtilService()) { }

    public AddProduct(IAmazonDynamoDB dynamoClient, UtilService utilService)
    {
        _dynamoClient = dynamoClient;
        _utilService = utilService;
    }

    /// <summary>
    /// Lambda function to add a product to Db
    /// </summary>
    /// <param name="request">The request for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request to add a product.");
        AddProductDto dto;
        try
        {
            dto = JsonSerializer.Deserialize<AddProductDto>(request.Body);
            if (dto == null || string.IsNullOrEmpty(dto.ProductName) || string.IsNullOrEmpty(dto.StoreId))
                return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid product data.");
        }
        catch (JsonException)
        {
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid JSON format.");
        }
        var productId = Guid.NewGuid().ToString();
        var putItemReq = new PutItemRequest
        {
            TableName = _dynamoTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = $"STORE#{dto.StoreId}" } },
                { "SK", new AttributeValue { S = $"PRODUCT#{productId}" } },
                { "ProductName", new AttributeValue { S = dto.ProductName } },
                { "Description", new AttributeValue { S = dto.Description ?? string.Empty } },
                { "Price", new AttributeValue { N = dto.Price.ToString() } },
                { "Category", new AttributeValue { S = dto.Category ?? "Uncategorized" } },
            }
        };
        try
        {
            await _dynamoClient.PutItemAsync(putItemReq);
            return _utilService.CreateResponse(HttpStatusCode.Created, $"Product added with the ID: {productId}");
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogError($"DynamoDb exception occured: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Internal server error.");
        }
    }
}

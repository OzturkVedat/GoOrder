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
    private readonly string _productsTableName = "Products";
    private readonly IAmazonDynamoDB _dynamoClient;

    private readonly UtilService _utilService;

    public AddProduct()
    {
        _dynamoClient = new AmazonDynamoDBClient();
        _utilService= new UtilService();
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

        var putItemReq = new PutItemRequest
        {
            TableName = _productsTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "ProductId", new AttributeValue { S = Guid.NewGuid().ToString() } },
                { "ProductName", new AttributeValue { S = dto.ProductName } },
                { "Price", new AttributeValue { N = dto.Price.ToString() } },
                { "Category", new AttributeValue { S = dto.Category ?? "Uncategorized" } },
                { "Description", new AttributeValue { S = dto.Description ?? string.Empty } },
                { "StoreId", new AttributeValue { S = dto.StoreId } }
            }
        };
        try
        {
            await _dynamoClient.PutItemAsync(putItemReq);
            return _utilService.CreateResponse(HttpStatusCode.Created, "Product added successfully.");
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogError($"DynamoDb exception occured: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Internal server error.");
        }
    }

    
}

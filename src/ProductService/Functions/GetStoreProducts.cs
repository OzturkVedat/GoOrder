using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ProductService.Models;
using System.Net;

namespace ProductService.Functions;

public class GetStoreProducts
{
    private readonly string _dynamoTableName = "GoOrderTable";
    private readonly IAmazonDynamoDB _dynamoClient;

    private readonly UtilService _utilService;

    public GetStoreProducts() : this(new AmazonDynamoDBClient(), new UtilService()) { }

    public GetStoreProducts(IAmazonDynamoDB dynamoClient, UtilService utilService)
    {
        _dynamoClient = dynamoClient;
        _utilService = utilService;
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Getting products for a store.");

        if (!request.PathParameters.TryGetValue("storeId", out var storeId) || string.IsNullOrEmpty(storeId))
        {
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Missing or invalid storeId.");
        }

        var queryRequest = new QueryRequest
        {
            TableName = _dynamoTableName,
            KeyConditionExpression = "PK = :pk AND begins_with(SK, :skPrefix)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pk", new AttributeValue { S = $"STORE#{storeId}" } },
                { ":skPrefix", new AttributeValue { S = "PRODUCT#" } }
            }
        };
        try
        {
            var response = await _dynamoClient.QueryAsync(queryRequest);

            var products = response.Items.Select(item => new GetProductDto
            {
                ProductId = item["SK"].S.Replace("PRODUCT#", ""),
                ProductName = item["ProductName"].S,
                Description = item.ContainsKey("Description") ? item["Description"].S : null,
                Category = item.ContainsKey("Category") ? item["Category"].S : null,
                Price = decimal.Parse(item["Price"].N)
            }).ToList();

            return _utilService.CreateDataResponse(HttpStatusCode.OK, products);
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogError($"DynamoDb exception: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Failed to get products.");
        }
    }

}


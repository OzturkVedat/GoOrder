using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ProductService.Models;
using System.Net;
using System.Text.Json;

namespace ProductService.Functions;

public class GetStoreProducts
{
    private readonly string _productTableName = "Products";
    private readonly IAmazonDynamoDB _dynamoClient;

    private readonly UtilService _utilService;
    public GetStoreProducts()
    {
        _dynamoClient = new AmazonDynamoDBClient();
        _utilService = new UtilService();
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request to get store products.");

        if (!request.PathParameters.TryGetValue("storeId", out string storeId) || string.IsNullOrEmpty(storeId))
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Missing or invalid storeId parameter.");

        var queryReq = new QueryRequest
        {
            TableName = _productTableName,
            IndexName= "StoreIdIndex",      // specify GSI
            KeyConditionExpression = "StoreId = :storeId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":storeId", new AttributeValue{ S = storeId} }
            }
        };

        try
        {
            var queryRes = await _dynamoClient.QueryAsync(queryReq);
            var prods = queryRes.Items.Select(item => new ProductModel
            {
                ProductId = item["ProductId"].S,
                ProductName = item["ProductName"].S,
                Price = decimal.Parse(item["Price"].N),
                Category = item.ContainsKey("Category") ? item["Category"].S : "Uncategorized",
                Description = item.ContainsKey("Description") ? item["Description"].S : string.Empty,
                StoreId = item["StoreId"].S
            }).ToList();

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(prods),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error fetching products: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Failed to fetch store products.");
        }
    }


}


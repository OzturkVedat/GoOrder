using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System.Net;
using System.Text.Json;
using UserService.Dto;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserService.Functions;

public class RegisterUser
{
    private readonly UtilService _utilService;

    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly IAmazonDynamoDB _dynamoClient;
    private readonly string _dynamoTableName;
    private readonly string _appClientId;

    public RegisterUser() : this(
        new UtilService(new AmazonSimpleSystemsManagementClient()),
        new AmazonCognitoIdentityProviderClient(),
        new AmazonDynamoDBClient())
    { }     // default constructor, passes these (this) arguments to the second constructor for a quick init

    public RegisterUser(
        UtilService utilService,
        IAmazonCognitoIdentityProvider cognitoClient,
        IAmazonDynamoDB dynamoClient)      // can be used for testing
    {
        _utilService = utilService;

        _cognitoClient = cognitoClient;
        _dynamoClient = dynamoClient;
        _dynamoTableName = _utilService.GetParameter("/goorder/dynamo-table-name").GetAwaiter().GetResult();
        _appClientId = _utilService.GetParameter("/goorder/app-client-id").GetAwaiter().GetResult();
    }


    /// <summary>
    /// Registers a new user with Cognito.
    /// </summary>
    /// <param name="request">The user registration request containing email and password.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>The Cognito User ID (sub) of the newly registered user.</returns>
    /// <exception cref="Exception">Thrown when user registration fails.</exception>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogLine($"Raw request body: {request.Body}");
        var registerReq = JsonSerializer.Deserialize<RegisterRequest>(request.Body);
        if (registerReq == null || string.IsNullOrEmpty(registerReq.Email) || string.IsNullOrEmpty(registerReq.Password))
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid request: Email or Password is missing.");
        if (string.IsNullOrEmpty(registerReq.FullName))
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "FullName is required.");

        try
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _appClientId,
                Username = registerReq.Email,
                Password = registerReq.Password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType{Name= "email", Value=registerReq.Email}
                }
            };
            var signUpResponse = await _cognitoClient.SignUpAsync(signUpRequest);
            context.Logger.LogLine($"User registered successfully with sub: {signUpResponse.UserSub}");

            var putItemReq = new PutItemRequest
            {
                TableName = _dynamoTableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue{ S= $"USER#{signUpResponse.UserSub}"} },
                    { "SK", new AttributeValue{ S= "PROFILE" } },
                    { "FullName", new AttributeValue{ S =registerReq.FullName} },
                    { "Email", new AttributeValue{ S =registerReq.Email} },
                    { "CreatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("o") } }
                }
            };
            await _dynamoClient.PutItemAsync(putItemReq);
            return _utilService.CreateResponse(HttpStatusCode.Created, "User registered successfully");
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error registering user: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Failed to register user..");
        }
        catch (AmazonDynamoDBException ex)
        {
            context.Logger.LogLine($"DynamoDB failed. Cleaning up Cognito user. Error: {ex.Message}");

            try
            {
                await _cognitoClient.AdminDeleteUserAsync(new AdminDeleteUserRequest
                {
                    UserPoolId = _utilService.GetParameter("/goorder/user-pool-id").GetAwaiter().GetResult(),
                    Username = registerReq.Email
                });

                context.Logger.LogLine("Cognito user deleted due to DynamoDB failure.");
            }
            catch (Exception deleteEx)
            {
                context.Logger.LogLine($"Failed to clean up Cognito user: {deleteEx.Message}");
            }
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "User registration failed. Please try again.");
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Unhandled exception: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please try again.");
        }
    }
}

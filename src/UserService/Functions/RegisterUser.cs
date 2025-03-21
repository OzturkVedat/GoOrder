using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
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

    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly UtilService _utilService;
    private readonly string _userPoolId;
    private readonly string _appClientId;
    
    public RegisterUser()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient();

        var ssmClient = new AmazonSimpleSystemsManagementClient();
        _utilService = new UtilService(ssmClient);
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
        var authRequest = JsonSerializer.Deserialize<AuthRequest>(request.Body);
        if (authRequest == null || string.IsNullOrEmpty(authRequest.Email) || string.IsNullOrEmpty(authRequest.Password))
            return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid request payload: Email or Password is missing.");

        try
        {
            
            var signUpRequest = new SignUpRequest
            {
                ClientId = _appClientId,
                Username = authRequest.Email,
                Password = authRequest.Password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType{Name= "email", Value=authRequest.Email}
                }
            };
            var signUpResponse = await _cognitoClient.SignUpAsync(signUpRequest);
            context.Logger.LogLine($"User registered successfully with sub: {signUpResponse.UserSub}");

            return _utilService.CreateResponse(HttpStatusCode.Created, "User registered successfully");
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error registering user: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Failed to register user..");
        }

    }
}

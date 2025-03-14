using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System.Text.Json;
using UserService.Dto;

namespace UserService.Functions;

public class LoginUser
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly UtilService _utilService;
    private readonly string _appClientId;

    public LoginUser()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient();

        var _ssmClient = new AmazonSimpleSystemsManagementClient();
        _utilService = new UtilService(_ssmClient);
        _appClientId = _utilService.GetParameter("/goorder/app-client-id").GetAwaiter().GetResult();
    }

    /// <summary>
    /// Authenticates a user with Cognito.
    /// </summary>
    /// <param name="request">The user login request containing email and password.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging.</param>
    /// <returns>The authentication result including the access token (or error ofc).</returns>
    /// <exception cref="Exception">Thrown when authentication fails.</exception>

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            var authRequest = JsonSerializer.Deserialize<AuthRequest>(request.Body);
            if (authRequest == null || string.IsNullOrEmpty(authRequest.Email) || string.IsNullOrEmpty(authRequest.Password))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { Message = "Invalid request payload: Email or Password is missing." }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var initAuthRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _appClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", authRequest.Email },
                    { "PASSWORD", authRequest.Password },
                    {"SECRET_HASH", await _utilService.ComputeSecretHash(_appClientId, authRequest.Email) }
                }
            };

            var authResponse = await _cognitoClient.InitiateAuthAsync(initAuthRequest);

            var tokens = new AuthResponse
            {
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken
            };
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { Message = "Login successful", Tokens = tokens }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error logging in user: {ex.Message}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 401,
                Body = JsonSerializer.Serialize(new { Message = "Failed to log in user." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

    }

}


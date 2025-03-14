using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
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
    /// <returns>The authentication result including the access token.</returns>
    /// <exception cref="Exception">Thrown when authentication fails.</exception>

    public async Task<ResultModel> FunctionHandler(AuthRequest request, ILambdaContext context)
    {
        context.Logger.LogLine($"Logging in user with email: {request.Email}");

        try
        {
            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _appClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", request.Email },
                    { "PASSWORD", request.Password },
                    {"SECRET_HASH", await _utilService.ComputeSecretHash(_appClientId, request.Email) }
                }
            };

            var authResponse = await _cognitoClient.InitiateAuthAsync(authRequest);

            var tokens = new AuthResponse
            {
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken
            };
            return new SuccessDataResult<AuthResponse>(tokens);
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error logging in user: {ex.Message}");
            return new FailureResult("Failed to log in user.");
        }

    }

}


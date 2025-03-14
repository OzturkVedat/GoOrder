using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
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

        _userPoolId = _utilService.GetParameter("/goorder/user-pool-id").GetAwaiter().GetResult();
        _appClientId = _utilService.GetParameter("/goorder/app-client-id").GetAwaiter().GetResult();
    }

    /// <summary>
    /// Registers a new user with Cognito.
    /// </summary>
    /// <param name="request">The user registration request containing email and password.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>The Cognito User ID (sub) of the newly registered user.</returns>
    /// <exception cref="Exception">Thrown when user registration fails.</exception>
    public async Task<string> FunctionHandler(AuthRequest request, ILambdaContext context)
    {
        context.Logger.LogLine($"Registering user with email: {request.Email}");

        try
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _appClientId,
                Username = request.Email,
                Password = request.Password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType{Name= "email", Value=request.Email}
                }
            };
            var signUpResponse = await _cognitoClient.SignUpAsync(signUpRequest);
            context.Logger.LogLine($"User registered successfully with sub: {signUpResponse.UserSub}");
            return signUpResponse.UserSub;
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error registering user: {ex.Message}");
            throw new Exception("Error registering user in Cognito", ex);
        }

    }
}

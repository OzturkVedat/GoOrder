using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserService.Dto;

namespace UserService.Functions;

public class ConfirmUser
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly UtilService _utilService;
    private readonly string _appClientId;

    public ConfirmUser()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient();

        var ssmClient = new AmazonSimpleSystemsManagementClient();
        _utilService = new UtilService(ssmClient);
        _appClientId = _utilService.GetParameter("/goorder/app-client-id").GetAwaiter().GetResult();
    }

    /// <summary>
    /// Confirms user via email with Cognito.
    /// </summary>
    /// <param name="request">The user confirmation request containing email and code.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging.</param>
    /// <returns>The confirmation result.</returns>
    /// <exception cref="Exception">Thrown when confirmation fails.</exception>

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            var confirmRequest = JsonSerializer.Deserialize<ConfirmRequest>(request.Body);
            if (confirmRequest == null || string.IsNullOrEmpty(confirmRequest.Email) || string.IsNullOrEmpty(confirmRequest.ConfirmationCode))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { message = "Invalid request payload: Email or Confirmation Code is missing." })
                };
            }

            //var secretHash = await _utilService.ComputeSecretHash(_appClientId, confirmRequest.Email);
            var cognitoRequest = new ConfirmSignUpRequest
            {
                ClientId = _appClientId,
                Username = confirmRequest.Email,
               // SecretHash = secretHash,
                ConfirmationCode = confirmRequest.ConfirmationCode
            };
            await _cognitoClient.ConfirmSignUpAsync(cognitoRequest);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { message = "User confirmed successfully" })
            };
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error confirming user: {ex.Message}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { message = "Error while confirming user." })
            };
        }
    }
}


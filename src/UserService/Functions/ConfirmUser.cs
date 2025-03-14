using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public async Task<ResultModel> FunctionHandler(ConfirmRequest request, ILambdaContext context)
    {
        try
        {
            var secretHash = await _utilService.ComputeSecretHash(_appClientId, request.Email);
            var cognitoRequest = new ConfirmSignUpRequest
            {
                ClientId = _appClientId,
                Username = request.Email,
                SecretHash = secretHash,
                ConfirmationCode = request.ConfirmationCode
            };
            await _cognitoClient.ConfirmSignUpAsync(cognitoRequest);
            return new SuccessResult("User confirmed successfully");
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error confirming user: {ex.Message}");
            return new FailureResult("Error while confirming user..");
        }
    }
}


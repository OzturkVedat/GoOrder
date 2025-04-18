﻿using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System.Net;
using System.Text.Json;
using UserService.Dto;

namespace UserService.Functions;

public class ConfirmUser
{
    private readonly UtilService _utilService;

    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly string _appClientId;

    public ConfirmUser() : this(new AmazonCognitoIdentityProviderClient(), new UtilService(new AmazonSimpleSystemsManagementClient()))
    { }

    public ConfirmUser(IAmazonCognitoIdentityProvider cognitoClient, UtilService utilService)
    {
        _utilService = utilService;

        _cognitoClient = cognitoClient;
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
                return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid request: Email or Confirmation Code is missing.");

            var cognitoRequest = new ConfirmSignUpRequest
            {
                ClientId = _appClientId,
                Username = confirmRequest.Email,
                ConfirmationCode = confirmRequest.ConfirmationCode
            };
            await _cognitoClient.ConfirmSignUpAsync(cognitoRequest);
            return _utilService.CreateResponse(HttpStatusCode.OK, "User confirmed successfully");
        }
        catch (AmazonCognitoIdentityProviderException ex)
        {
            context.Logger.LogLine($"Error confirming user: {ex.Message}");
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Error while confirming user..");
        }
    }
}


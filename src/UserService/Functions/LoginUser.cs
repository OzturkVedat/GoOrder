﻿using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using System.Net;
using System.Text.Json;
using UserService.Dto;

namespace UserService.Functions;

public class LoginUser
{
    private readonly UtilService _utilService;

    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly string _appClientId;

    public LoginUser() : this(new AmazonCognitoIdentityProviderClient(), new UtilService(new AmazonSimpleSystemsManagementClient()))
    { }

    public LoginUser(IAmazonCognitoIdentityProvider cognitoClient, UtilService utilService)
    {
        _utilService = utilService;

        _cognitoClient = cognitoClient;
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
                return _utilService.CreateResponse(HttpStatusCode.BadRequest, "Invalid request payload: Email or Password is missing.");

            var initAuthRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _appClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", authRequest.Email },
                    { "PASSWORD", authRequest.Password }
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
            return _utilService.CreateResponse(HttpStatusCode.InternalServerError, "Failed to login user..");
        }

    }

}


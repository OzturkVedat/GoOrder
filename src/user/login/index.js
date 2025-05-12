const { CognitoIdentityProviderClient, InitiateAuthCommand } = require("@aws-sdk/client-cognito-identity-provider");

const cognito = new CognitoIdentityProviderClient({});
const CLIENT_ID = process.env.CLIENT_ID;

exports.handler = async (event) => {
  try {
    const body = JSON.parse(event.body);
    const { email, password } = body;

    if (!email || !password) {
      return {
        statusCode: 400,
        body: JSON.stringify({ message: "Email and password are required." }),
      };
    }

    const command = new InitiateAuthCommand({
      AuthFlow: "USER_PASSWORD_AUTH",
      ClientId: CLIENT_ID,
      AuthParameters: {
        USERNAME: email,
        PASSWORD: password,
      },
    });

    const result = await cognito.send(command);

    return {
      statusCode: 200,
      body: JSON.stringify({
        message: "Login successful",
        idToken: result.AuthenticationResult.IdToken,
        accessToken: result.AuthenticationResult.AccessToken,
        refreshToken: result.AuthenticationResult.RefreshToken,
      }),
    };
  } catch (err) {
    console.error("Login error:", err);

    let message = "Login failed.";
    if (err.name === "NotAuthorizedException") {
      message = "Incorrect username or password.";
    } else if (err.name === "UserNotConfirmedException") {
      message = "User not confirmed. Please verify your email.";
    }

    return {
      statusCode: 400,
      body: JSON.stringify({ message, error: err.message }),
    };
  }
};

const { CognitoIdentityProviderClient, ConfirmSignUpCommand } = require("@aws-sdk/client-cognito-identity-provider");

const cognito = new CognitoIdentityProviderClient({ region: process.env.AWS_REGION });
const CLIENT_ID = process.env.CLIENT_ID;

exports.handler = async (event) => {
  try {
    const body = JSON.parse(event.body);
    const { email, code } = body;

    if (!email || !code) {
      return {
        statusCode: 400,
        body: JSON.stringify({ message: "Email and code are required." }),
      };
    }

    const command = new ConfirmSignUpCommand({
      ClientId: CLIENT_ID,
      Username: email,
      ConfirmationCode: code,
    });

    await cognito.send(command);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: "User confirmed." }),
    };
  } catch (err) {
    console.error("Verification error:", err);
    return {
      statusCode: 400,
      body: JSON.stringify({ message: "Verification failed.", error: err.message }),
    };
  }
};

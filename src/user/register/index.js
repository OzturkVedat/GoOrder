const { CognitoIdentityProviderClient, SignUpCommand } = require("@aws-sdk/client-cognito-identity-provider");

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
    const command = new SignUpCommand({
      ClientId: CLIENT_ID,
      Username: email,
      Password: password,
      UserAttributes: [
        {
          Name: "email",
          Value: email,
        },
      ],
    });
    const result = await cognito.send(command);

    return {
      statusCode: 200,
      body: JSON.stringify({
        message: "User registered. Please confirm your email.",
        userConfirmed: result.UserConfirmed,
      }),
    };
  } catch (err) {
    console.error("Registration error:", err);

    let message = "Registration failed.";
    if (err.name === "UsernameExistsException") {
      message = "User already exists.";
    }

    return {
      statusCode: 400,
      body: JSON.stringify({ message }),
    };
  }
};

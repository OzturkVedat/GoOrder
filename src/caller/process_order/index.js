const { SFNClient, StartExecutionCommand } = require("@aws-sdk/client-sfn");

const sfn = new SFNClient({});
const STATE_MACHINE_ARN = process.env.PROCESS_ORDER_SM_ARN;

exports.handler = async (event) => {
  try {
    const claims = event.requestContext.authorizer.jwt.claims;
    const userId = claims.sub;
    if (!userId) {
      return {
        statusCode: 401,
        body: JSON.stringify({ message: "Unauthorized: Missing user identity." }),
      };
    }
    const body = JSON.parse(event.body || "{}");
    const cart = body.cart;
    if (!Array.isArray(cart) || cart.length === 0) {
      return {
        statusCode: 400,
        body: JSON.stringify({ message: "Cart is empty." }),
      };
    }

    const input = {
      userId,
      cart,
    };
    console.log("Triggering the Step function with input: ", input);
    const command = new StartExecutionCommand({
      stateMachineArn: STATE_MACHINE_ARN,
      input: JSON.stringify(input),
    });

    const result = await sfn.send(command);

    return {
      statusCode: 202,
      body: JSON.stringify({
        message: "Checkout started",
        executionArn: result.executionArn,
      }),
    };
  } catch (err) {
    console.error("Failed to start Step Function execution:", err);

    return {
      statusCode: 500,
      body: JSON.stringify({
        message: "Failed to start checkout process.",
        error: err.message || "Unknown error",
      }),
    };
  }
};

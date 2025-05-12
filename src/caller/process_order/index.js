const { SFNClient, StartExecutionCommand } = require("@aws-sdk/client-sfn");

const sfn = new SFNClient({});
const STATE_MACHINE_ARN = process.env.PROCESS_ORDER_SM_ARN;

exports.handler = async (event) => {
  const claims = event.requestContext.authorizer.jwt.claims;
  const userId = claims.sub;

  const body = JSON.parse(event.body);

  const input = {
    userId,
    cart: body.cart,
  };

  const command = new StartExecutionCommand({
    stateMachineArn: STATE_MACHINE_ARN,
    input: JSON.stringify(input),
  });

  const result = await sfn.send(command); //trigger

  return {
    statusCode: 202,
    body: JSON.stringify({
      message: "Checkout started",
      executionArn: result.executionArn,
    }),
  };
};

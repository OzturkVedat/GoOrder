const { SFNClient, DescribeExecutionCommand } = require("@aws-sdk/client-sfn");

const sfn = new SFNClient({});

exports.handler = async (event) => {
  const executionArn = event.pathParameters?.executionArn;

  if (!executionArn) {
    return {
      statusCode: 400,
      body: JSON.stringify({ message: "Missing order detail in path." }),
    };
  }

  try {
    const result = await sfn.send(new DescribeExecutionCommand({ executionArn }));

    return {
      statusCode: 200,
      body: JSON.stringify({
        status: result.status,
        output: result.output ? JSON.parse(result.output) : null,
      }),
    };
  } catch (err) {
    console.error("Error describing execution:", err);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: "Failed to check order status." }),
    };
  }
};

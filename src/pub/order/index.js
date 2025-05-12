const { SNSClient, PublishCommand } = require("@aws-sdk/client-sns");

const sns = new SNSClient({});
const ORDER_NOTIF_TOPIC_ARN = process.env.ORDER_TOPIC_ARN;

exports.handler = async (event) => {
  console.log("Sending order notification: ", JSON.stringify(event));

  const { userId, orderId, status, message } = event;

  try {
    const payload = {
      userId,
      orderId,
      status,
      message: message || (status === "success" ? "Order placed" : "Order failed"),
    };

    await sns.send(
      new PublishCommand({
        TopicArn: ORDER_NOTIF_TOPIC_ARN,
        Message: JSON.stringify(payload),
        Subject: `Order ${status.toUpperCase()}`,
      })
    );

    return {
      isSuccess: true,
      message: "Notification sent",
    };
  } catch (error) {
    console.error("Notification failed:", err);
    return {
      isSuccess: false,
      message: "Failed to send notification",
    };
  }
};

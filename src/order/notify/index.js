exports.handler = async (event) => {
  for (const record of event.Records) {
    try {
      const orderEvent = JSON.parse(record.body);

      const { userId, orderId, status, message } = orderEvent;

      const text = status === "success" ? `🎉 Hi ${userId}, your order ${orderId} has been placed successfully.` : `⚠️ Hi ${userId}, your order ${orderId} failed. ${message || ""}`;

      console.log("Sending user notification:", text); // can be replaced with real email or SMS code
    } catch (err) {
      console.error("Failed to parse or notify:", record.body);
    }
  }

  return { status: "ok" };
};

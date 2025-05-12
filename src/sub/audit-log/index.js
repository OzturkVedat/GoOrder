exports.handler = async (event) => {
  for (const record of event.Records) {
    try {
      const orderEvent = JSON.parse(record.body);
      console.log("🧾 AUDIT LOG:", JSON.stringify(orderEvent));
    } catch (err) {
      console.error("Failed to parse message:", record.body);
    }
  }

  return { status: "ok" };
};

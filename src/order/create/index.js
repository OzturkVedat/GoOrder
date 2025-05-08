const { DynamoDBClient, PutItemCommand } = require("@aws-sdk/client-dynamodb");
const { v4: uuidv4 } = require("uuid");

const dynamo = new DynamoDBClient({});
const TABLE_NAME = process.env.DYNAMO_TABLE_NAME;

exports.handler = async (event) => {
  console.log("Received event: ", JSON.stringify(event));

  const { userId, cart, totalPrice } = event;

  if (!userId || !cart || !totalPrice || totalPrice <= 0) {
    return {
      isSuccess: false,
      message: "Invalid order data.",
      userId,
      cart,
      totalPrice,
    };
  }

  const orderId = uuidv4();
  const createdAt = new Date().toISOString();
  const status = "PLACED";

  const item = {
    PK: { S: `USER#${userId}` },
    SK: { S: `ORDER#${orderId}` },
    OrderId: { S: orderId },
    UserId: { S: userId },
    TotalPrice: { N: totalPrice.toString() },
    Status: { S: status },
    CreatedAt: { S: createdAt },
    Cart: { S: JSON.stringify(cart) },
  };

  const params = {
    TableName: TABLE_NAME,
    Item: item,
  };

  try {
    await dynamo.send(new PutItemCommand(params));

    return {
      isSuccess: true,
      message: "Order placed successfully.",
      userId,
      orderId,
      totalPrice,
      cart,
      status,
      createdAt,
    };
  } catch (err) {
    console.error("Error saving order:", err);
    return {
      isSuccess: false,
      message: "Failed to save order.",
      userId,
      cart,
      totalPrice,
    };
  }
};

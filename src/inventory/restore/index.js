const { DynamoDBClient, TransactWriteItemsCommand, BatchGetItemCommand } = require("@aws-sdk/client-dynamodb");

const dynamo = new DynamoDBClient({});
const TABLE_NAME = process.env.DYNAMO_TABLE_NAME;

exports.handler = async (event) => {
  console.log("Restoring inventory:", JSON.stringify(event));

  const { userId, cart } = event;

  if (!userId || !cart || !Array.isArray(cart) || cart.length === 0) {
    return {
      isSuccess: false,
      message: "Missing or invalid userId or cart.",
      userId,
      cart,
    };
  }

  const keys = cart.map((item) => ({
    PK: { S: `STORE#${item.storeId}` },
    SK: { S: `PRODUCT#${item.productId}` },
  }));

  const keyMap = new Map();
  cart.forEach((item) => {
    const key = `${item.storeId}#${item.productId}`;
    keyMap.set(key, item.quantity);
  });

  try {
    const response = await dynamo.send(
      new BatchGetItemCommand({
        RequestItems: {
          [TABLE_NAME]: { Keys: keys },
        },
      })
    );

    const dbItems = response.Responses?.[TABLE_NAME] || [];
    const transactItems = [];

    for (const dbItem of dbItems) {
      const storeId = dbItem.PK.S.replace("STORE#", "");
      const productId = dbItem.SK.S.replace("PRODUCT#", "");
      const key = `${storeId}#${productId}`;
      const quantity = keyMap.get(key);

      const reserved = parseInt(dbItem.Reserved?.N || "0");

      if (reserved < quantity) {
        return {
          isSuccess: false,
          message: `Cannot restore more than reserved for ${productId} in ${storeId}.`,
          userId,
          cart,
        };
      }

      transactItems.push({
        Update: {
          TableName: TABLE_NAME,
          Key: {
            PK: { S: `STORE#${storeId}` },
            SK: { S: `PRODUCT#${productId}` },
          },
          UpdateExpression: "SET Stock = Stock + :qty, Reserved = Reserved - :qty",
          ConditionExpression: "Reserved >= :qty",
          ExpressionAttributeValues: {
            ":qty": { N: quantity.toString() },
          },
        },
      });
    }

    await dynamo.send(
      new TransactWriteItemsCommand({
        TransactItems: transactItems,
      })
    );

    return {
      isSuccess: true,
      message: "Inventory restored.",
      userId,
      cart,
    };
  } catch (err) {
    console.error("Release failed:", err.message || err);
    return {
      isSuccess: false,
      message: "Failed to restore inventory.",
      userId,
      cart,
    };
  }
};

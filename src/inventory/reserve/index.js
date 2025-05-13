const { DynamoDBClient, BatchGetItemCommand, TransactWriteItemsCommand } = require("@aws-sdk/client-dynamodb");

const dynamo = new DynamoDBClient({});
const TABLE_NAME = process.env.DYNAMO_TABLE_NAME;

exports.handler = async (event) => {
  console.log("Reserving inventory:", JSON.stringify(event));

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
    if (dbItems.length < keys.length) {
      return {
        isSuccess: false,
        message: "One or more products not found.",
      };
    }

    let totalPrice = 0;
    const transactItems = [];

    for (const dbItem of dbItems) {
      const storeId = dbItem.PK.S.replace("STORE#", "");
      const productId = dbItem.SK.S.replace("PRODUCT#", "");
      const key = `${storeId}#${productId}`;
      const quantity = keyMap.get(key);

      const stock = parseInt(dbItem.Stock?.N || "0");
      const price = parseFloat(dbItem.Price?.N || "0");

      if (stock < quantity) {
        return {
          isSuccess: false,
          message: `Insufficient stock for ${productId} in ${storeId}.`,
          userId,
          cart,
        };
      }

      totalPrice += price * quantity;

      transactItems.push({
        Update: {
          TableName: TABLE_NAME,
          Key: {
            PK: { S: `STORE#${storeId}` },
            SK: { S: `PRODUCT#${productId}` },
          },
          UpdateExpression: "SET Stock = Stock - :qty, Reserved = if_not_exists(Reserved, :zero) + :qty",
          ConditionExpression: "Stock >= :qty",
          ExpressionAttributeValues: {
            ":qty": { N: quantity.toString() },
            ":zero": { N: "0" },
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
      message: "Inventory reserved.",
      userId,
      cart,
      totalPrice,
    };
  } catch (err) {
    console.error("Reservation failed:", err.message || err);
    return {
      isSuccess: false,
      message: "Failed to reserve inventory.",
      userId,
      cart,
    };
  }
};

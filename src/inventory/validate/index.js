const { DynamoDBClient, BatchGetItemCommand } = require("@aws-sdk/client-dynamodb");

const dynamo = new DynamoDBClient({});
const TABLE_NAME = process.env.DYNAMO_TABLE_NAME;

exports.handler = async (event) => {
  console.log("Incoming event: ", JSON.stringify(event));

  const cart = event.cart || {};
  const itemIds = Object.keys(cart);

  if (itemIds.length === 0) {
    return {
      isSuccess: false,
      message: "Cart is empty.",
      errors: ["No items provided."],
    };
  }

  const keys = itemIds.map((itemId) => ({
    PK: { S: `PRODUCT#${itemId}` },
    SK: { S: "METADATA" },
  }));

  const params = {
    RequestItems: {
      [TABLE_NAME]: {
        Keys: keys,
      },
    },
  };

  try {
    const data = await dynamo.send(new BatchGetItemCommand(params));
    const items = data.Responses?.[TABLE_NAME] || [];

    const errors = [];
    let totalPrice = 0;
    for (const dbItem of items) {
      const itemId = dbItem.PK.S.replace("PRODUCT#", "");
      const stock = parseInt(dbItem.Stock?.N || "0");
      const price = parseFloat(dbItem.Price?.N || "0");

      const requestedQty = cart[itemId];

      if (requestedQty > stock) {
        errors.push(`Insufficient stock for ${itemId}: requested ${requestedQty}, available ${stock}`);
      } else {
        totalPrice += price * requestedQty;
      }
    }

    if (errors.length > 0) {
      return {
        isSuccess: false,
        message: "Inventory validation failed.",
        errors,
      };
    }
    return {
      isSuccess: true,
      message: "Inventory validated for the cart.",
      userId: event.userId,
      cart: event.cart,
      totalPrice,
    };
  } catch (err) {
    console.error("Error validating inventory:", err);
    return {
      isSuccess: false,
      message: "Unexpected error during inventory check.",
    };
  }
};

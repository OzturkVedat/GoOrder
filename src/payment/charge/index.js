exports.handler = async (event) => {
  console.log("Processing payment for order: ", JSON.stringify(event));

  const { userId, cart, totalPrice } = event;

  if (!userId || !cart || !totalPrice || totalPrice <= 0) {
    return {
      isSuccess: false,
      message: "Invalid payment request.",
      errors: ["Missing or invalid userId, cart or totalPrice"],
    };
  }

  await new Promise((resolve) => setTimeout(resolve, 300)); // simulating payment delay

  const failChance = Math.random();
  if (failChance < 0.1) {
    return {
      isSuccess: false,
      message: "Payment failed.",
      userId,
      cart,
      totalPrice,
    };
  }
  return {
    isSuccess: true,
    message: "Payment processed successfully.",
    userId,
    cart,
    totalPrice,
  };
};

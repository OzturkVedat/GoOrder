resource "aws_sqs_queue" "payment_queue" {
  name = "PaymentQueue"
}

resource "aws_sqs_queue" "inventory_queue" {
  name = "InventoryQueue"
}

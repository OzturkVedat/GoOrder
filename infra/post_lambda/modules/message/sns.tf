resource "aws_sns_topic" "payment_required" {
  name = "payment-required-topic"
}

resource "aws_sns_topic_subscription" "payment_required" { # sub payment service (via sqs) to listen sns topic (payment required)
  topic_arn = aws_sns_topic.payment_required.arn
  protocol  = "sqs"
  endpoint  = aws_sqs_queue.payment_service.arn

  raw_message_delivery = true # allow sns to send messages
}


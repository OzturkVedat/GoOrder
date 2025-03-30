resource "aws_sns_topic_subscription" "payment_required" {
  topic_arn = var.payment_req_topic_arn
  protocol  = "sqs"
  endpoint  = var.payment_queue_arn # sub payment service (via sqs) to listen sns topic (payment required)

  raw_message_delivery = true # allow sns to send messages
}

resource "aws_sns_topic_subscription" "payment_confirmed" {
  topic_arn            = var.pay_conf_topic_arn
  protocol             = "sqs"
  endpoint             = var.payment_queue_arn
  raw_message_delivery = true
}

resource "aws_lambda_permission" "allow_sns_to_invoke_lambda" {
  statement_id  = "AllowExeFromSNS"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.failed_payment.function_name
  principal     = "sns.amazonaws.com"
  source_arn    = var.pay_fail_topic_arn
}

resource "aws_sns_topic_subscription" "payment_failed" {
  topic_arn = var.pay_fail_topic_arn
  protocol  = "lambda"
  endpoint  = aws_lambda_function.failed_payment.arn
}

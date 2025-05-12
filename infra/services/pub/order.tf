data "aws_s3_object" "order_lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.order_notify_lambda_bucket_key
}

resource "aws_lambda_function" "order_notify" {
  function_name = "goorder-notify-order"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.order_notify_lambda_bucket_key
  source_code_hash = data.aws_s3_object.order_lambda_zip.etag

  environment {
    variables = {
      ORDER_TOPIC_ARN = var.order_events_topic_arn
    }
  }
}


output "order_notify_lambda_arn" {
  value = aws_lambda_function.order_notify.arn
}

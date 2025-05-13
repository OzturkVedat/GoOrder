data "aws_s3_object" "order_status_zip" {
  bucket = var.lambda_bucket_name
  key    = var.status_lambda_bucket_key
}

resource "aws_lambda_function" "order_status" {
  function_name = "goorder-order-status"
  role          = var.caller_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.status_lambda_bucket_key
  source_code_hash = data.aws_s3_object.order_status_zip.etag
}

resource "aws_lambda_event_source_mapping" "order_status_consumer" {
  event_source_arn = var.user_notif_queue_arn
  function_name    = aws_lambda_function.order_status.function_name
  batch_size       = 10
}

output "order_status_lambda_arn" {
  value = aws_lambda_function.order_status.arn
}

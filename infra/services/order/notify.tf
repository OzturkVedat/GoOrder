data "aws_s3_object" "notify_order_zip" {
  bucket = var.lambda_bucket_name
  key    = var.notify_lambda_bucket_key
}

resource "aws_lambda_function" "notify_order" {
  function_name = "goorder-notify-order"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.place_lambda_bucket_key
  source_code_hash = data.aws_s3_object.notify_order_zip.etag
}

resource "aws_lambda_event_source_mapping" "user_notify_consumer" {
  event_source_arn = var.user_notif_queue_arn
  function_name    = aws_lambda_function.notify_order.function_name
  batch_size       = 10
}

output "notify_order_lambda_arn" {
  value = aws_lambda_function.notify_order.arn
}

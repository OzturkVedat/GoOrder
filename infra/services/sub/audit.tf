data "aws_s3_object" "audit_log_zip" {
  bucket = var.lambda_bucket_name
  key    = var.audit_lambda_bucket_key
}

resource "aws_lambda_function" "audit_log" {
  function_name = "goorder-audit-log"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.audit_lambda_bucket_key
  source_code_hash = data.aws_s3_object.audit_log_zip.etag
}

resource "aws_lambda_event_source_mapping" "audit_log_consumer" {
  event_source_arn = var.audit_log_queue_arn
  function_name    = aws_lambda_function.audit_log.function_name
  batch_size       = 10
}

output "notify_order_lambda_arn" {
  value = aws_lambda_function.audit_log.arn
}

data "aws_s3_object" "lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.payment_lambda_bucket_key
}

resource "aws_lambda_function" "process_payment" {
  function_name = "GoOrderProcessPayment"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "PaymentService::PaymentService.Functions.ProcessPayment::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.payment_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}

resource "aws_lambda_function" "failed_payment" {
  function_name = "GoOrderFailedPayment"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "PaymentService::PaymentService.Functions.FailedPayment::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.payment_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}

data "aws_s3_object" "process_order_caller_zip" {
  bucket = var.lambda_bucket_name
  key    = var.process_order_caller_bucket_key
}

resource "aws_lambda_function" "process_order_caller" {
  function_name = "goorder-process-order-caller"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.process_order_caller_bucket_key
  source_code_hash = data.aws_s3_object.process_order_caller_zip.etag

  environment {
    variables = {
      PROCESS_ORDER_SM_ARN = var.process_order_sm_arn
    }
  }
}


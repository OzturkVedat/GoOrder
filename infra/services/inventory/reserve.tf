data "aws_s3_object" "reserve_lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.reserve_lambda_bucket_key
}

resource "aws_lambda_function" "reserve_inv" {
  function_name = "goorder-inventory-reserve"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.reserve_lambda_bucket_key
  source_code_hash = data.aws_s3_object.reserve_lambda_zip.etag

  environment {
    variables = {
      DYNAMO_TABLE_NAME = var.dynamo_table_name
    }
  }
}


output "reserve_inv_lambda_arn" {
  value = aws_lambda_function.reserve_inv.arn
}

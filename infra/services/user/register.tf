data "aws_s3_object" "register_zip" {
  bucket = var.lambda_bucket_name
  key    = var.register_lambda_bucket_key
}

resource "aws_lambda_function" "register" {
  function_name = "goorder-register-user"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.register_lambda_bucket_key
  source_code_hash = data.aws_s3_object.register_zip.etag

  environment {
    variables = {
      CLIENT_ID = var.client_id
    }
  }
}


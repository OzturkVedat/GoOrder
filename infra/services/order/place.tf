data "aws_s3_object" "place_order_zip" {
  bucket = var.lambda_bucket_name
  key    = var.place_lambda_bucket_key
}

resource "aws_lambda_function" "place_order" {
  function_name = "goorder-order-place"
  role          = var.lambda_exec_role_arn
  runtime       = "nodejs20.x"
  handler       = "index.handler"
  timeout       = var.lambda_timeout
  memory_size   = var.lambda_memory
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.place_lambda_bucket_key
  source_code_hash = data.aws_s3_object.place_order_zip.etag

  environment {
    variables = {
      DYNAMO_TABLE_NAME = var.dynamo_table_name
    }
  }
}


output "place_order_lambda_arn" {
  value = aws_lambda_function.place_order.arn
}

data "aws_s3_object" "lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.order_lambda_bucket_key
}

resource "aws_lambda_function" "place_order" {
  function_name = "GoOrderPlaceOrder"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "OrderService::OrderService.Functions.PlaceOrder::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.order_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}


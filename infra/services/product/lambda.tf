data "aws_s3_object" "lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.product_lambda_bucket_key
}

resource "aws_lambda_function" "add_product" {
  function_name = "GoOrderAddProduct"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "ProductService::ProductService.Functions.AddProduct::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.product_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}

resource "aws_lambda_function" "get_store_products" {
  function_name = "GoOrderGetStoreProducts"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "ProductService::ProductService.Functions.GetStoreProducts::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.product_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}

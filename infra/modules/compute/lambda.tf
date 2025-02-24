# create a lambda for api hosting
resource "aws_lambda_function" "order_processor" {
  function_name = "OrderProcessorLambda"
  role          = aws_iam_role.lambda_exe_role.arn
  runtime       = "dotnet8"
  timeout       = 30
  memory_size   = 512
  publish       = true

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.lambda_bucket_key
  source_code_hash = filebase64sha256(var.lambda_bucket_key) # specifying the source code hash for updates

  vpc_config {
    subnet_ids         = [var.private_subnet_id]
    security_group_ids = [var.lambda_sg_id]
  }
}

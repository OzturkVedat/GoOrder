resource "aws_lambda_function" "register_user" {
  function_name = "UserRegisterLambda"
  role          = aws_iam_role.lambda_exe_role.arn
  runtime       = "dotnet8"
  handler       = "UserService::UserService.UserLambda::RegisterUser"
  timeout       = 30
  memory_size   = 512
  publish       = false # true, if you want versioning

  s3_bucket = var.lambda_bucket_name
  s3_key    = var.user_lambda_bucket_key

  vpc_config {
    subnet_ids         = [var.private_subnet_id]
    security_group_ids = [var.lambda_sg_id]
  }
}

resource "aws_lambda_function" "login_user" {
  function_name = "UserLoginLambda"
  role          = aws_iam_role.lambda_exe_role.arn
  runtime       = "dotnet8"
  handler       = "UserService::UserService.UserLambda::LoginUser"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket = var.lambda_bucket_name
  s3_key    = var.user_lambda_bucket_key

  vpc_config {
    subnet_ids         = [var.private_subnet_id]
    security_group_ids = [var.lambda_sg_id]
  }
}

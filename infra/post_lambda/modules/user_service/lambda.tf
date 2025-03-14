resource "aws_lambda_function" "register_user" {
  function_name = "GoOrderUserRegister"
  role          = aws_iam_role.user_lambda_exe_role.arn
  runtime       = "dotnet8"
  handler       = "UserService.Functions::UserService.Functions.RegisterUser::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false # true, if you want versioning

  s3_bucket = var.lambda_bucket_name
  s3_key    = var.user_lambda_bucket_key
}

resource "aws_lambda_function" "login_user" {
  function_name = "UserLoginLambda"
  role          = aws_iam_role.user_lambda_exe_role.arn
  runtime       = "dotnet8"
  handler       = "UserService.Functions::UserService.Functions.LoginUSer::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket = var.lambda_bucket_name
  s3_key    = var.user_lambda_bucket_key
}

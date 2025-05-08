data "aws_s3_object" "lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.user_lambda_bucket_key
}

resource "aws_lambda_function" "register_user" {
  function_name = "GoOrderUserRegister"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "UserService::UserService.Functions.RegisterUser::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false # true, if you want versioning

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.user_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag # detect change when lambda is updated
}

resource "aws_lambda_function" "confirm_user" {
  function_name = "GoOrderUserConfirm"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "UserService::UserService.Functions.ConfirmUser::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.user_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}


resource "aws_lambda_function" "login_user" {
  function_name = "GoOrderUserLogin"
  role          = var.lambda_exec_role_arn
  runtime       = "dotnet8"
  handler       = "UserService::UserService.Functions.LoginUser::FunctionHandler"
  timeout       = 30
  memory_size   = 512
  publish       = false

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.user_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag
}

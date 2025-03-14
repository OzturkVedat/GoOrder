data "aws_s3_object" "lambda_zip" {
  bucket = var.lambda_bucket_name
  key    = var.user_lambda_bucket_key
}

locals {
  lambda_functions = [
    {
      function_name = "GoOrderUserRegister"
      handler       = "UserService::UserService.Functions.RegisterUser::FunctionHandler"
    },
    {
      function_name = "GoOrderUserConfirm"
      handler       = "UserService::UserService.Functions.ConfirmUser::FunctionHandler"
    },
    {
      function_name = "GoOrderUserLogin"
      handler       = "UserService::UserService.Functions.LoginUser::FunctionHandler"
    }
  ]
}

resource "aws_lambda_function" "this" {
  for_each = { for fn in local.lambda_functions : fn.function_name => fn }

  function_name = each.value.function_name
  role          = aws_iam_role.user_lambda_exe_role.arn
  runtime       = "dotnet8"
  handler       = each.value.handler
  timeout       = 30
  memory_size   = 512
  publish       = false # true, if you want versioning

  s3_bucket        = var.lambda_bucket_name
  s3_key           = var.user_lambda_bucket_key
  source_code_hash = data.aws_s3_object.lambda_zip.etag # detect change when lambda zip is updated
}


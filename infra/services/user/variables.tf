variable "lambda_bucket_name" {}

variable "register_lambda_bucket_key" {
  default = "user/register.zip"
}
variable "login_lambda_bucket_key" {
  default = "user/login.zip"
}
variable "verify_lambda_bucket_key" {
  default = "user/verify.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "apigw_id" {}
variable "apigw_exe_arn" {}

variable "client_id" {}

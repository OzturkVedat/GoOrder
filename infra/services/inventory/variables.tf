variable "lambda_bucket_name" {}

variable "reserve_lambda_bucket_key" {
  default = "inventory/reserve.zip"
}
variable "restore_lambda_bucket_key" {
  default = "inventory/restore.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "apigw_id" {}
variable "apigw_exe_arn" {}
variable "dynamo_table_name" {}

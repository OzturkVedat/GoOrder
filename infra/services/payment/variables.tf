variable "lambda_bucket_name" {}

variable "charge_lambda_bucket_key" {
  default = "payment/charge.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "dynamo_table_name" {}

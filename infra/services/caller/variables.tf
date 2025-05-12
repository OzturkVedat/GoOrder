variable "lambda_bucket_name" {}

variable "process_order_caller_bucket_key" {
  default = "caller/process_order.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "apigw_id" {}
variable "apigw_exe_arn" {}
variable "process_order_sm_arn" {}

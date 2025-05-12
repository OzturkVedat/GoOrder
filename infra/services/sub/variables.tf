variable "lambda_bucket_name" {}

variable "audit_lambda_bucket_key" {
  default = "sub/audit-log.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "apigw_id" {}
variable "apigw_exe_arn" {}
variable "audit_log_queue_arn" {}

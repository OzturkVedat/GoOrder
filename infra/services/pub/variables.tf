variable "lambda_bucket_name" {}

variable "order_notify_lambda_bucket_key" {
  default = "pub/order.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "apigw_id" {}
variable "apigw_exe_arn" {}
variable "order_events_topic_arn" {}

variable "lambda_bucket_name" {}

variable "order_pub_lambda_bucket_key" {
  default = "pub/order.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "order_events_topic_arn" {}

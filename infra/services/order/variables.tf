variable "lambda_bucket_name" {}

variable "place_lambda_bucket_key" {
  default = "order/place.zip"
}
variable "notify_lambda_bucket_key" {
  default = "order/notify.zip"
}

variable "lambda_exec_role_arn" {}
variable "lambda_timeout" {}
variable "lambda_memory" {}

variable "dynamo_table_name" {}
variable "user_notif_queue_arn" {}

variable "lambda_bucket_name" {
  description = "Name of the bucket containing lambda ZIPs"
  type        = string
  default     = "goorder-lambda-bucket"
}

variable "payment_lambda_bucket_key" {
  description = "Object key in the bucket (zip)"
  type        = string
  default     = "payment_lambda.zip"
}

variable "ssm_read_param_policy_arn" {}
variable "dynamodb_crud_policy_arn" {}

variable "payment_queue_arn" {}
variable "payment_req_topic_arn" {}
variable "pay_conf_topic_arn" {}
variable "pay_fail_topic_arn" {}

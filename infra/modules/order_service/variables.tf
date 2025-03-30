variable "lambda_bucket_name" {
  description = "Name of the bucket containing lambda ZIPs"
  type        = string
  default     = "goorder-lambda-bucket"
}

variable "order_lambda_bucket_key" {
  description = "Object key in the bucket (zip)"
  type        = string
  default     = "order_lambda.zip"
}

variable "apigw_id" {}
variable "apigw_exe_arn" {}

variable "ssm_read_param_policy_arn" {}
variable "dynamodb_crud_policy_arn" {}

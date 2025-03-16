variable "lambda_bucket_name" {
  description = "Name of the bucket containing lambda ZIPs"
  type        = string
  default     = "goorder-lambda-bucket"
}

variable "product_lambda_bucket_key" {
  description = "Object key in the bucket (zip)"
  type        = string
  default     = "product_lambda.zip"
}

variable "apigw_id" {}
variable "apigw_exe_arn" {}

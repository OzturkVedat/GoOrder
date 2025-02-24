variable "lambda_bucket_name" {
  description = "Name of the bucket containing lambda ZIPs"
  type        = string
  default     = "goorder-lambda-bucket"
}

variable "lambda_bucket_key" {
  description = "Object key in the bucket (zip)"
  type        = string
  default     = "lambda.zip"
}

variable "private_subnet_id" {}
variable "lambda_sg_id" {}

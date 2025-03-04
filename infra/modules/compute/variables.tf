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

variable "lambda_api_image_uri" {
  default = "962546904675.dkr.ecr.eu-north-1.amazonaws.com/goorder/api:latest"
}

variable "private_subnet_id" {}
variable "lambda_sg_id" {}

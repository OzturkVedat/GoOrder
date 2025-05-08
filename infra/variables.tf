variable "aws_region" {
  default = "eu-north-1"
}

variable "aws_az" {
  default = "eu-north-1a"
}

variable "parameter_path_prefix" {
  type        = string
  default     = "/goorder/"
  description = "Prefix path for all SSM parameters"
}

variable "lambda_default_mem" {
  default = 512
}

variable "lambda_default_timeout" {
  default = 30
}

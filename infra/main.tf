terraform {
  backend "s3" {
    bucket  = "goorder-tfstate-bucket"
    key     = "terraform.tfstate"
    region  = "eu-north-1"
    encrypt = true
  }
}

provider "aws" {
  # credentials will be fetched from env automatically
  #profile = "default"
  region = var.aws_region
}

module "network" {
  source = "./modules/network"

  aws_az_1                   = var.aws_az_1
  register_lambda_invoke_arn = module.lambda.register_lambda_invoke_arn
  register_lambda_func_name  = module.lambda.register_lambda_func_name
}

module "lambda" {
  source = "./modules/lambda"

  private_subnet_id = module.network.private_subnet_id
  lambda_sg_id      = module.network.lambda_sg_id
}

module "data" {
  source = "./modules/data"
}


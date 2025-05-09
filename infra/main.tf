terraform {
  backend "s3" {
    bucket  = "goorder-tfstate-bucket"
    key     = "terraform.tfstate"
    region  = "eu-north-1"
    encrypt = true
  }
}

provider "aws" {
  region = var.aws_region
}

module "auth" {
  source = "./domain/auth"

  parameter_path_prefix = var.parameter_path_prefix
}


module "storage" {
  source = "./domain/storage"
}

module "network" {
  source = "./platform/network"

  aws_region = var.aws_region

  user_pool_id  = module.auth.user_pool_id
  app_client_id = module.auth.app_client_id
}

module "access" {
  source = "./platform/access"

  aws_region            = var.aws_region
  dynamo_table_arn      = module.storage.dynamo_table_arn
  parameter_path_prefix = var.parameter_path_prefix
}

module "inventory_service" {
  source = "./services/inventory"

  apigw_id          = module.network.apigw_id
  apigw_exe_arn     = module.network.apigw_exe_arn
  dynamo_table_name = module.storage.dynamo_table_name

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

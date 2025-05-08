terraform {
  backend "s3" {
    bucket  = "goorder-tfstate-bucket"
    key     = "terraform.tfstate"
    region  = "eu-north-1"
    encrypt = true
  }
}

provider "aws" {
  # creds will be fetched from env
  #profile = "default"
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


module "user_service" {
  source = "./services/user"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn

  lambda_exec_role_arn = module.security.lambda_exec_role_arn
  lambda_mem_size      = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
}

module "product_service" {
  source = "./services/product"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn

  lambda_exec_role_arn = module.security.lambda_exec_role_arn
  lambda_mem_size      = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
}

module "order_service" {
  source = "./services/order"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn

  lambda_exec_role_arn = module.security.lambda_exec_role_arn
  lambda_mem_size      = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
}

module "payment_service" {
  source = "./services/payment"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn

  lambda_exec_role_arn = module.security.lambda_exec_role_arn
  lambda_mem_size      = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
}

module "flows" {
  source = "./flows"

  step_func_role_arn = module.access.step_func_role_arn
}

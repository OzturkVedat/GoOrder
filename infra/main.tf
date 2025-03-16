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

module "auth" {
  source = "./modules/auth"
}

module "network" {
  source = "./modules/network"

  aws_region = var.aws_region

  user_pool_id  = module.auth.user_pool_id
  app_client_id = module.auth.app_client_id
}

module "message" {
  source = "./modules/message"
}

module "storage" {
  source = "./modules/storage"
}

module "user_service" {
  source = "./modules/user_service"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn
}

module "product_service" {
  source = "./modules/product_service"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn
}

module "order_service" {
  source = "./modules/order_service"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn
}

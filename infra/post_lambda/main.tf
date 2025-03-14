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
}

module "user_service" {
  source = "./modules/user_service"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn
}

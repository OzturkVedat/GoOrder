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

module "event" {
  source = "./domain/event"
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
  parameter_path_prefix = var.parameter_path_prefix

  dynamo_table_arn = module.storage.dynamo_table_arn
}

module "inventory_service" {
  source = "./services/inventory"

  dynamo_table_name = module.storage.dynamo_table_name

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

module "payment_service" {
  source = "./services/payment"

  dynamo_table_name = module.storage.dynamo_table_name

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

module "order_service" {
  source = "./services/order"

  dynamo_table_name    = module.storage.dynamo_table_name
  user_notif_queue_arn = module.event.user_notif_queue_arn

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

module "pub_service" {
  source = "./services/pub"

  order_events_topic_arn = module.event.order_events_topic_arn

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

module "sub_service" {
  source = "./services/sub"

  audit_log_queue_arn = module.event.audit_log_queue_arn

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

module "user" {
  source = "./services/user"

  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn

  client_id = module.auth.app_client_id

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn

}

module "flows" {
  source = "./flows"

  aws_region = var.aws_region

  reserve_inv_arn    = module.inventory_service.reserve_inv_lambda_arn
  restore_inv_arn    = module.inventory_service.restore_inv_lambda_arn
  charge_payment_arn = module.payment_service.charge_lambda_arn
  place_order_arn    = module.order_service.place_order_lambda_arn
  publish_order_arn  = module.pub_service.order_pub_lambda_arn
}

module "caller" {
  source = "./services/caller"

  apigw_id      = module.network.apigw_id
  apigw_exe_arn = module.network.apigw_exe_arn
  authorizer_id = module.network.authorizer_id

  process_order_sm_arn = module.flows.process_order_sm_arn
  lambda_bucket_name   = var.lambda_bucket_name
  lambda_memory        = var.lambda_default_mem
  lambda_timeout       = var.lambda_default_timeout
  lambda_exec_role_arn = module.access.lambda_exec_role_arn
}

output "apigw_url" {
  value = module.network.apigw_url
}

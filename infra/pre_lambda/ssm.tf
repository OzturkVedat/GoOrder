resource "aws_ssm_parameter" "user_pool_id" {
  name  = "/goorder/user-pool-id"
  type  = "SecureString"
  value = var.user_pool_id

  tags = {
    Name = "GoOrderUserPoolId"
  }
}

resource "aws_ssm_parameter" "app_client_id" {
  name  = "/goorder/app-client-id"
  type  = "SecureString"
  value = var.app_client_id

  tags = {
    Name = "GoOrderUserPoolId"
  }
}

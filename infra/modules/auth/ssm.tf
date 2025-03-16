resource "aws_ssm_parameter" "user_pool_id" {
  name  = "/goorder/user-pool-id"
  type  = "SecureString"
  value = aws_cognito_user_pool.user_pool.id

  tags = {
    Name = "GoOrderUserPoolId"
  }
}

resource "aws_ssm_parameter" "app_client_id" {
  name  = "/goorder/app-client-id"
  type  = "SecureString"
  value = aws_cognito_user_pool_client.app_client.id

  tags = {
    Name = "GoOrderAppClientId"
  }
}

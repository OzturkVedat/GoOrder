resource "aws_ssm_parameter" "app_client_id" {
  name  = "/goorder/app-client-id"
  type  = "SecureString"
  value = var.app_client_id

  tags = {
    Name = "GoOrderAppClientId"
  }
}

resource "aws_ssm_parameter" "app_client_secret" {
  name  = "/goorder/app-client-secret"
  type  = "SecureString"
  value = var.app_client_secret

  tags = {
    Name = "GoOrderAppClientSecret"
  }
}

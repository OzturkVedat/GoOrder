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

resource "aws_ssm_parameter" "pay_req_topic_arn" {
  name  = "/goorder/topic-arn/payment-required"
  type  = "SecureString"
  value = var.pay_req_topic_arn
  tags = {
    Name = "GoOrderPaymentRequiredTopicArn"
  }
}

resource "aws_ssm_parameter" "pay_req_topic_arn" {
  name  = "/goorder/topic-arn/payment-required"
  type  = "SecureString"
  value = aws_sns_topic.payment_required.arn

  tags = {
    Name = "GoOrderPaymentRequiredTopicArn"
  }
}

resource "aws_ssm_parameter" "pay_conf_topic_arn" {
  name  = "/goorder/topic-arn/payment-confirmed"
  type  = "SecureString"
  value = aws_sns_topic.payment_required.arn

  tags = {
    Name = "GoOrderPaymentRequiredTopicArn"
  }
}

resource "aws_ssm_parameter" "pay_fail_topic_arn" {
  name  = "/goorder/topic-arn/payment-failed"
  type  = "SecureString"
  value = aws_sns_topic.payment_failed.arn

  tags = {
    Name = "GoOrderPaymentFailedTopicArn"
  }
}

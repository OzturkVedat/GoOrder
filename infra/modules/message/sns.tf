resource "aws_sns_topic" "payment_required" {
  name = "goorder-payment-required"
}

resource "aws_sns_topic" "payment_confirmed" {
  name = "goorder-payment-confirmed"
}

resource "aws_sns_topic" "payment_failed" {
  name = "goorder-payment-failed"
}


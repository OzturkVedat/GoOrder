output "payment_queue_arn" {
  value = aws_sqs_queue.payment_service.arn
}

output "payment_req_topic_arn" {
  value = aws_sns_topic.payment_required.arn
}

output "pay_conf_topic_arn" {
  value = aws_sns_topic.payment_confirmed.arn
}

output "pay_fail_topic_arn" {
  value = aws_sns_topic.payment_failed.arn
}

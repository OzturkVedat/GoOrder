output "order_events_topic_arn" {
  value = aws_sns_topic.order_events.arn
}

output "user_notif_queue_arn" {
  value = aws_sqs_queue.order_queues["user_notifications"].arn
}

output "audit_log_queue_arn" {
  value = aws_sqs_queue.order_queues["audit_log"].arn
}

output "shipment_queue_arn" {
  value = aws_sqs_queue.order_queues["shipment"].arn
}

locals {
  order_sqs_queues = {
    user_notifications = "goorder-user-notifications"
    audit_log          = "goorder-audit-log"
    shipment           = "goorder-shipment"
  }
}

resource "aws_sqs_queue" "order_queues" {
  for_each = local.order_sqs_queues
  name     = each.value
}

resource "aws_sns_topic_subscription" "order_events_subs" {
  for_each = aws_sqs_queue.order_queues

  topic_arn            = aws_sns_topic.order_events.arn
  protocol             = "sqs"
  endpoint             = each.value.arn
  raw_message_delivery = true
}

resource "aws_sqs_queue_policy" "allow_sns" {
  for_each = aws_sqs_queue.order_queues

  queue_url = each.value.id
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Effect    = "Allow",
      Principal = "*",
      Action    = "sqs:SendMessage",
      Resource  = each.value.arn,
      Condition = {
        ArnEquals = {
          "aws:SourceArn" = aws_sns_topic.order_events.arn
        }
      }
    }]
  })
}



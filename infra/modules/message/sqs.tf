resource "aws_sqs_queue" "order_service" {
  name = "order-service-queue"
}

resource "aws_sqs_queue" "payment_service" {
  name = "payment-service-queue"
}

data "aws_iam_policy_document" "sqs_policy" {
  statement {
    actions = ["SQS:SendMessage"]

    resources = [
      aws_sqs_queue.order_service.arn,
      aws_sqs_queue.payment_service.arn
    ]

    principals {
      type        = "Service"
      identifiers = ["sns.amazonaws.com"]
    }
  }
}

resource "aws_sqs_queue_policy" "order_qp" {
  queue_url = aws_sqs_queue.order_service.url
  policy    = data.aws_iam_policy_document.sqs_policy
}

resource "aws_sqs_queue_policy" "payment_qp" {
  queue_url = aws_sqs_queue.payment_service.url
  policy    = data.aws_iam_policy_document.sqs_policy.json
}



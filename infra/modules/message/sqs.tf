resource "aws_sqs_queue" "payment_service" {
  name = "payment-service-queue"
}

resource "aws_sqs_queue_policy" "payment_qp" {
  queue_url = aws_sqs_queue.payment_service.url
  policy    = data.aws_iam_policy_document.sqs_policy.json
}

data "aws_iam_policy_document" "sqs_policy" {
  statement {
    actions = ["SQS:SendMessage"]

    resources = [
      aws_sqs_queue.payment_service.arn
    ]

    principals {
      type        = "Service"
      identifiers = ["sns.amazonaws.com"]
    }
  }
}

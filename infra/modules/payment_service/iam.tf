data "aws_iam_policy_document" "lambda_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "payment_lambda_exe_role" {
  name               = "payment-service-lambda-execution-role"
  assume_role_policy = data.aws_iam_policy_document.lambda_assume_role_policy.json
}

resource "aws_iam_role_policy_attachment" "policy_attachments" {
  for_each = toset([
    "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess",
    "arn:aws:iam::aws:policy/AmazonSNSFullAccess",
    "arn:aws:iam::aws:policy/AmazonSQSFullAccess"
  ])
  role       = aws_iam_role.payment_lambda_exe_role.name
  policy_arn = each.value
}

resource "aws_iam_role_policy_attachment" "dynamo_crud" {
  role       = aws_iam_role.payment_lambda_exe_role.name
  policy_arn = var.dynamodb_crud_policy_arn
}

resource "aws_iam_role_policy_attachment" "ssm_param_read" {
  role       = aws_iam_role.payment_lambda_exe_role.name
  policy_arn = var.ssm_read_param_policy_arn
}

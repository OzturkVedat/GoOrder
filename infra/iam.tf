resource "aws_iam_policy" "ssm_read_goorder_params" {
  name = "goorder-ssm-read-params"

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "ssm:GetParameter",
          "ssm:GetParameters",
          "ssm:GetParametersByPath"
        ],
        Resource = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/goorder/*"
      },
      {
        Effect = "Allow",
        Action = [
          "kms:Decrypt"
        ],
        Resource = "*"
      }
    ]
  })
}


resource "aws_iam_policy" "dynamodb_table_crud" {
  name = "goorder-dynamodb-put-item"

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "dynamodb:PutItem",
          "dynamodb:GetItem", # single item
          "dynamodb:Query",   # multiple items
          "dynamodb:Scan",    # entire table
          "dynamodb:UpdateItem",
          "dynamodb:DeleteItem"
        ],
        Resource = module.storage.dynamo_table_arn
      }
    ]
  })
}

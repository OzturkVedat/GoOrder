resource "aws_iam_role" "lambda_exe_role" {
  name = "LambdaExecutionRole"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_policy" "s3_access_policy" {
  name        = "LambdaS3AccessPolicy"
  description = "Policy to allow Lambda function to access S3 bucket"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "s3:GetObject",
          "s3:ListBucket",
          "s3:PutObject"
        ]
        Resource = [
          "arn:aws:s3:::${var.lambda_bucket_name}",
          "arn:aws:s3:::${var.lambda_bucket_name}/${var.lambda_bucket_key}"
        ]
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "attach_s3_access" {
  policy_arn = aws_iam_policy.s3_access_policy.arn
  role       = aws_iam_role.lambda_exe_role.name
}

resource "aws_iam_role_policy_attachment" "attach_vpc_access" {
  role       = aws_iam_role.lambda_exe_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role" "step_function_role" {
  name = "GoOrderStepFunctionRole"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = {
        Service = "states.${var.aws_region}.amazonaws.com"
      }
      Action = "sts:AssumeRole"
    }]
  })
}

resource "aws_iam_role_policy" "step_function_policy" {
  name = "GoOrderStepFunctionPolicy"
  role = aws_iam_role.step_function_role.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "lambda:InvokeFunction"
        ]
        Resource = [
          var.reserve_inv_arn,
          var.restore_inv_arn,
          var.charge_payment_arn,
          var.place_order_arn,
          var.publish_order_arn
        ]
      }
    ]
  })
}

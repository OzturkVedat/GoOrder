output "lambda_exec_role_arn" {
  value = aws_iam_role.lambda_role.arn
}

output "caller_exec_role_arn" {
  value = aws_iam_role.caller_role.arn
}

output "register_lambda_invoke_arn" {
  value = aws_lambda_function.register_user.invoke_arn
}

output "register_lambda_func_name" {
  value = aws_lambda_function.register_user.function_name
}

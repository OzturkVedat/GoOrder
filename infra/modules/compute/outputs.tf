output "order_lambda_invoke_arn" {
  value = aws_lambda_function.user_service.invoke_arn
}

output "order_lambda_func_name" {
  value = aws_lambda_function.user_service.function_name
}

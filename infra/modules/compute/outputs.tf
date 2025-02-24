output "order_lambda_invoke_arn" {
  value = aws_lambda_function.order_processing.invoke_arn
}

output "order_lambda_func_name" {
  value = aws_lambda_function.order_processing.function_name
}


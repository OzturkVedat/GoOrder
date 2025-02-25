output "order_lambda_invoke_arn" {
  value = aws_lambda_function.order_processor.invoke_arn
}

output "order_lambda_func_name" {
  value = aws_lambda_function.order_processor.function_name
}

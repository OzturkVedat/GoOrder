output "apigw_id" {
  value = aws_apigatewayv2_api.goorder_api.id
}

output "apigw_exe_arn" {
  value = aws_apigatewayv2_api.goorder_api.execution_arn
}

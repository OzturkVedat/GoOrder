output "apigw_id" {
  value     = aws_apigatewayv2_api.goorder_api.id
  sensitive = true
}

output "apigw_exe_arn" {
  value     = aws_apigatewayv2_api.goorder_api.execution_arn
  sensitive = true
}

output "apigw_url" {
  value = aws_apigatewayv2_api.goorder_api.api_endpoint
}

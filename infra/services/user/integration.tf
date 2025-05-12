resource "aws_apigatewayv2_integration" "user_registration" {
  api_id                 = var.apigw_id
  integration_type       = "AWS_PROXY"
  integration_uri        = aws_lambda_function.register.arn
  integration_method     = "POST"
  payload_format_version = "2.0"
}

resource "aws_apigatewayv2_route" "user_registration" {
  api_id    = var.apigw_id
  route_key = "POST /"
  target    = "integrations/${aws_apigatewayv2_integration.user_registration.id}"
}

resource "aws_lambda_permission" "apigw_invoke" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.register.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${var.apigw_exe_arn}/*/*"
}

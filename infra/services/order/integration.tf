resource "aws_apigatewayv2_integration" "order_status" {
  api_id                 = var.apigw_id
  integration_type       = "AWS_PROXY"
  integration_uri        = aws_lambda_function.order_status.arn
  integration_method     = "POST"
  payload_format_version = "2.0"
}

resource "aws_apigatewayv2_route" "order_status" {
  api_id    = var.apigw_id
  route_key = "GET /order/status/{executionArn}"
  target    = "integrations/${aws_apigatewayv2_integration.order_status.id}"

  authorizer_id      = var.authorizer_id
  authorization_type = "JWT"
}

resource "aws_lambda_permission" "order_status" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.order_status.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${var.apigw_exe_arn}/*/GET/order/status/*"
}

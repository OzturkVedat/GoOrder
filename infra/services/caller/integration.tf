resource "aws_apigatewayv2_integration" "process_order_caller" {
  api_id                 = var.apigw_id
  integration_type       = "AWS_PROXY"
  integration_uri        = aws_lambda_function.process_order_caller.arn
  integration_method     = "POST"
  payload_format_version = "2.0"
}

resource "aws_apigatewayv2_route" "process_order_caller" {
  api_id    = var.apigw_id
  route_key = "POST /process-order"
  target    = "integrations/${aws_apigatewayv2_integration.process_order_caller.id}"

  authorizer_id      = var.authorizer_id
  authorization_type = "JWT"
}

resource "aws_lambda_permission" "apigw_invoke" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.process_order_caller.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${var.apigw_exe_arn}/*/POST/process-order"
}

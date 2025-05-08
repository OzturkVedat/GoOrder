locals {
  lambda_endpoints = {
    place_order = {
      lambda_function = aws_lambda_function.place_order
      route_key       = "POST /order/place"
      permission_sid  = "AllowAPIGatewayInvokeRegister"
    }
    // add more
  }
}

resource "aws_apigatewayv2_integration" "lambda_integrations" {
  for_each               = local.lambda_endpoints
  api_id                 = var.apigw_id
  integration_type       = "AWS_PROXY"
  integration_uri        = each.value.lambda_function.arn
  payload_format_version = "2.0"
}

resource "aws_apigatewayv2_route" "lambda_routes" {
  for_each  = local.lambda_endpoints
  api_id    = var.apigw_id
  route_key = each.value.route_key
  target    = "integrations/${aws_apigatewayv2_integration.lambda_integrations[each.key].id}"
}

resource "aws_lambda_permission" "lambda_permissions" {
  for_each      = local.lambda_endpoints
  statement_id  = each.value.permission_sid
  action        = "lambda:InvokeFunction"
  function_name = each.value.lambda_function.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${var.apigw_exe_arn}/*/*"
}

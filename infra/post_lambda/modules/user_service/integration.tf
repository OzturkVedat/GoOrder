locals {
  lambda_endpoints = {
    register = {
      lambda_function = aws_lambda_function.register_user
      route_key       = "POST /register-user"
      permission_sid  = "AllowAPIGatewayInvokeRegister"
    }
    login = {
      lambda_function = aws_lambda_function.login_user
      route_key       = "POST /login-user"
      permission_sid  = "AllowAPIGatewayInvokeLogin"
    }
    confirm = {
      lambda_function = aws_lambda_function.confirm_user
      route_key       = "POST /confirm-email"
      permission_sid  = "AllowAPIGatewayInvokeLogin"
    }
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

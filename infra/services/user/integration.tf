locals {
  user_routes = {
    register = {
      path   = "/user/register"
      method = "POST"
      lambda = aws_lambda_function.register
    }
    verify = {
      path   = "/user/verify"
      method = "POST"
      lambda = aws_lambda_function.verify
    }
    login = {
      path   = "/user/login"
      method = "POST"
      lambda = aws_lambda_function.login
    }
  }
}

resource "aws_apigatewayv2_integration" "user_routes" {
  for_each = local.user_routes

  api_id                 = var.apigw_id
  integration_type       = "AWS_PROXY"
  integration_uri        = each.value.lambda.arn
  integration_method     = each.value.method
  payload_format_version = "2.0"
}

resource "aws_apigatewayv2_route" "user_routes" {
  for_each = local.user_routes

  api_id    = var.apigw_id
  route_key = "${each.value.method} ${each.value.path}"
  target    = "integrations/${aws_apigatewayv2_integration.user_routes[each.key].id}"
}

resource "aws_lambda_permission" "user_routes" {
  for_each = local.user_routes

  statement_id  = "AllowInvoke${title(each.key)}"
  action        = "lambda:InvokeFunction"
  function_name = each.value.lambda.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${var.apigw_exe_arn}/*/${each.value.method}${each.value.path}"
}


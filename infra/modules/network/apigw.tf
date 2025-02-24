resource "aws_apigatewayv2_api" "goorder_api" {
  name          = "GoOrderAPI"
  description   = "API Gateway for Order Processing"
  protocol_type = "HTTP"
}

# link the lambda to apigw
resource "aws_apigatewayv2_integration" "lambda_integration" {
  api_id           = aws_apigatewayv2_api.goorder_api.id
  integration_type = "AWS_PROXY"
  integration_uri  = var.order_lambda_invoke_arn
}

# define a route to handle all http methods
resource "aws_apigatewayv2_route" "root_route" {
  api_id    = aws_apigatewayv2_api.goorder_api.id
  route_key = "ANY /{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.lambda_integration.id}"
}

# create stage and enable auto-deploy
resource "aws_apigatewayv2_stage" "prod" {
  api_id      = aws_apigatewayv2_api.goorder_api.id
  name        = "prod"
  auto_deploy = true
}

# grant apigw permission to invoke lambda function
resource "aws_lambda_permission" "apigw_lambda" {
  action        = "lambda:InvokeFunction"
  function_name = var.order_lambda_func_name
  principal     = "apigateway.amazonaws.com"

  source_arn = "${aws_apigatewayv2_api.goorder_api.execution_arn}/*/*"
}

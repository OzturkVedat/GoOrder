resource "aws_apigatewayv2_api" "goorder_api" {
  name          = "goorder-apigw"
  description   = "API Gateway for Order Processing"
  protocol_type = "HTTP"

  tags = {
    Name = "goorder-api-gateway"
  }
}

# create stage and enable auto-deploy
resource "aws_apigatewayv2_stage" "dev" {
  api_id      = aws_apigatewayv2_api.goorder_api.id
  name        = "dev"
  auto_deploy = true
}


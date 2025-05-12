resource "aws_apigatewayv2_api" "goorder_api" {
  name          = "goorder-apigw"
  protocol_type = "HTTP"

  tags = {
    Name = "goorder-api-gateway"
  }
}

resource "aws_apigatewayv2_authorizer" "cognito_jwt_auth" {
  api_id           = aws_apigatewayv2_api.goorder_api.id
  authorizer_type  = "JWT"
  identity_sources = ["$request.header.Authorization"]

  name = "GoOrderCognitoAuthorizer"

  jwt_configuration {
    audience = [var.app_client_id]
    issuer   = "https://cognito-idp.${var.aws_region}.amazonaws.com/${var.user_pool_id}"
  }
}

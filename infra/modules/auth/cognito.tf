resource "aws_cognito_user_pool" "user_pool" {
  name = "goorder-user-pool"

  username_attributes      = ["email"] # Users will sign in with email
  auto_verified_attributes = ["email"]

  password_policy {
    minimum_length    = 8
    require_lowercase = true
    require_uppercase = true
    require_numbers   = true
    require_symbols   = false
  }

  admin_create_user_config {
    allow_admin_create_user_only = false # Users can sign up themselves
  }

  schema {
    name                = "email"
    attribute_data_type = "String"
    required            = true
    mutable             = false
  }

  tags = {
    Name = "GoOrderUserPool"
  }
}

resource "aws_cognito_user_pool_client" "app_client" {
  name         = "goorder-app-client"
  user_pool_id = aws_cognito_user_pool.user_pool.id

  generate_secret     = false # jwt auth does not require a secret
  explicit_auth_flows = ["ALLOW_USER_PASSWORD_AUTH", "ALLOW_REFRESH_TOKEN_AUTH"]

  prevent_user_existence_errors = "ENABLED"
}


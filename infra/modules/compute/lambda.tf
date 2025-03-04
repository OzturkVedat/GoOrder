# create a lambda for api hosting
resource "aws_lambda_function" "order_service" {
  function_name = "OrderServiceLambda"
  role          = aws_iam_role.lambda_exe_role.arn
  package_type  = "Image"
  image_uri     = var.lambda_api_image_uri
  timeout       = 30
  memory_size   = 512
  publish       = false # true, if you want versioning

  vpc_config {
    subnet_ids         = [var.private_subnet_id]
    security_group_ids = [var.lambda_sg_id]
  }
}

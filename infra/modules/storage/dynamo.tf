resource "aws_dynamodb_table" "orders" {
  name         = "Orders"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "OrderId" # PK

  attribute {
    name = "OrderId"
    type = "S"
  }
  attribute {
    name = "UserId"
    type = "S"
  }

  global_secondary_index {
    name            = "UserIdIndex" # for querying orders by user
    hash_key        = "UserId"
    projection_type = "ALL"
  }

  tags = {
    Name       = "GoOrder_OrdersTable"
    Enviroment = "dev"
  }
}

resource "aws_dynamodb_table" "products" {
  name         = "Products"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "ProductId"

  attribute {
    name = "ProductId"
    type = "S"
  }
  attribute {
    name = "StoreId"
    type = "S"
  }

  global_secondary_index {
    name            = "StoreIdIndex"
    hash_key        = "StoreId"
    projection_type = "ALL"
  }

  tags = {
    Name       = "GoOrder_ProductsTable"
    Enviroment = "dev"
  }
}

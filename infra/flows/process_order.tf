resource "aws_sfn_state_machine" "process_order" {
  name     = "GoOrderProcessOrderSM"
  role_arn = aws_iam_role.step_function_role.arn

  definition = jsonencode({
    Comment = "Order processing flow for GoOrder"
    StartAt = "ReserveInventory"

    States = {
      ReserveInventory = {
        Type     = "Task"
        Resource = var.reserve_inv_arn
        Next     = "CheckInventoryResult"
        Catch = [{
          ErrorEquals = ["States.ALL"]
          ResultPath  = "$.error"
          Next        = "RestoreInventory"
        }]
      }
      RestoreInventory = {
        Type     = "Task"
        Resource = var.reserve_inv_arn
        Next     = "CheckInventoryResult"
        Catch = [{
          ErrorEquals = ["States.ALL"]
          ResultPath  = "$.error"
          Next        = "SendFailureNotification"
        }]
      }
      CheckInventoryResult = {
        Type = "Choice"
        Choices = [
          {
            Variable      = "$.isSuccess"
            BooleanEquals = true
            Next          = "ChargePayment"
          }
        ]
        Default = "RestoreInventory"
      }
      ChargePayment = {
        Type     = "Task"
        Resource = var.charge_payment_arn
        Next     = "CreateOrder"
        Catch = [{
          ErrorEquals = ["States.ALL"]
          Next        = "RestoreInventory"
        }]
      }
      CreateOrder = {
        Type     = "Task"
        Resource = var.place_order_arn
        Next     = "SendSuccessNotification"
      }
      SendSuccessNotification = {
        Type     = "Task"
        Resource = var.notify_order_arn
        Parameters = {
          "userId.$" : "$.userId",
          "orderId.$" : "$.orderId",
          "status" : "success"
        }
        End = true
      }
      SendFailureNotification = {
        Type     = "Task"
        Resource = var.notify_order_arn
        Parameters = {
          "userId.$" : "$.userId",
          "orderId.$" : "$.orderId",
          "status" : "failed"
        }
        End = true
      }
    }
  })
}

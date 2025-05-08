resource "aws_sfn_state_machine" "place_order" {
  name     = "GoOrderOrderPlacementSM"
  role_arn = aws_iam_role.step_function_role.arn

  definition = jsonencode({
    Comment = "Order placement flow for GoOrder"
    StartAt = "ValidateInventory"

    States = {
      ValidateInventory = {
        Type     = "Task"
        Resource = var.validate_inventory_arn
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
        Default = "SendFailureNotification"
      }

      ChargePayment = {
        Type     = "Task"
        Resource = var.charge_payment_arn
        Next     = "CreateOrder"
        Catch = [{
          ErrorEquals = ["States.ALL"]
          Next        = "SendFailureNotification"
        }]
      }
      CreateOrder = {
        Type     = "Task"
        Resource = var.create_order_arn
        Next     = "SendSuccessNotification"
      }
      SendSuccessNotification = {
        Type     = "Task"
        Resource = var.send_notification_arn
        End      = true
      }
      SendFailureNotification = {
        Type     = "Task"
        Resource = var.send_notification.arn
        End      = true
      }
    }
  })
}

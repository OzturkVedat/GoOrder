output "private_subnet_id" {
  value = aws_subnet.private_subnet.id
}

output "lambda_sg_id" {
  value = aws_security_group.lambda_sg.id
}

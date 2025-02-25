resource "aws_vpc" "goorder_vpc" {
  cidr_block = "17.0.0.0/16"
}

resource "aws_subnet" "private_subnet" {
  vpc_id                  = aws_vpc.goorder_vpc.id
  cidr_block              = "17.0.1.0/24"
  map_public_ip_on_launch = false
  availability_zone       = var.aws_az_1
}

resource "aws_security_group" "lambda_sg" {
  vpc_id = aws_vpc.goorder_vpc.id

  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

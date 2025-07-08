# GoOrder - Order Processing Implementation

This is a simple serverless backend for processing orders. It's built with **AWS Lambda**, **Step Functions**, **SNS**/**SQS** and **Cognito**. The API is exposed through **HTTP APIGW** and uses JWT for auth.

---

## Features

- Secure API access with **JWT** via **Cognito**
- Orchestration via **Step Functions**
- Event-driven communication using **SNS** and **SQS**
- IaC with **Terraform**
- CI/CD using **GitHub Actions**
- Extensible for new workflows and services

---

# Simplified Diagram

![Architecture Diagram](docs/goorder-diagram.png)

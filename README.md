# ECommerce API

Production-grade E-Commerce REST API built with .NET 9 Clean Architecture.

## 🏗️ Architecture
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS** with MediatR
- **JWT Authentication** + Refresh Tokens
- **Redis Caching**
- **Azure Deployment**

## 🛠️ Tech Stack
- .NET 9
- Entity Framework Core
- SQL Server
- Redis Cache
- Docker
- Azure Container Apps

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- SQL Server (via Docker)

### Run Locally
```bash
# Start dependencies
docker compose up -d

# Run API
dotnet run --project src/ECommerce.API
```

## 📡 API Endpoints

### Auth
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token

### Products
- GET /api/products
- GET /api/products/{id}
- POST /api/products (Admin only)

### Categories
- GET /api/categories
- POST /api/categories (Admin only)

## ☁️ Azure Resources
- Azure Container Apps
- Azure SQL Database
- Azure Redis Cache
- Azure Key Vault
- Azure Container Registry


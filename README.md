# eCommerceSolution.OrdersService

An event-driven checkout and order processing microservice built with **.NET Core**, utilizing document-based **MongoDB storage** to handle highly flexible, non-relational order invoices and histories.

## 🛠️ Tech Stack & Infrastructure
* **Framework:** .NET Core API
* **Database:** MongoDB (NoSQL document store optimized for flexible, historic order schemas)
* **Containerization:** Docker & Docker Compose
* **Dependencies:** Depends explicitly on `ecommerce-users-microservice` and `ecommerce-products-microservice` to complete transactional checkout steps.

## 🏗️ Architecture Role & Data Flow
This service coordinates transaction workflows. Because order structures vary over time (dynamic discounts, multi-item checkouts, historical tax rates), a flexible document store was chosen.
* **Inter-Service Communication:** Uses the `inter-service-network` to make secure API requests back to the **Product Service** (for inventory verification) and the **User Service** (for shipping and billing verification) before finalizing a state change in MongoDB.

## 📂 System Architecture Overview
This repository is part of a larger, decentralized microservice ecosystem:
1. **[UsersService](https://github.com/B3nzy/eCommerceSolution.UsersService)** (PostgreSQL)
2. **[ProductsService](https://github.com/B3nzy/eCommerceSolution.ProductsService)** (MS SQL Server + Redis)
3. **[OrdersService](https://github.com/B3nzy/eCommerceSolution.OrdersService)** (MongoDB) - *You are here*

## 🚀 How to Run (via Orchestrated Compose)
To run this service alongside the entire ecosystem, navigate to the root configuration containing the `docker-compose.yml` file and execute:
```bash
docker-compose up --build

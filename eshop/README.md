# eShop API

A microservices-based e-commerce backend built with ASP.NET Core 8, following Clean Architecture and CQRS patterns.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                   YARP API Gateway                      │
│                  (Port 6004 / 6064)                     │
│   /identity-service  /catalog-service                   │
│   /basket-service    /ordering-service                  │
└────────┬──────────┬──────────┬──────────┬───────────────┘
         │          │          │          │
    ┌────┴───┐ ┌────┴───┐ ┌────┴───┐ ┌────┴────┐
    │Identity│ │Catalog │ │Basket  │ │Ordering │
    │  API   │ │  API   │ │  API   │ │  API    │
    │ :6005  │ │ :6000  │ │ :6001  │ │ :6003   │
    └────────┘ └────────┘ └────────┘ └─────────┘
    PostgreSQL  PostgreSQL  PostgreSQL   MSSQL
                            + Redis
                         Discount gRPC
                            (:6002)
```

## Services

| Service | Port (HTTP) | Port (HTTPS) | Database |
|---------|-------------|--------------|----------|
| Identity API | 6005 | 6005 | PostgreSQL (identitydb:5434) |
| Catalog API | 6000 | 6060 | PostgreSQL (catalogdb:5432) |
| Basket API | 6001 | 6061 | PostgreSQL (basketdb:5433) + Redis |
| Discount gRPC | 6002 | 6062 | SQLite |
| Ordering API | 6003 | 6063 | MSSQL (orderdb:1433) |
| YARP Gateway | 6004 | 6064 | — |

## Service Endpoints

### Identity API (`http://localhost:6005`)

**Authentication (OpenID Connect)**

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/connect/token` | POST | — | Issue access/refresh token |
| `/connect/authorize` | GET/POST | — | Authorization endpoint |
| `/connect/userinfo` | GET | Bearer | Get current user claims |
| `/connect/logout` | POST | — | Revoke session |

**User Management**

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/users/register` | POST | — | Register new user |
| `/users` | GET | `users:write` | List all users |
| `/users/{id}` | GET | Bearer | Get user by ID |
| `/users/{id}` | PUT | `users:write` | Update user info |
| `/users/{id}` | DELETE | `users:write` | Delete user |
| `/users/{id}/roles/{roleName}` | POST | `users:write` | Assign role to user |
| `/users/{id}/roles/{roleName}` | DELETE | `users:write` | Remove role from user |
| `/users/{id}/permissions` | GET | `users:write` | Get user permissions |
| `/users/{id}/permissions` | POST | `users:write` | Add permission to user |
| `/users/{id}/permissions/{permission}` | DELETE | `users:write` | Remove permission from user |

**Role Management**

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/roles` | GET | `users:write` | List all roles |
| `/roles` | POST | `users:write` | Create role |
| `/roles/{id}` | DELETE | `users:write` | Delete role |
| `/roles/{id}/permissions` | POST | `users:write` | Add permission to role |
| `/roles/{id}/permissions/{permission}` | DELETE | `users:write` | Remove permission from role |

---

### Catalog API (`http://localhost:6000`)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/products` | GET | List all products (paginated) |
| `/products/{id}` | GET | Get product by ID |
| `/products/category/{category}` | GET | Get products by category |
| `/products` | POST | Create product |
| `/products` | PUT | Update product |
| `/products/{id}` | DELETE | Delete product |

---

### Basket API (`http://localhost:6001`)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/basket/{userName}` | GET | Get shopping cart |
| `/basket` | POST | Create or update shopping cart |
| `/basket/{userName}` | DELETE | Delete shopping cart |
| `/basket/checkout` | POST | Checkout basket — accepts optional `couponCode` for order discount |

---

### Discount Service (`http://localhost:6002`)

Exposes both REST (admin/user) and gRPC (internal).

**Product Discounts — REST (Admin)**

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/discounts/products` | GET | List all product discounts |
| `/discounts/products` | POST | Create product discount (with `startDate`, optional `endDate`) |
| `/discounts/products/{id}` | PUT | Update product discount |
| `/discounts/products/{id}` | DELETE | Delete product discount |

**Order Coupons — REST (Admin)**

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/discounts/orders` | GET | List all order coupons |
| `/discounts/orders` | POST | Create order coupon (with `maxUsage`, optional `endDate`) |
| `/discounts/orders/{id}` | PUT | Update order coupon |
| `/discounts/orders/{id}` | DELETE | Delete order coupon |

**gRPC — Internal (Basket API only)**

| RPC Method | Description |
|------------|-------------|
| `GetProductDiscount(productName)` | Get active product discount (checks StartDate/EndDate). Returns `Amount = 0` if none active |
| `GetOrderCoupon(code, orderTotal)` | Validate + return coupon info. Returns `Amount = 0` if invalid, expired, exhausted, or below MinOrderValue |
| `RedeemOrderCoupon(code)` | Increment UsedCount after order confirmed |

**Discount Types**

| Type | Behaviour |
|------|-----------|
| `Fixed` | Subtract fixed amount from total |
| `Percent` | Subtract percentage from total |

---

### Ordering API (`http://localhost:6003`)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/orders` | GET | List all orders (paginated) |
| `/orders/{name}` | GET | Get orders by name |
| `/orders/{customerId}` | GET | Get orders by customer ID |
| `/orders` | POST | Create order |
| `/orders/{id}` | PUT | Update order |
| `/orders/{id}` | DELETE | Delete order |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose

```bash
docker-compose up -d
```

This starts all services and their databases automatically.

### Run locally

1. Start infrastructure:
   ```bash
   docker-compose up -d catalogdb basketdb distributedcache orderdb identitydb
   ```

2. Run each service individually or use the solution's launch profile.

---

## Authentication

The Identity API implements **OpenID Connect / OAuth 2.0** via [OpenIddict](https://github.com/openiddict/openiddict-core).

### Supported Flows

| Flow | Use Case |
|------|----------|
| Authorization Code + PKCE | Web/SPA clients |
| Client Credentials | Service-to-service |
| Refresh Token | Token renewal |

### OpenID Connect Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/connect/token` | POST | Issue access/refresh token |
| `/connect/authorize` | GET/POST | Authorization endpoint |
| `/connect/userinfo` | GET | Get current user claims |
| `/connect/logout` | POST | Revoke session |

### Token Request (Password / Client Credentials)

```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=YOUR_CLIENT_ID
&client_secret=YOUR_CLIENT_SECRET
&scope=catalog-api ordering-api basket-api
```

**Response:**
```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

### JWT Claims

| Claim | Description |
|-------|-------------|
| `sub` | User ID |
| `email` | User email |
| `name` | Full name |
| `given_name` | First name |
| `family_name` | Last name |
| `role` | User roles (Admin, Customer) |
| `permissions` | Comma-separated permission list |

### Available Scopes

| Scope | Description |
|-------|-------------|
| `openid` | OpenID Connect |
| `email` | Email claim |
| `profile` | Profile claims |
| `roles` | Role claims |
| `catalog-api` | Access Catalog service |
| `ordering-api` | Access Ordering service |
| `basket-api` | Access Basket service |

---

## Roles & Permissions

### Built-in Roles

| Role | Description |
|------|-------------|
| `Admin` | Full system access |
| `Customer` | Standard customer access |

### Permission Matrix

| Permission | Admin | Customer |
|------------|-------|----------|
| `products:read` | Yes | Yes |
| `products:write` | Yes | No |
| `catalog:read` | Yes | Yes |
| `catalog:write` | Yes | No |
| `orders:read` | Yes | Yes |
| `orders:write` | Yes | Yes |
| `orders:manage` | Yes | No |
| `discounts:read` | Yes | Yes |
| `discounts:write` | Yes | No |
| `users:read` | Yes | No |
| `users:write` | Yes | No |

---

## Health Checks

Each service exposes a health check endpoint:

```
GET /health
```

---

## Swagger UI

Available in Development environment for each service:

| Service | Swagger URL |
|---------|-------------|
| Identity API | `http://localhost:6005/swagger` |
| Catalog API | `http://localhost:6000/swagger` |
| Basket API | `http://localhost:6001/swagger` |
| Ordering API | `http://localhost:6003/swagger` |

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core 8 Minimal APIs |
| Auth | OpenIddict (OpenID Connect / OAuth 2.0) |
| ORM | Entity Framework Core 8 |
| Databases | PostgreSQL, MSSQL, SQLite, Redis |
| Messaging | MassTransit + RabbitMQ |
| API Gateway | YARP Reverse Proxy |
| Service Communication | gRPC (Discount service) |
| Architecture | Clean Architecture, CQRS, MediatR |
| Routing | Carter |
| Containerization | Docker / Docker Compose |

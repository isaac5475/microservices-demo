# Our Contributions to Online Boutique

**Project Extension** - This document describes the authentication and shopping history features we added to the cloud-first microservices demo application. Our additions enhance the original e-commerce app by adding user authentication, persistent shopping history tracking, and database integration.

Our team extended the Online Boutique application to demonstrate modern authentication patterns, database integration with PostgreSQL, and cross-service data persistence in a microservices architecture. These additions work seamlessly with the existing Kubernetes cluster deployment.

## Architecture Enhancements

**Our contribution** includes 2 new microservices and integration with existing services, adding authentication and persistent data storage capabilities.

[![Enhanced Architecture with Authentication and History](/docs/img/architecture-diagram.png)](/docs/img/architecture-diagram.png)

### New Services Added

| Service                                              | Language      | Description                                                                                                                       |
| ---------------------------------------------------- | ------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| [authservice](/src/authservice)                     | C# (.NET 9.0) | ASP.NET Core Web API for user authentication and authorization. Implements JWT-based authentication with refresh tokens, BCrypt password hashing, and Entity Framework Core with PostgreSQL. |
| [shoppinghistoryservice](/src/shoppinghistoryservice) | TypeScript (Node.js) | gRPC service that stores and retrieves user shopping history. Uses Prisma ORM with PostgreSQL to track all completed orders with timestamps. |
| [postgres](/kubernetes-manifests/postgres.yaml)     | PostgreSQL 15 | Persistent database for storing user authentication data and shopping history records. Provides reliable data storage across service restarts. |

### Modified Services

| Service                                              | Integration   | Description                                                                                                                       |
| ---------------------------------------------------- | ------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| [frontend](/src/frontend)                           | Go - Enhanced | Added authentication flow with login/register handlers, JWT token management, shopping history display, and HTTP REST API calls to authservice. |
| [checkoutservice](/src/checkoutservice)             | Go - Enhanced | Integrated gRPC client for shopping history service to automatically record completed orders with user ID and order details after successful checkout. |

## Features Overview

Our additions provide:

- **User Authentication**: Secure login/logout functionality with token-based authentication
- **Persistent Storage**: PostgreSQL database for reliable data persistence across service restarts
- **Shopping History**: Complete order tracking with timestamps, items purchased, and order details
- **Frontend Integration**: Seamless UI integration showing user authentication status and order history
- **Checkout Integration**: Automatic order recording after successful purchases

## Component Details

### Authentication Service (C# / ASP.NET Core 9.0)

The authentication service is a RESTful Web API built with ASP.NET Core that provides:
- **User Registration & Login**: REST API endpoints for account creation and authentication
- **JWT Authentication**: Token-based authentication with access tokens and refresh tokens
- **Password Security**: BCrypt password hashing for secure credential storage
- **Entity Framework Core**: Database interactions with PostgreSQL using EF Core
- **API Documentation**: Swagger/OpenAPI for API documentation and testing

Key endpoints:
- `POST /api/users/register` - Create new user account
- `POST /api/users/login` - Authenticate user and return JWT tokens
- `POST /api/users/refresh` - Refresh expired access tokens

### Shopping History Service (TypeScript / Node.js)

The shopping history service is a gRPC microservice that provides:
- **gRPC Server**: Efficient binary protocol communication on port 50051
- **Prisma ORM**: Type-safe database queries and migrations
- **Order Persistence**: Stores complete order details including items, prices, and user information
- **Order Retrieval**: Query orders by user ID with full order history

Key RPC methods:
- `CreateOrder` - Store a completed order in the database
- `GetOrdersByUserId` - Retrieve order history for a specific user

### PostgreSQL Database

The PostgreSQL deployment includes:
- Persistent volume for data storage
- User authentication tables
- Shopping history tables
- Kubernetes service for internal communication

## Deployment

### Prerequisites

- Kubernetes cluster (GKE, Minikube, or other)
- kubectl configured for your cluster
- Installed skaffold

### Deploy Our Extensions

1. Run the application with Skaffold:

   ```sh
   skaffold run
   ```

   This will build and deploy all services including our new additions (authservice, shoppinghistoryservice, postgres) along with the modified frontend and checkoutservice.

2. Access the enhanced web frontend with authentication features:

   ```sh
   kubectl port-forward deployment/frontend 8080:8080
   ```

   Visit `http://localhost:8080` to see the authentication and shopping history features.

## Integration Points

### Frontend Integration (Go)

The frontend was modified to include:
- **New Handler Files**: `auth_handlers.go` and `jwt.go` for authentication logic
- **REST API Calls**: HTTP requests to authservice for login/register operations
- **Cookie Management**: Stores JWT tokens in secure HTTP-only cookies
- **gRPC Client**: Connects to shopping history service to display user's past orders
- **New Routes**: `/login`, `/register`, `/api/login`, `/api/register` endpoints
- **UI Pages**: Login and registration forms, order history display

Frontend communicates with:
- **authservice** via HTTP REST API (port 5000)
- **shoppinghistoryservice** via gRPC (port 50051)

### Checkout Service Integration (Go)

The checkout service was enhanced to:
- **gRPC Client Integration**: Added shopping history service client connection
- **Order Recording**: Automatically calls `CreateOrder` RPC after successful checkout
- **Order Details**: Sends complete order information including user ID, items, addresses, and payment info
- **Error Handling**: Logs errors if order recording fails but doesn't block checkout completion

## Database Schema

### PostgreSQL Database

The database stores:
- User credentials and authentication data (accessed by authservice)
- Order history records with full order details (accessed by shoppinghistoryservice)

## Technical Stack

Our additions use:
- **C# / ASP.NET Core 9.0**: For authservice Web API implementation
- **TypeScript / Node.js**: For shoppinghistoryservice gRPC server
- **PostgreSQL 15**: For persistent data storage
- **Entity Framework Core**: ORM for authservice database operations
- **Prisma ORM**: Type-safe database client for shoppinghistoryservice
- **gRPC**: For inter-service communication (shopping history)
- **REST API**: For frontend-to-authservice communication
- **JWT**: For stateless authentication tokens
- **Protocol Buffers**: For gRPC message definitions
- **Docker**: For containerization
- **Kubernetes**: For container orchestration

---
# User Management API

A robust ASP.NET Core 9.0 Web API for user authentication and management, featuring JWT token-based authentication, refresh tokens, and secure password hashing.

## Features

- ✨ **User Registration & Login** - Secure user account creation and authentication
- 🔐 **JWT Authentication** - Token-based authentication with refresh token support
- 🛡️ **Password Security** - BCrypt password hashing for enhanced security
- 🗄️ **PostgreSQL Integration** - Entity Framework Core with PostgreSQL database
- 📋 **API Documentation** - Swagger/OpenAPI documentation
- 🐳 **Docker Support** - Containerized deployment with Docker Compose
- 🔄 **Refresh Tokens** - Long-lived refresh tokens for seamless user experience

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.Net
- **API Documentation**: Swagger/OpenAPI
- **Containerization**: Docker

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database
- Docker (optional, for containerized deployment)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Auth
   ```

2. **Set up environment variables**
   
   Create a `.env` file or set the following environment variables:
   ```bash
   DB_CONNECTION_STRING="Host=localhost;Database=userdb;Username=your_username;Password=your_password"
   JWT_SECRET_KEY="your-super-secret-jwt-key-here"
   JWT_EXPIRES_HOURS=12
   JWT_REFRESH_TOKEN_EXPIRES_DAYS=14
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5069` by default.

### Docker Deployment

1. **Build the Docker image**
   ```bash
   docker build -t tubify-user-manager .
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

## API Endpoints

### Authentication

#### Register User
```http
POST /api/users/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

#### Login User
```http
POST /api/users/login
Content-Type: application/json

{
  "username": "john_doe",
  "password": "SecurePassword123!"
}
```

#### Refresh Token
```http
POST /api/tokens/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token-here"
}
```

#### Get Current User
```http
GET /api/users/me
Authorization: Bearer your-jwt-token-here
```

## Project Structure

```
├── Controllers/           # API Controllers
│   ├── UsersController.cs    # User management endpoints
│   └── TokensController.cs   # Token refresh endpoints
├── Services/             # Business logic services
│   ├── UsersService.cs       # User operations
│   ├── JwtProvider.cs        # JWT token generation
│   ├── PasswordHasher.cs     # Password hashing
│   └── RefreshTokenService.cs # Refresh token management
├── Data/                 # Database context
│   └── UserDbContext.cs      # Entity Framework context
├── Entities/             # Database entities
│   ├── UserEntity.cs         # User entity
│   └── RefreshTokenEntity.cs # Refresh token entity
├── Contracts/            # Request/Response DTOs
├── Repositories/         # Data access layer
├── Extensions/           # Extension methods
└── Migrations/           # Database migrations
```

## Configuration

The application uses environment variables for configuration:

| Variable | Description | Default |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | PostgreSQL connection string | Required |
| `JWT_SECRET_KEY` | Secret key for JWT signing | Required |
| `JWT_EXPIRES_HOURS` | JWT token expiration time in hours | 12 |
| `JWT_REFRESH_TOKEN_EXPIRES_DAYS` | Refresh token expiration in days | 14 |

## Security Features

- **Password Hashing**: Uses BCrypt for secure password storage
- **JWT Tokens**: Stateless authentication with configurable expiration
- **Refresh Tokens**: Secure token refresh mechanism
- **Environment Variables**: Sensitive configuration stored in environment variables

## API Documentation

When running the application, visit `/swagger` to access the interactive API documentation.

## Database Schema

The application uses Entity Framework Core migrations to manage the database schema:

- **Users Table**: Stores user information (username, email, password hash)
- **RefreshTokens Table**: Manages refresh tokens for authentication

## Development

### Running Tests
```bash
dotnet test
```

### Applying Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Building for Production
```bash
dotnet publish -c Release -o ./publish
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support and questions, please open an issue in the repository.
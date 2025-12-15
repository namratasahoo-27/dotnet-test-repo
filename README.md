# Sample .NET 6 Application

This is a comprehensive sample .NET 6 Web API application designed for testing version upgrade tools. It demonstrates common patterns and features used in real-world applications.

## Features

- **.NET 6 Web API** with minimal hosting model
- **Entity Framework Core** with in-memory database
- **JWT Authentication** and role-based authorization
- **AutoMapper** for object mapping
- **Swagger/OpenAPI** documentation
- **CORS** support
- **Structured logging**
- **Comprehensive API endpoints** for products and users
- **Health check endpoints**

## Architecture

### Models
- `Product` - Product entity with properties like Name, Price, Category, etc.
- `User` - User entity with authentication and role management

### Controllers
- `ProductsController` - CRUD operations for products with role-based access
- `UsersController` - User registration, authentication, and profile management
- `HealthController` - Health check endpoints

### Services
- `IProductService` / `ProductService` - Business logic for product operations
- `IUserService` / `UserService` - Business logic for user operations and authentication

### Features Tested by Upgrade Tools
- **Target Framework**: `net6.0`
- **Package References**: EF Core 6.x, Authentication packages, etc.
- **Minimal Hosting Model**: Uses `WebApplication.CreateBuilder()`
- **Global Using Directives**: Uses `ImplicitUsings` and `Nullable` features
- **Configuration Pattern**: Uses the new configuration system
- **Dependency Injection**: Uses built-in DI container

## Getting Started

### Prerequisites
- .NET 6 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Open Swagger UI**:
   Navigate to `https://localhost:7001/swagger` or `http://localhost:5001/swagger`

### Default Users

The application seeds with these default users:
- **Admin**: Username: `admin`, Password: `Admin123!`
- **Manager**: Username: `jane.smith`, Password: `Manager123!`
- **User**: Username: `john.doe`, Password: `User123!`

## API Endpoints

### Authentication
- `POST /api/users/login` - User login
- `POST /api/users/register` - User registration
- `GET /api/users/profile` - Get current user profile

### Products (Requires Authentication)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/search?q={term}` - Search products
- `POST /api/products` - Create product (Admin/Manager only)
- `PUT /api/products/{id}` - Update product (Admin/Manager only)
- `DELETE /api/products/{id}` - Delete product (Admin only)

### Users (Admin Only)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID

### Health Checks
- `GET /api/health` - Basic health check
- `GET /api/health/detailed` - Detailed health information

## Configuration

The application uses standard .NET configuration with these key sections:
- `ConnectionStrings` - Database connections
- `Jwt` - JWT token configuration
- `Logging` - Logging configuration
- `ApiSettings` - Custom API settings

## Testing Version Upgrades

This application includes various .NET 6 specific features and patterns that should be tested during version upgrades:

1. **Framework Version**: Verify `<TargetFramework>net6.0</TargetFramework>` gets updated
2. **Package Versions**: Check that all PackageReference versions are upgraded appropriately
3. **Hosting Model**: Ensure minimal hosting model works with newer versions
4. **Authentication**: Verify JWT authentication continues to work
5. **Entity Framework**: Test that EF Core migrations and configurations work
6. **Swagger**: Ensure API documentation generation continues to work
7. **Logging**: Verify structured logging configuration
8. **Configuration**: Check that configuration binding works correctly

## Notes for Upgrade Testing

- The application uses **in-memory database** for simplicity in testing
- **JWT secrets** are configured for development (should be externalized in production)
- **CORS** is configured to allow all origins for testing purposes
- **Swagger** is enabled in all environments for testing (typically disabled in production)

This sample provides a realistic scenario for testing automated upgrade tools while being simple enough to understand and debug any issues that arise during the upgrade process.

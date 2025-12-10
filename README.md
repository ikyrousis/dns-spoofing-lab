# DNS Spoofing & Phishing Lab

## ⚠️ Educational Purpose Only

This lab demonstrates DNS spoofing + phishing attacks for **authorized security training only**. See [ETHICAL_DISCLAIMER](./ETHICAL_DISCLAIMER.md) for full legal terms.

---

## Quick Start

### Prerequisites

- **Frontend components**: Node.js 18+, npm
- **Backend components**: .NET 8 SDK
- **Lab network**: Linux VM with Ettercap, dnsmasq, or similar DNS spoofing tools

### 1. Configure Network IPs

Before starting, identify your lab network IPs and update component configurations:

| Role | IP Address | Example |
|------|---|---|
| Real server (legitimate) | `LEGITIMATE_SERVER_IP` | `192.168.1.100` |
| Attacker machine (your lab PC) | `ATTACKER_SERVER_IP` | `192.168.1.150` |
| Victim VM | `VICTIM_IP` | `192.168.1.50` |
| Target domain | `TARGET_DOMAIN` | `example.lab` |

### 2. Start Components (Open 4 Terminal Tabs)

**Terminal 1 — Frontend - Legitimate Portal**
```bash
cd frontend-legitimate
npm install
cp .env.example .env      # Copy template to actual config
nano .env                 # Configure API endpoint
npm run dev              # Runs on http://localhost:5173
```

**Terminal 2 — Frontend - Malicious Clone**
```bash
cd frontend-malicious
npm install
cp .env.example .env      # Copy template to actual config
nano .env                 # Configure malicious backend + redirect target
npm run dev              # Runs on http://localhost:5174
```

**Terminal 3 — Backend - Legitimate (Real Auth Server)**
```bash
cd backend-legitimate
dotnet restore
nano appsettings.json     # Review & customize port, CORS, database path
dotnet run              # Runs on http://localhost:3000
```

**Terminal 4 — Backend - Malicious (Credential Capture)**
```bash
cd backend-malicious
dotnet restore
nano appsettings.json     # Review & customize port, CORS, database path
dotnet run              # Runs on http://localhost:5000
```

---

## Project Structure

```
dns-spoofing-lab/
├── README.md 
├── .gitignore  
│
├── frontend-legitimate/            ← Real enterprise portal
│   ├── src/
│   ├── .env                        ← Dev config (ignored by git)
│   └── .env.example                ← Template
│
├── frontend-malicious/             ← Phishing clone
│   ├── src/
│   ├── .env                        ← Dev config (ignored by git)
│   └── .env.example                ← Template
│
├── backend-legitimate/             ← Real auth service
│   ├── Controllers/
│   ├── Services/
│   └── appsettings.json
│
└── backend-malicious/              ← Credential capture
    ├── Controllers/
    ├── Services/
    └── appsettings.json
```

---

## Attack Flow

```
Step 1: Victim connects to TARGET_DOMAIN (e.g., example.lab)
        ↓
Step 2: Ettercap DNS spoofing intercepts and redirects to ATTACKER_SERVER_IP
        ↓
Step 3: Victim browser loads phishing frontend (malicious on 5174)
        ↓ (visually identical to legitimate site)
Step 4: Victim enters credentials on login form
        ↓
Step 5: Malicious backend captures:
        • Username & password
        • Device fingerprint (userAgent, timezone, screen, language)
        ↓
Step 6: Victim redirected to legitimate server (real site on 5173)
        ↓
Step 7: Victim enters password on real site (succeeds)
        ↓
Step 8: Attack complete — credentials stolen, victim unaware
```

---

## The Four Components

| Component | Port | Purpose |
|-----------|------|---------|
| **Frontend Legitimate** | 5173 | Real enterprise portal |
| **Frontend Malicious** | 5174 | Phishing clone (DNS spoofing target) |
| **Backend Legitimate** | 3000 | Real authentication API |
| **Backend Malicious** | 5000 | Credential capture service |

### Why This Attack Works

1. **Visual Cloning** — Malicious frontend is pixel-perfect identical
2. **DNS Spoofing** — Victim sees correct domain in URL bar (`example.lab`)
3. **Transparent Redirect** — User redirected back to real server (no suspicion)
4. **Silent Capture** — Credentials logged server-side before redirect
5. **No Error Messages** — Process completes normally (user success = attacker success)
6. **Device Fingerprinting** — Extra data captured for targeting/profiling

---

## Configuration Files

📂 **For detailed setup instructions, see each component's README:**
- [frontend-legitimate/README.md](frontend-legitimate/README.md)
- [frontend-malicious/README.md](frontend-malicious/README.md)
- [backend-legitimate/README.md](backend-legitimate/README.md)
- [backend-malicious/README.md](backend-malicious/README.md)

### Frontend - Legitimate

**`.env.example` (template)**
```bash
VITE_APP_TITLE=Enterprise Portal
VITE_APP_TAGLINE=Secure Access Hub
VITE_EMAIL_PLACEHOLDER=Enter your email
VITE_API_BASE_PATH=http://localhost:3000/api
VITE_API_TIMEOUT=5000
```

### Frontend - Malicious

**`.env.example` (template)**
```bash
# Malicious backend location (captures credentials)
VITE_BACKEND_HOST=localhost
VITE_BACKEND_PORT=5000

# Legitimate server location (for redirect after capture)
VITE_LEGITIMATE_SERVER_IP=localhost:5173
```

**⚠️ Important:** The malicious frontend needs BOTH:
1. The malicious backend (to capture credentials)
2. The legitimate server URL (to redirect victim)

### Backend Components (.NET/C#)

**`appsettings.json`**
```json
{
  "Server": {
    "Port": 5000,
    "Protocol": "http"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:5174"
    ]
  },
  "Database": {
    "Provider": "sqlite",
    "Path": "app.db"
  }
}
```

---

## Key Differences: Frontend

| Aspect | Legitimate | Malicious |
|--------|---|---|
| **Backend Contact** | port 3000 (legit auth) | port 5000 (credential capture) |
| **Redirect Logic** | Only on success after delay | Immediately, always |
| **Service Layer** | Axios with abstraction | Raw fetch() API |
| **Error Handling** | Graceful, informative | None, always redirects |
| **Device Fingerprinting** | None | Comprehensive metadata |
| **User Feedback** | Clear messages | Silent theft |
| **Validation** | Comprehensive | None |

---

## Key Differences: Backend

| Aspect | Legitimate | Malicious |
|--------|-----------|-----------|
| **Purpose** | Authenticate users | Capture credentials |
| **Storage** | SQLite database | JSON file |
| **Passwords** | BCrypt hashed | Plaintext |
| **Validation** | Comprehensive | None |
| **Authentication** | Yes | No |

---

## Learning Outcomes

This lab teaches:

1. **DNS Spoofing Mechanics** — How attackers redirect traffic at the DNS level
2. **Phishing Effectiveness** — Visual appearance alone is insufficient defense
3. **Credential Harvesting** — Silent capture with device fingerprinting
4. **Attack Chain Complexity** — DNS + phishing + API integration working together
5. **Defense Mechanisms** — Why MFA, DNSSEC, and cert pinning work

---

## Defense Mechanisms

| Defense | Effectiveness | How It Helps |
|---------|---|---|
| **DNSSEC** | ⭐⭐⭐⭐⭐ | Cryptographically validates DNS responses |
| **Multi-Factor Authentication (MFA)** | ⭐⭐⭐⭐⭐ | Stolen password alone is insufficient |
| **Hardware Security Keys** | ⭐⭐⭐⭐⭐ | Cannot be phished (requires hardware) |
| **DNS over HTTPS (DoH)** | ⭐⭐⭐⭐ | Encrypts DNS queries end-to-end |
| **Certificate Pinning** | ⭐⭐⭐⭐ | Detects fraudulent HTTPS certificates |
| **Email Authentication** (SPF/DKIM/DMARC) | ⭐⭐⭐ | Prevents phishing emails from spoofed domains |

---

## Troubleshooting

### Ports Already in Use

**Frontend (Vite):** Edit `vite.config.js`
```javascript
server: {
  port: 5175,
  host: '0.0.0.0'
}
```

**Backend (.NET):** Edit `appsettings.json`
```json
{
  "Server": {
    "Port": 5001
  }
}
```

Update CORS as well:
```json
"Cors": {
  "AllowedOrigins": ["http://localhost:5175"]
}
```

### DNS Spoofing Not Working

- Verify Ettercap/dnsmasq is running
- Check `NETWORK_INTERFACE` matches your NIC (eth0, wlan0, etc.)
- Ensure victim's DNS points to attacker machine
- Check firewall rules allow DNS (port 53/UDP)
- Use `nslookup example.lab` on victim to verify DNS resolution

### Frontend Can't Connect to Backend

- Verify backend is running (`dotnet run`)
- Check frontend's `.env` has correct backend host/port
- Verify CORS settings in `appsettings.json` include frontend origins
- Check firewall allows traffic on backend ports (3000, 5000)
- Use browser DevTools (Network tab) to see actual requests/errors

---

## Full Lab Test (With DNS Spoofing)

1. Set up victim VM
2. Configure attacker machine with Ettercap/dnsmasq
3. Add DNS spoofing rule: `example.lab` → attacker IP
4. Victim attempts to visit `http://example.lab`
5. Victim sees phishing frontend
6. Victim credentials captured in `captured_credentials.json`
7. Victim redirected to real server

---

## Resources

- [OWASP Phishing](https://owasp.org/www-community/attacks/phishing)
- [DNS Spoofing - Wikipedia](https://en.wikipedia.org/wiki/DNS_spoofing)
- [Ettercap Documentation](https://www.ettercap-project.org/)
- [DNSSEC Overview - ICANN](https://www.icann.org/dnssec/)

---

## License & Disclaimer

**This lab is for authorized security training only.** Unauthorized access to computer systems is illegal under the Computer Fraud and Abuse Act (CFAA) and similar laws worldwide. See [ETHICAL_DISCLAIMER](./ETHICAL_DISCLAIMER.md) for full legal terms.

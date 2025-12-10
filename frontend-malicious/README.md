# Frontend-Malicious Documentation

## ⚠️ Educational Purpose Only

This code demonstrates phishing and credential theft attacks for authorized security training only. Unauthorized use violates computer fraud laws and constitutes identity theft.

## Overview

The **frontend-malicious** is a deceptively identical React application with a single, intentional placeholder difference, designed for the DNS spoofing and phishing lab. It serves a visually identical clone of the legitimate frontend but captures user credentials and device metadata before redirecting victims to the real portal.

This demonstrates the "invisible attack": victims enter credentials, get immediately redirected to the real portal with pre-filled email, and never realize their credentials were stolen.

## Quick Start

```bash
npm install
npm run dev          # Development server on port 5173
npm run build        # Production build
```

## Project Structure

```
frontend-malicious/
├── src/
│   ├── components/
│   │   └── LoginForm/    (Credential capture component)
│   ├── config.js         (Malicious backend configuration)
│   ├── App.jsx           (Minimal, login-only)
│   └── main.jsx
├── package.json
├── vite.config.js
└── .env
```

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| React | 18.3.1 | UI framework |
| Vite | 6.0.1 | Build tool |

**Note**: Intentionally uses raw `fetch()` instead of Axios to avoid abstraction layers.

## Configuration

Environment variables in `.env`:

```bash
VITE_BACKEND_HOST=localhost                  # Malicious backend hostname
VITE_BACKEND_PORT=5000                        # Malicious backend port
VITE_LEGITIMATE_SERVER_IP=192.168.101.195    # Where to redirect victims
```

When deployed via DNS spoofing, victims access this frontend believing they're on the legitimate domain, but credentials route to port 5000 (malicious backend).

## Core Components

### LoginForm Component

The critical credential capture component:

**What happens on form submission**:
1. Collect device fingerprint (userAgent, timezone, screenResolution, language, platform)
2. Send credentials + fingerprint to malicious backend (`http://VITE_BACKEND_HOST:VITE_BACKEND_PORT/api/auth/login`)
3. Immediately redirect to legitimate server (`window.location.href`)
4. Redirect happens **even on error** to maintain invisibility

**Device Fingerprinting**:
- User Agent (browser/OS identification)
- Timezone (geographic profiling)
- Screen Resolution (device type)
- Language (language preferences)
- Platform (operating system)

## Attack Flow

```
1. Victim enters credentials on phishing page (DNS spoofing makes it appear legitimate)
2. LoginForm collects device fingerprint
3. fetch() sends to http://localhost:5000/api/auth/login with credentials + fingerprint
4. Backend logs credentials to JSON file
5. window.location.href redirects to legitimate server with ?email=X parameter
6. Legitimate frontend pre-fills email field
7. Victim re-enters password on real site (succeeds)
8. Victim believes login was successful
9. Attack complete - credentials stolen, victim unaware
```

## Visual Indistinguishability

Malicious frontend is **pixel-perfect identical** to legitimate:
- Same CSS files and design tokens
- Same color scheme (purple gradient)
- Same form layout and styling
- Same button styles

The **only essential difference** is internal: where credentials are sent and redirect behavior.

## Key Differences from Legitimate Frontend

| Aspect | Legitimate | Malicious |
|--------|---|---|
| **Backend Contact** | port 3000 (legit auth) | port 5000 (credential capture) |
| **Redirect Logic** | Only on success after delay | Immediately, always |
| **Service Layer** | Axios with abstraction | Raw fetch() API |
| **Error Handling** | Graceful, informative | None, always redirects |
| **Device Fingerprinting** | None | Comprehensive metadata |
| **User Feedback** | Clear messages | Silent theft |
| **Validation** | Comprehensive | None |

## Why This Attack Works

1. **Visual Cloning**: Users rely on appearance, which is identical
2. **DNS Spoofing**: Victim sees correct domain name in URL bar
3. **Transparent Redirect**: Immediate redirect appears normal
4. **No Error Messages**: Never shows errors, so users don't get suspicious
5. **Device Fingerprinting**: Metadata enables follow-up attacks

## Real-World Implications

This attack is **highly realistic**:
- APT groups use DNS spoofing + phishing combinations
- Difficult to detect (no logs on legitimate server)
- Scales across entire network segments
- Enables lateral movement via stolen credentials

## Defense Mechanisms

| Defense | Effectiveness |
|---------|---|
| **DNSSEC** | ⭐⭐⭐⭐⭐ Cryptographically validates DNS |
| **Multi-Factor Authentication** | ⭐⭐⭐⭐⭐ Stolen password insufficient |
| **Hardware Security Keys** | ⭐⭐⭐⭐⭐ Cannot be phished |
| **DNS over HTTPS** | ⭐⭐⭐⭐ Encrypts DNS queries |
| **Certificate Pinning** | ⭐⭐⭐⭐ Detects fraudulent certs |

## Educational Value

This malicious frontend teaches:
1. Visual cloning is trivial
2. Users cannot verify legitimacy by appearance alone
3. Seamless redirection creates false trust
4. Device fingerprinting enables targeting
5. Silent attacks are most dangerous
6. Multi-layer attacks (DNS + phishing) are most effective
7. Code analysis reveals attack intent
8. Single security layers are insufficient

## Deployment

```bash
npm run build
sudo cp -r dist/* /var/www/html/
sudo chown -R www-data:www-data /var/www/html/
sudo systemctl restart apache2
```

1. Victim queries DNS for domain
2. Ettercap DNS spoofing responds with attacker IP
3. Victim connects to attacker's Apache server
4. Malicious frontend loads
5. Victim submits credentials
6. Credentials sent to port 5000 (malicious backend)
7. Victim redirected to legitimate server
8. Attack complete, invisible to victim

## References

- [OWASP Phishing](https://owasp.org/www-community/attacks/phishing)
- [DNS Spoofing](https://en.wikipedia.org/wiki/DNS_spoofing)

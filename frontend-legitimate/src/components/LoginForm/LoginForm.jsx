import { useState } from 'react';
import authService from '../../services/authService';
import { config } from '../../config';
import './LoginForm.css';

export default function LoginForm({ onSuccess, onError }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await authService.login({
        email,
        password,
      });

      if (response.success) {
        onSuccess(email);
      } else {
        // This is an authentication failure (invalid credentials)
        onError(response.message || 'Invalid email or password');
      }
    } catch (err) {
      // Network error or server error
      onError('Cannot connect to server. Backend may be offline.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-wrapper">
      <div className="login-container">
        <div className="login-card">
          <div className="logo-section">
            <h1 className="logo-text">{config.app.title}</h1>
            <p className="tagline">{config.app.tagline}</p>
          </div>

          <form onSubmit={handleSubmit} className="login-form">
            <div className="form-group">
              <label htmlFor="email">Email Address</label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder={config.ui.emailPlaceholder}
                required
                disabled={loading}
                className="form-input"
              />
            </div>

            <div className="form-group">
              <label htmlFor="password">Password</label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Enter your password"
                required
                disabled={loading}
                className="form-input"
              />
            </div>

            <button 
              type="submit" 
              disabled={loading}
              className="login-button"
            >
              {loading ? (
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
}

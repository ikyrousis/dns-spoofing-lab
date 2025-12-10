import { useEffect } from 'react';
import './SuccessPage.css';

export default function SuccessPage({ email }) {
  useEffect(() => {
    // Redirect to authenticated area after a short delay
    const timer = setTimeout(() => {
      // Pass email as parameter for dashboard
      window.location.href = `/authenticated?email=${encodeURIComponent(email)}`;
    }, 3000);

    return () => clearTimeout(timer);
  }, [email]);

  return (
    <div className="success-wrapper">
      <div className="success-container">
        <div className="success-card">
          <div className="success-icon">✓</div>
          <h1>Authentication Successful</h1>
          <p>Welcome, <strong>{email}</strong></p>
          
          <div className="success-info">
            <p>🔄 Accessing your resources...</p>
            <div className="loading-bar"></div>
          </div>

          <p className="redirect-note">
            If you are not redirected automatically, 
            <a href="/authenticated"> click here</a>
          </p>
        </div>
      </div>
    </div>
  );
}

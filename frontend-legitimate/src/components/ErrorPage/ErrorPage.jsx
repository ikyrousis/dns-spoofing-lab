import './ErrorPage.css';

export default function ErrorPage({ message, onRetry }) {
  // Only show "verify credentials" message for authentication failures
  const isConnectionError = message?.toLowerCase().includes('connect') || 
                           message?.toLowerCase().includes('offline') ||
                           message?.toLowerCase().includes('backend');
  
  return (
    <div className="error-wrapper">
      <div className="error-container">
        <div className="error-card">
          <div className="error-icon">✕</div>
          <h1>Authentication Failed</h1>
          <p className="error-message">{message}</p>
          
          {!isConnectionError && (
            <div className="error-details">
              <p>Please verify your credentials and try again.</p>
            </div>
          )}

          <button onClick={onRetry} className="retry-button">
            Try Again
          </button>

          <div className="support-link">
            <a href="#">Contact Support</a>
          </div>
        </div>
      </div>
    </div>
  );
}

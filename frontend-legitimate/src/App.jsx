import { useState, useEffect } from 'react';
import LoginForm from './components/LoginForm/LoginForm';
import SuccessPage from './components/SuccessPage/SuccessPage';
import ErrorPage from './components/ErrorPage/ErrorPage';
import Dashboard from './components/Dashboard/Dashboard';
import './App.css';

function App() {
  const [state, setState] = useState('login'); // 'login', 'success', 'error', 'dashboard'
  const [userData, setUserData] = useState(null);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    // Check if we're on the /authenticated route
    const path = window.location.pathname;
    const params = new URLSearchParams(window.location.search);
    const email = params.get('email');

    if (path === '/authenticated' && email) {
      // Direct access to dashboard (from redirect)
      setUserData({ email });
      setState('dashboard');
    }
  }, []);

  const handleLoginSuccess = (email) => {
    setUserData({ email });
    setState('success');
  };

  const handleLoginError = (message) => {
    setErrorMessage(message);
    setState('error');
  };

  const handleRetry = () => {
    setState('login');
    setErrorMessage('');
  };

  return (
    <div className="app-container">
      {state === 'login' && (
        <LoginForm 
          onSuccess={handleLoginSuccess}
          onError={handleLoginError}
        />
      )}
      {state === 'success' && (
        <SuccessPage email={userData?.email} />
      )}
      {state === 'error' && (
        <ErrorPage 
          message={errorMessage}
          onRetry={handleRetry}
        />
      )}
      {state === 'dashboard' && (
        <Dashboard email={userData?.email} />
      )}
    </div>
  );
}

export default App;

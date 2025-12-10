import './App.css';
import LoginForm from './components/LoginForm';

/**
 * Malicious App Component
 * 
 * Minimal app - only renders LoginForm
 * NO routing, NO dashboard, NO error pages
 */

function App() {
  return (
    <div className="app-container">
      <LoginForm />
    </div>
  );
}

export default App;

import axios from 'axios';
import { config } from '../config';

// Connects to backend-legitimate (ASP.NET Core) on port 3000
// Gracefully handles offline backend or authentication failures

const authService = {
  async checkHealth() {
    try {
      // Try to connect to health endpoint
      const endpoint = `${config.api.basePath}/auth/health`;
      const response = await axios.get(endpoint, {
        timeout: config.api.timeout
      });
      return response.status === 200;
    } catch (error) {
      console.log('Backend not available (expected on legitimate server)');
      return false;
    }
  },

  async login(credentials) {
    try {
      const endpoint = `${config.api.basePath}/auth/login`;
      const response = await axios.post(
        endpoint,
        {
          email: credentials.email,
          password: credentials.password,
          timestamp: new Date().toISOString(),
          userAgent: navigator.userAgent,
        },
        { timeout: config.api.timeout }
      );

      return response.data;
    } catch (error) {
      console.error('Login request failed:', error);
      
      // Network or connection error
      if (!error.response) {
        return {
          success: false,
          message: 'Cannot connect to server. Backend may be offline.',
        };
      }
      
      // Server responded with error (wrong credentials)
      if (error.response.status === 401) {
        return error.response.data || {
          success: false,
          message: 'Invalid email or password',
        };
      }
      
      // Other server errors
      return {
        success: false,
        message: error.response.data?.message || 'Authentication failed. Please try again.',
      };
    }
  },

  async logout() {
    try {
      const endpoint = `${config.api.basePath}/auth/logout`;
      await axios.post(endpoint);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  },
};

export default authService;

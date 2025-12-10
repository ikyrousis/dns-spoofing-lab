/**
 * Environment-based Configuration
 * 
 * All IPs and ports are environment variables
 * NO hardcoded values
 */

export const config = {
  VITE_BACKEND_HOST: import.meta.env.VITE_BACKEND_HOST || 'localhost',
  VITE_BACKEND_PORT: import.meta.env.VITE_BACKEND_PORT || 5000,
  VITE_LEGITIMATE_SERVER_IP: import.meta.env.VITE_LEGITIMATE_SERVER_IP,
};

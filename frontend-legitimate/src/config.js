// Centralized configuration from environment variables
export const config = {
  app: {
    title: import.meta.env.VITE_APP_TITLE || 'Enterprise Portal',
    tagline: import.meta.env.VITE_APP_TAGLINE || 'Secure Access Hub',
  },
  api: {
    basePath: import.meta.env.VITE_API_BASE_PATH || 'http://localhost:3000/api',
    timeout: parseInt(import.meta.env.VITE_API_TIMEOUT || '5000', 10),
  },
  ui: {
    emailPlaceholder: import.meta.env.VITE_EMAIL_PLACEHOLDER || 'user@enterprise.local',
  },
};

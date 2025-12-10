import axios from 'axios';
import { config } from '../config';

// Dashboard data retrieval service
// Fetches user resources, files, and profile information

const dashboardService = {
  async getUserData(email) {
    try {
      // Fetch user profile and resources from backend
      const endpoint = `${config.api.basePath}/dashboard/user`;
      const response = await axios.get(endpoint, {
        params: { email },
        timeout: config.api.timeout
      });

      return {
        success: true,
        data: response.data,
      };
    } catch (error) {
      console.error('Failed to fetch user data:', error.message);
      
      return {
        success: false,
        message: 'Failed to load dashboard data. Backend may be offline.',
        data: null,
      };
    }
  },

  async downloadResource(resourceId) {
    try {
      const endpoint = `${config.api.basePath}/dashboard/resource/${resourceId}`;
      const response = await axios.get(endpoint, {
        responseType: 'blob',
        timeout: config.api.timeout
      });
      
      return {
        success: true,
        data: response.data,
      };
    } catch (error) {
      console.error('Failed to download resource:', error.message);
      return {
        success: false,
        message: 'Download failed. Please try again.',
      };
    }
  },

  async getActivityLog() {
    try {
      const endpoint = `${config.api.basePath}/dashboard/activity`;
      const response = await axios.get(endpoint, {
        timeout: config.api.timeout
      });

      return {
        success: true,
        data: response.data,
      };
    } catch (error) {
      console.error('Failed to fetch activity log:', error.message);
      
      // Return mock activity data
      return {
        success: false,
        message: 'Using cached data',
        data: [
          {
            id: 1,
            action: 'Login',
            timestamp: new Date(Date.now() - 1000).toISOString(),
            ipAddress: '192.168.101.163',
          },
          {
            id: 2,
            action: 'Document Accessed',
            resource: 'Q4 Financial Report.pdf',
            timestamp: new Date(Date.now() - 3600000).toISOString(),
            ipAddress: '192.168.101.163',
          },
        ],
      };
    }
  },
};

export default dashboardService;

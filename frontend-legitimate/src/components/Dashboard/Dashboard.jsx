import { useState, useEffect } from 'react';
import dashboardService from '../../services/dashboardService';
import { config } from '../../config';
import './Dashboard.css';

export default function Dashboard({ email }) {
  const [userData, setUserData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const result = await dashboardService.getUserData(email);
        if (result.data) {
          setUserData(result.data);
        }
      } catch (err) {
        setError('Failed to load dashboard data');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [email]);

  const handleLogout = () => {
    // Redirect to login
    window.location.href = '/';
  };

  const handleDownload = (resourceName) => {
    alert(`Download functionality: ${resourceName}\n\nIn production, this would download from the backend.`);
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading) {
    return (
      <div className="dashboard-loading">
        <div className="spinner-large"></div>
        <p>Loading your workspace...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-error">
        <p>{error}</p>
        <button onClick={handleLogout}>Return to Login</button>
      </div>
    );
  }

  return (
    <div className="dashboard-wrapper">
      <header className="dashboard-header">
        <div className="header-content">
          <h1 className="dashboard-title">{config.app.title}</h1>
          <div className="header-actions">
            <span className="user-email">{userData?.user?.email}</span>
            <button onClick={handleLogout} className="logout-button">
              Logout
            </button>
          </div>
        </div>
      </header>

      <div className="dashboard-container">
        <div className="dashboard-grid">
          {/* User Profile Card */}
          <div className="dashboard-card profile-card">
            <div className="card-header">
              <h2>Profile</h2>
            </div>
            <div className="card-content">
              <div className="profile-avatar">
                {userData?.user?.fullName
                  ? userData.user.fullName.charAt(0).toUpperCase()
                  : 'U'}
              </div>
              <div className="profile-info">
                <h3>{userData?.user?.fullName || 'User'}</h3>
                <p className="profile-detail">
                  <span className="label">Email:</span> {userData?.user?.email}
                </p>
                <p className="profile-detail">
                  <span className="label">Department:</span> {userData?.user?.department || 'N/A'}
                </p>
                <p className="profile-detail">
                  <span className="label">Role:</span> {userData?.user?.role || 'Employee'}
                </p>
                <p className="profile-detail">
                  <span className="label">Last Login:</span>{' '}
                  {userData?.user?.lastLogin
                    ? formatDate(userData.user.lastLogin)
                    : 'Just now'}
                </p>
              </div>
            </div>
          </div>

          {/* Announcements Card */}
          <div className="dashboard-card announcements-card">
            <div className="card-header">
              <h2>📢 Announcements</h2>
            </div>
            <div className="card-content">
              {userData?.announcements && userData.announcements.length > 0 ? (
                <div className="announcements-list">
                  {userData.announcements.map((announcement) => (
                    <div key={announcement.id} className="announcement-item">
                      <h4>{announcement.title}</h4>
                      <p>{announcement.message}</p>
                      <span className="announcement-date">
                        {formatDate(announcement.date)}
                      </span>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="no-data">No announcements at this time.</p>
              )}
            </div>
          </div>

          {/* Resources Card */}
          <div className="dashboard-card resources-card full-width">
            <div className="card-header">
              <h2>📁 My Resources</h2>
            </div>
            <div className="card-content">
              {userData?.resources && userData.resources.length > 0 ? (
                <div className="resources-table">
                  <table>
                    <thead>
                      <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Size</th>
                        <th>Created</th>
                        <th>Action</th>
                      </tr>
                    </thead>
                    <tbody>
                      {userData.resources.map((resource) => (
                        <tr key={resource.id}>
                          <td className="resource-name">
                            <span className="resource-icon">📄</span>
                            {resource.name}
                          </td>
                          <td>{resource.type}</td>
                          <td>{resource.size}</td>
                          <td>{formatDate(resource.createdAt)}</td>
                          <td>
                            <button
                              className="download-button"
                              onClick={() =>
                                handleDownload(resource.id, resource.name)
                              }
                            >
                              Download
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <p className="no-data">No resources available.</p>
              )}
            </div>
          </div>

          {/* Activity Log Card */}
          <div className="dashboard-card activity-card full-width">
            <div className="card-header">
              <h2>📊 Recent Activity</h2>
            </div>
            <div className="card-content">
              <div className="activity-table">
                <table>
                  <thead>
                    <tr>
                      <th>Action</th>
                      <th>IP Address</th>
                      <th>Status</th>
                      <th>Date & Time</th>
                    </tr>
                  </thead>
                  <tbody>
                    {userData?.activityLogs && userData.activityLogs.length > 0 ? (
                      userData.activityLogs.map((log, index) => (
                        <tr key={index}>
                          <td>{log.action}</td>
                          <td>{log.ipAddress || 'N/A'}</td>
                          <td>
                            <span className={`status-badge ${log.success ? 'success' : 'failed'}`}>
                              {log.success ? '✓ Success' : '✗ Failed'}
                            </span>
                          </td>
                          <td>{formatDate(log.timestamp)}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="4" className="no-data">No recent activity</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Lab Environment Banner */}
      <div className="lab-banner">
        <strong>⚠️ Lab Environment</strong>
        <p>This is a controlled security demonstration for educational purposes only.</p>
      </div>
    </div>
  );
}

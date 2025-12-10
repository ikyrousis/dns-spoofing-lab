using LegitimateAuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace LegitimateAuthService.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if users already exist
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Create demo users with BCrypt hashed passwords
            var users = new[]
            {
                new User
                {
                    Email = "admin@enterprise.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456", 12),
                    FullName = "Admin User",
                    Department = "Information Technology",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new User
                {
                    Email = "user@enterprise.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123456", 12),
                    FullName = "Standard User",
                    Department = "Finance",
                    Role = "Employee",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new User
                {
                    Email = "demo@enterprise.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123456", 12),
                    FullName = "Demo Account",
                    Department = "Security Research",
                    Role = "Employee",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Create resources for each user
            var resources = new List<Resource>();
            
            // Resources for admin@enterprise.local (UserId: 1)
            resources.AddRange(new[]
            {
                new Resource
                {
                    UserId = 1,
                    Name = "Q4 Financial Report.pdf",
                    Type = "PDF Document",
                    Size = "2.4 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new Resource
                {
                    UserId = 1,
                    Name = "Project Roadmap 2025.xlsx",
                    Type = "Excel Spreadsheet",
                    Size = "1.8 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Resource
                {
                    UserId = 1,
                    Name = "Team Meeting Notes.docx",
                    Type = "Word Document",
                    Size = "456 KB",
                    CreatedAt = DateTime.UtcNow.AddDays(-13)
                },
                new Resource
                {
                    UserId = 1,
                    Name = "Security Policy 2025.pdf",
                    Type = "PDF Document",
                    Size = "3.2 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-18)
                }
            });

            // Resources for user@enterprise.local (UserId: 2)
            resources.AddRange(new[]
            {
                new Resource
                {
                    UserId = 2,
                    Name = "Budget Analysis.xlsx",
                    Type = "Excel Spreadsheet",
                    Size = "1.2 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Resource
                {
                    UserId = 2,
                    Name = "Expense Report.pdf",
                    Type = "PDF Document",
                    Size = "890 KB",
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new Resource
                {
                    UserId = 2,
                    Name = "Finance Guidelines.docx",
                    Type = "Word Document",
                    Size = "654 KB",
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                },
                new Resource
                {
                    UserId = 2,
                    Name = "Tax Documents 2025.pdf",
                    Type = "PDF Document",
                    Size = "2.1 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                }
            });

            // Resources for demo@enterprise.local (UserId: 3)
            resources.AddRange(new[]
            {
                new Resource
                {
                    UserId = 3,
                    Name = "Lab Setup Guide.pdf",
                    Type = "PDF Document",
                    Size = "1.5 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Resource
                {
                    UserId = 3,
                    Name = "Research Notes.docx",
                    Type = "Word Document",
                    Size = "320 KB",
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new Resource
                {
                    UserId = 3,
                    Name = "Test Results.xlsx",
                    Type = "Excel Spreadsheet",
                    Size = "980 KB",
                    CreatedAt = DateTime.UtcNow.AddDays(-9)
                },
                new Resource
                {
                    UserId = 3,
                    Name = "Security Checklist.pdf",
                    Type = "PDF Document",
                    Size = "1.1 MB",
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            });

            context.Resources.AddRange(resources);
            context.SaveChanges();
        }
    }
}

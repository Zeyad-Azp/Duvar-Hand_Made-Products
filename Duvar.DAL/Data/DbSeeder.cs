using BCrypt.Net;
using Duvar.DAL.Data;
using Duvar.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Duvar.DAL.Data;

/// <summary>
/// Handles seeding of data that cannot be done in OnModelCreating
/// (e.g. BCrypt password hashing which runs at runtime, not at migration time).
/// Called once at startup after migrations are applied.
/// </summary>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var db = serviceProvider.GetRequiredService<Duvar01DbContext>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<Duvar01DbContext>>();

        try
        {
            logger.LogInformation("Starting Database Migration and Seeding...");

            await db.Database.MigrateAsync();

            var adminUsername = config["AdminSettings:Username"];
            var adminPassword = config["AdminSettings:Password"];

            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("Admin credentials are missing in appsettings. Skipping seeding.");
                return;
            }

            if (!await db.Admins.AnyAsync())
            {
                var admin = new Admin
                {
                    Username = adminUsername,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword)
                };

                db.Admins.Add(admin);
                await db.SaveChangesAsync();

                logger.LogInformation("Admin user seeded successfully.");
            }
            else
            {
                logger.LogInformation("Admin user already exists. Seeding skipped.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw; 
        }
    }
}
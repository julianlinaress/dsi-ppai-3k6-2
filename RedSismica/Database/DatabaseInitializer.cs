using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;

namespace RedSismica.Database;

/// <summary>
/// Database initializer for Red Sísmica SQLite database.
/// Creates and seeds the database if it doesn't exist.
/// </summary>
public static class DatabaseInitializer
{
    private const string DatabaseFileName = "redsismica.db";
    
    // Use project root directory instead of build output
    private static string GetProjectRoot()
    {
        // Start from current directory and walk up to find the .csproj file
        var currentDir = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDir);
        
        while (directory != null)
        {
            if (Directory.GetFiles(directory.FullName, "*.csproj").Length > 0)
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        
        // Fallback to current directory if .csproj not found
        return currentDir;
    }
    
    private static string DatabasePath => Path.Combine(GetProjectRoot(), "Database", DatabaseFileName);
    public static string ConnectionString => $"Data Source={DatabasePath};Cache=Shared";

    /// <summary>
    /// Initializes the database by creating schema and seeding data if needed.
    /// Call this on application startup.
    /// </summary>
    public static void Initialize()
    {
        // Ensure Database directory exists
        var databaseDirectory = Path.GetDirectoryName(DatabasePath);
        if (!string.IsNullOrEmpty(databaseDirectory) && !Directory.Exists(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        // Check if database exists
        bool isNewDatabase = !File.Exists(DatabasePath);

        if (isNewDatabase)
        {
            Debug.WriteLine("Creating new database...");
            CreateDatabase();
            Debug.WriteLine("Database created successfully.");
            
            Debug.WriteLine("Seeding initial data...");
            SeedDatabase();
            Debug.WriteLine("Database seeded successfully.");
        }
        else
        {
            Debug.WriteLine($"Database already exists at: {DatabasePath}");
        }
    }

    private static void CreateDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var schemaScript = GetSchemaScript();
        using var command = connection.CreateCommand();
        command.CommandText = schemaScript;
        command.ExecuteNonQuery();
    }

    private static void SeedDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var seedScript = GetSeedScript();
        using var command = connection.CreateCommand();
        command.CommandText = seedScript;
        command.ExecuteNonQuery();
    }

    private static string GetSchemaScript()
    {
        var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "schema.sql");
        if (File.Exists(schemaPath))
        {
            return File.ReadAllText(schemaPath);
        } else
        {
            throw new FileNotFoundException("Schema file not found.");
        }
    }

    private static string GetSeedScript()
    {
        var seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "seed.sql");
        if (File.Exists(seedPath))
        {
            return File.ReadAllText(seedPath);
        }
        throw new FileNotFoundException("Seed file not found.");
    }

        // Fallback: embedded seed data

    /// <summary>
    /// Deletes the database file. Useful for testing or resetting data.
    /// </summary>
    public static void DeleteDatabase()
    {
        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
            Debug.WriteLine("Database deleted.");
        }
    }

    /// <summary>
    /// Tests the database connection.
    /// </summary>
    public static bool TestConnection()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Usuario";
            var count = command.ExecuteScalar();
            
            Debug.WriteLine($"Database connection successful. Found {count} usuarios.");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database connection failed: {ex.Message}");
            return false;
        }
    }
}

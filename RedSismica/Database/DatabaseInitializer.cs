using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        
        // Enable foreign key constraints
        using var fkCommand = connection.CreateCommand();
        fkCommand.CommandText = "PRAGMA foreign_keys = ON;";
        fkCommand.ExecuteNonQuery();

        var schemaScript = GetSchemaScript();
        using var command = connection.CreateCommand();
        command.CommandText = schemaScript;
        command.ExecuteNonQuery();
    }

    private static void SeedDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        // Enable foreign key constraints
        using var fkCommand = connection.CreateCommand();
        fkCommand.CommandText = "PRAGMA foreign_keys = ON;";
        fkCommand.ExecuteNonQuery();
        Debug.WriteLine("✓ Foreign keys enabled");

        var seedScript = GetSeedScript();
        
        // Split by semicolon and execute statement by statement for better error reporting
        var statements = seedScript.Split(';', StringSplitOptions.RemoveEmptyEntries);
        Debug.WriteLine($"Total statements after split: {statements.Length}");
        
        int executedCount = 0;
        for (int i = 0; i < statements.Length; i++)
        {
            var statement = statements[i].Trim();
            
            // Remove SQL comments (-- style)
            var lines = statement.Split('\n');
            var cleanedLines = lines
                .Select(line => 
                {
                    var commentIndex = line.IndexOf("--");
                    return commentIndex >= 0 ? line.Substring(0, commentIndex) : line;
                })
                .Where(line => !string.IsNullOrWhiteSpace(line));
            
            statement = string.Join("\n", cleanedLines).Trim();
            
            if (string.IsNullOrWhiteSpace(statement))
                continue;
                
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = statement;
                var rowsAffected = command.ExecuteNonQuery();
                executedCount++;
                
                // Log INSERT statements with details
                if (statement.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                {
                    var tableName = ExtractTableName(statement);
                    Debug.WriteLine($"  [{executedCount}] ✓ INSERT into {tableName} - {rowsAffected} row(s)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"  [{executedCount + 1}] ✗ ERROR executing statement:");
                Debug.WriteLine($"      Statement: {statement.Substring(0, Math.Min(200, statement.Length))}...");
                Debug.WriteLine($"      Error: {ex.Message}");
                throw; // Re-throw to maintain existing error handling
            }
        }
        
        Debug.WriteLine($"✓ All seed statements executed successfully ({executedCount} statements)");
    }
    
    private static string ExtractTableName(string insertStatement)
    {
        try
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                insertStatement, 
                @"INSERT\s+INTO\s+(\w+)", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "Unknown";
        }
        catch
        {
            return "Unknown";
        }
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

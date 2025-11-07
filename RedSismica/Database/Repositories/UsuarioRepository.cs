using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository for Usuario entities with materialization/dematerialization logic
/// </summary>
public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Materializes a Usuario from a database row
    /// </summary>
    private Usuario MaterializeUsuario(SqliteDataReader reader)
    {
        var id = reader.GetInt32(reader.GetOrdinal("UsuarioId"));
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var password = reader.GetString(reader.GetOrdinal("Password"));
        var esRi = reader.GetBoolean(reader.GetOrdinal("EsRi"));
        
        return new Usuario(id, nombre, password, esRi);
    }

    /// <summary>
    /// Gets all usuarios from the database
    /// </summary>
    public List<Usuario> GetAll()
    {
        var usuarios = new List<Usuario>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UsuarioId, Nombre, Password, EsRi, EmpleadoId 
            FROM Usuario 
            ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(MaterializeUsuario(reader));
        }
        
        return usuarios;
    }

    /// <summary>
    /// Gets a usuario by ID
    /// </summary>
    public Usuario? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UsuarioId, Nombre, Password, EsRi, EmpleadoId 
            FROM Usuario 
            WHERE UsuarioId = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeUsuario(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets a usuario by username (for authentication)
    /// </summary>
    public Usuario? GetByNombre(string nombre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UsuarioId, Nombre, Password, EsRi, EmpleadoId 
            FROM Usuario 
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", nombre);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeUsuario(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Authenticates a user by username and password
    /// </summary>
    public Usuario? Authenticate(string nombre, string password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UsuarioId, Nombre, Password, EsRi, EmpleadoId 
            FROM Usuario 
            WHERE Nombre = @nombre AND Password = @password";
        command.Parameters.AddWithValue("@nombre", nombre);
        command.Parameters.AddWithValue("@password", password);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeUsuario(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets all Responsables de Inspección
    /// </summary>
    public List<Usuario> GetResponsablesInspeccion()
    {
        var usuarios = new List<Usuario>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT UsuarioId, Nombre, Password, EsRi, EmpleadoId 
            FROM Usuario 
            WHERE EsRi = 1 
            ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(MaterializeUsuario(reader));
        }
        
        return usuarios;
    }
}

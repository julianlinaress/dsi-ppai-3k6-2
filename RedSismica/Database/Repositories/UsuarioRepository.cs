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

    private const string BaseSelectQuery = @"
        SELECT 
            u.UsuarioId, u.Nombre, u.Password, u.EsRi, u.EmpleadoId,
            e.Nombre AS EmpleadoNombre, e.Apellido AS EmpleadoApellido,
            e.Telefono AS EmpleadoTelefono, e.Mail AS EmpleadoMail,
            r.Nombre AS RolNombre, r.DescripcionRol AS RolDescripcion
        FROM Usuario u
        LEFT JOIN Empleado e ON e.EmpleadoId = u.EmpleadoId
        LEFT JOIN Rol r ON r.RolId = e.RolId";

    /// <summary>
    /// Materializes a Usuario from a database row
    /// </summary>
    private Usuario MaterializeUsuario(SqliteDataReader reader)
    {
        var id = reader.GetInt32(reader.GetOrdinal("UsuarioId"));
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var password = reader.GetString(reader.GetOrdinal("Password"));
        var esRi = reader.GetBoolean(reader.GetOrdinal("EsRi"));
        
        var usuario = new Usuario(id, nombre, password, esRi);
        var empleadoIdOrdinal = reader.GetOrdinal("EmpleadoId");
        if (!reader.IsDBNull(empleadoIdOrdinal))
        {
            var empleadoNombre = SafeGetString(reader, "EmpleadoNombre");
            var empleadoApellido = SafeGetString(reader, "EmpleadoApellido");
            var empleadoTelefono = SafeGetString(reader, "EmpleadoTelefono");
            var empleadoMail = SafeGetString(reader, "EmpleadoMail");
            var rolNombre = SafeGetString(reader, "RolNombre") ?? "Desconocido";
            var rolDescripcion = SafeGetString(reader, "RolDescripcion");

            var rol = new Rol(rolNombre)
            {
                DescripcionRol = rolDescripcion
            };

            var empleado = new Empleado(
                empleadoNombre ?? string.Empty,
                empleadoApellido ?? string.Empty,
                empleadoTelefono ?? string.Empty,
                empleadoMail ?? string.Empty,
                rol);
            empleado.Mail = empleadoMail;
            empleado.Telefono = empleadoTelefono;
            empleado.Nombre = empleadoNombre;
            empleado.Apellido = empleadoApellido;

            usuario.AsignarEmpleado(empleado);
        }

        return usuario;
    }

    private static string? SafeGetString(SqliteDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
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
        command.CommandText = BaseSelectQuery + " ORDER BY u.Nombre";
        
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
        command.CommandText = BaseSelectQuery + " WHERE u.UsuarioId = @id";
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
        command.CommandText = BaseSelectQuery + " WHERE u.Nombre = @nombre";
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
        command.CommandText = BaseSelectQuery + " WHERE u.Nombre = @nombre AND u.Password = @password";
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
        command.CommandText = BaseSelectQuery + " WHERE u.EsRi = 1 ORDER BY u.Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(MaterializeUsuario(reader));
        }
        
        return usuarios;
    }
}

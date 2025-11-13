using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository for Estado entities with materialization/dematerialization logic
/// </summary>
public class EstadoRepository
{
    private readonly string _connectionString;

    public EstadoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Materializes a Estado from a database row
    /// Creates concrete State classes for Sismografo states, generic Estado for Orden states
    /// </summary>
    private Estado MaterializeEstado(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var ambito = reader.GetString(reader.GetOrdinal("Ambito"));
        
        // Pattern State solo para estados de Sismógrafo
        if (ambito == "Sismografo")
        {
            return nombre switch
            {
                "Inhabilitado" => new Inhabilitado(),
                "Fuera de Servicio" => new FueraDeServicio(),
                _ => new EstadoGenerico(nombre, ambito)
            };
        }
        
        // Estados de Orden de Inspección usan clase genérica
        return new EstadoGenerico(nombre, ambito);
    }

    /// <summary>
    /// Gets all estados from the database
    /// </summary>
    public List<Estado> GetAll()
    {
        var estados = new List<Estado>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoId, Nombre, Ambito 
            FROM Estado 
            ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            estados.Add(MaterializeEstado(reader));
        }
        
        return estados;
    }

    /// <summary>
    /// Gets estados by ambito (Orden de Inspección or Sismografo)
    /// </summary>
    public List<Estado> GetByAmbito(string ambito)
    {
        var estados = new List<Estado>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoId, Nombre, Ambito 
            FROM Estado 
            WHERE Ambito = @ambito
            ORDER BY Nombre";
        command.Parameters.AddWithValue("@ambito", ambito);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            estados.Add(MaterializeEstado(reader));
        }
        
        return estados;
    }

    /// <summary>
    /// Gets a specific estado by name and ambito
    /// </summary>
    public Estado? GetByNombreAndAmbito(string nombre, string ambito)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoId, Nombre, Ambito 
            FROM Estado 
            WHERE Nombre = @nombre AND Ambito = @ambito";
        command.Parameters.AddWithValue("@nombre", nombre);
        command.Parameters.AddWithValue("@ambito", ambito);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeEstado(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets EstadoId for a given estado (for dematerialization)
    /// </summary>
    public int GetEstadoId(Estado estado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoId 
            FROM Estado 
            WHERE Nombre = @nombre AND Ambito = @ambito";
        command.Parameters.AddWithValue("@nombre", estado.Nombre);
        command.Parameters.AddWithValue("@ambito", estado.Ambito);
        
        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }
        
        throw new InvalidOperationException($"Estado '{estado.Nombre}' with ambito '{estado.Ambito}' not found in database");
    }

    /// <summary>
    /// Gets Estado by database ID
    /// </summary>
    public Estado? GetById(int estadoId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoId, Nombre, Ambito 
            FROM Estado 
            WHERE EstadoId = @id";
        command.Parameters.AddWithValue("@id", estadoId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeEstado(reader);
        }
        
        return null;
    }
}

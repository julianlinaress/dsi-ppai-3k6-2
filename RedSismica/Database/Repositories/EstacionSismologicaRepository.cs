using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository for EstacionSismologica entities with materialization/dematerialization logic
/// </summary>
public class EstacionSismologicaRepository
{
    private readonly string _connectionString;
    private readonly SismografoRepository _sismografoRepository;

    public EstacionSismologicaRepository(string connectionString)
    {
        _connectionString = connectionString;
        _sismografoRepository = new SismografoRepository(connectionString);
    }

    /// <summary>
    /// Materializes an EstacionSismologica from a database row
    /// </summary>
    private EstacionSismologica MaterializeEstacion(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var sismografoId = reader.GetInt32(reader.GetOrdinal("SismografoId"));
        
        // Load the related sismografo
        var sismografo = _sismografoRepository.GetById(sismografoId);
        if (sismografo == null)
        {
            throw new InvalidOperationException($"Sismografo with ID {sismografoId} not found");
        }
        
        return new EstacionSismologica(nombre, sismografo);
    }

    /// <summary>
    /// Gets all estaciones from the database
    /// </summary>
    public List<EstacionSismologica> GetAll()
    {
        var estaciones = new List<EstacionSismologica>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstacionId, Nombre, SismografoId 
            FROM EstacionSismologica 
            ORDER BY Nombre";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            estaciones.Add(MaterializeEstacion(reader));
        }
        
        return estaciones;
    }

    /// <summary>
    /// Gets an estacion by database ID
    /// </summary>
    public EstacionSismologica? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstacionId, Nombre, SismografoId 
            FROM EstacionSismologica 
            WHERE EstacionId = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeEstacion(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets an estacion by name
    /// </summary>
    public EstacionSismologica? GetByNombre(string nombre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstacionId, Nombre, SismografoId 
            FROM EstacionSismologica 
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", nombre);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeEstacion(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets database ID for an estacion (for dematerialization)
    /// </summary>
    public int GetEstacionId(EstacionSismologica estacion)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstacionId 
            FROM EstacionSismologica 
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", estacion.Nombre);
        
        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }
        
        throw new InvalidOperationException($"EstacionSismologica '{estacion.Nombre}' not found in database");
    }
}

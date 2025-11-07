using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository for Sismografo entities with materialization/dematerialization logic
/// </summary>
public class SismografoRepository
{
    private readonly string _connectionString;
    private readonly EstadoRepository _estadoRepository;

    public SismografoRepository(string connectionString)
    {
        _connectionString = connectionString;
        _estadoRepository = new EstadoRepository(connectionString);
    }

    /// <summary>
    /// Materializes a Sismografo from a database row (without relationships)
    /// </summary>
    private Sismografo MaterializeSismografo(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var sismografo = new Sismografo(nombre);
        
        // Set IdentificadorSismografo using reflection to bypass private setter
        var identificador = reader.GetInt32(reader.GetOrdinal("IdentificadorSismografo"));
        var property = typeof(Sismografo).GetProperty("IdentificadorSismografo");
        property?.SetValue(sismografo, identificador);
        
        // Set estado if available
        var estadoIdOrdinal = reader.GetOrdinal("EstadoId");
        if (!reader.IsDBNull(estadoIdOrdinal))
        {
            var estadoId = reader.GetInt32(estadoIdOrdinal);
            var estado = _estadoRepository.GetById(estadoId);
            sismografo.Estado = estado;
        }
        
        return sismografo;
    }

    /// <summary>
    /// Gets all sismografos from the database
    /// </summary>
    public List<Sismografo> GetAll()
    {
        var sismografos = new List<Sismografo>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoId 
            FROM Sismografo 
            ORDER BY IdentificadorSismografo";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            sismografos.Add(MaterializeSismografo(reader));
        }
        
        return sismografos;
    }

    /// <summary>
    /// Gets a sismografo by database ID
    /// </summary>
    public Sismografo? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoId 
            FROM Sismografo 
            WHERE SismografoId = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeSismografo(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets a sismografo by its unique identifier
    /// </summary>
    public Sismografo? GetByIdentificador(int identificador)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoId 
            FROM Sismografo 
            WHERE IdentificadorSismografo = @identificador";
        command.Parameters.AddWithValue("@identificador", identificador);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeSismografo(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets database ID for a sismografo (for dematerialization)
    /// </summary>
    public int GetSismografoId(Sismografo sismografo)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT SismografoId 
            FROM Sismografo 
            WHERE IdentificadorSismografo = @identificador";
        command.Parameters.AddWithValue("@identificador", sismografo.IdentificadorSismografo);
        
        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }
        
        throw new InvalidOperationException($"Sismografo with identificador '{sismografo.IdentificadorSismografo}' not found in database");
    }

    /// <summary>
    /// Updates the estado of a sismografo
    /// </summary>
    public void UpdateEstado(Sismografo sismografo, Estado estado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var estadoId = _estadoRepository.GetEstadoId(estado);
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Sismografo 
            SET EstadoId = @estadoId 
            WHERE IdentificadorSismografo = @identificador";
        command.Parameters.AddWithValue("@estadoId", estadoId);
        command.Parameters.AddWithValue("@identificador", sismografo.IdentificadorSismografo);
        
        command.ExecuteNonQuery();
        
        // Update in-memory object
        sismografo.Estado = estado;
    }
}

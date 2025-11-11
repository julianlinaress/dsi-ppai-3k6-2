using System;
using System.Collections.Generic;
using System.Linq;
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
    /// EstadoId is stored in the database and kept synchronized with active CambioEstado
    /// </summary>
    private Sismografo MaterializeSismografo(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var sismografo = new Sismografo(nombre);
        
        // Set IdentificadorSismografo using reflection to bypass private setter
        var identificador = reader.GetInt32(reader.GetOrdinal("IdentificadorSismografo"));
        var property = typeof(Sismografo).GetProperty("IdentificadorSismografo");
        property?.SetValue(sismografo, identificador);
        
        // Load Estado from EstadoId foreign key
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
    /// Loads the complete state change history for a sismografo
    /// </summary>
    private List<CambioEstado> LoadCambiosEstado(int sismografoId)
    {
        var cambios = new List<CambioEstado>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CE.CambioEstadoId, CE.FechaHoraInicio, CE.FechaHoraFin, CE.EstadoId
            FROM CambioEstado CE
            WHERE CE.SismografoId = @sismografoId
            ORDER BY CE.FechaHoraInicio";
        command.Parameters.AddWithValue("@sismografoId", sismografoId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var fechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaHoraInicio"));
            var fechaFinOrdinal = reader.GetOrdinal("FechaHoraFin");
            DateTime? fechaFin = reader.IsDBNull(fechaFinOrdinal) ? null : reader.GetDateTime(fechaFinOrdinal);
            var estadoId = reader.GetInt32(reader.GetOrdinal("EstadoId"));
            var estado = _estadoRepository.GetById(estadoId);
            
            if (estado != null)
            {
                var cambio = new CambioEstado(fechaInicio, estado, new List<MotivoFueraServicio>());
                cambio.FechaHoraFin = fechaFin;
                cambios.Add(cambio);
            }
        }
        
        return cambios;
    }

    /// <summary>
    /// Gets all sismografos from the database with complete state change history
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
            var sismografo = MaterializeSismografo(reader);
            var sismografoId = reader.GetInt32(reader.GetOrdinal("SismografoId"));
            
            // Load complete state change history
            sismografo.CambioEstado = LoadCambiosEstado(sismografoId);
            
            sismografos.Add(sismografo);
        }
        
        return sismografos;
    }

    /// <summary>
    /// Gets a sismografo by database ID with complete state change history
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
            var sismografo = MaterializeSismografo(reader);
            var sismografoId = reader.GetInt32(reader.GetOrdinal("SismografoId"));
            
            // Load complete state change history
            sismografo.CambioEstado = LoadCambiosEstado(sismografoId);
            
            return sismografo;
        }
        
        return null;
    }

    /// <summary>
    /// Gets a sismografo by its unique identifier with complete state change history
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
            var sismografo = MaterializeSismografo(reader);
            var sismografoId = reader.GetInt32(reader.GetOrdinal("SismografoId"));
            
            // Load complete state change history
            sismografo.CambioEstado = LoadCambiosEstado(sismografoId);
            
            return sismografo;
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
    /// Maintains synchronization: updates both Sismografo.EstadoId and CambioEstado history
    /// </summary>
    public void UpdateEstado(Sismografo sismografo, Estado nuevoEstado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var sismografoId = GetSismografoId(sismografo);
        var estadoId = _estadoRepository.GetEstadoId(nuevoEstado);
        
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // 1. Close the current CambioEstado (set FechaHoraFin to now)
            using var updateCambioCommand = connection.CreateCommand();
            updateCambioCommand.CommandText = @"
                UPDATE CambioEstado 
                SET FechaHoraFin = @fechaFin 
                WHERE SismografoId = @sismografoId 
                  AND FechaHoraFin IS NULL";
            updateCambioCommand.Parameters.AddWithValue("@fechaFin", DateTime.Now);
            updateCambioCommand.Parameters.AddWithValue("@sismografoId", sismografoId);
            updateCambioCommand.ExecuteNonQuery();
            
            // 2. Insert new CambioEstado
            using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO CambioEstado (SismografoId, EstadoId, FechaHoraInicio, FechaHoraFin)
                VALUES (@sismografoId, @estadoId, @fechaInicio, NULL)";
            insertCommand.Parameters.AddWithValue("@sismografoId", sismografoId);
            insertCommand.Parameters.AddWithValue("@estadoId", estadoId);
            insertCommand.Parameters.AddWithValue("@fechaInicio", DateTime.Now);
            insertCommand.ExecuteNonQuery();
            
            // 3. Update Sismografo.EstadoId to keep it synchronized
            using var updateSismografoCommand = connection.CreateCommand();
            updateSismografoCommand.CommandText = @"
                UPDATE Sismografo 
                SET EstadoId = @estadoId 
                WHERE SismografoId = @sismografoId";
            updateSismografoCommand.Parameters.AddWithValue("@estadoId", estadoId);
            updateSismografoCommand.Parameters.AddWithValue("@sismografoId", sismografoId);
            updateSismografoCommand.ExecuteNonQuery();
            
            transaction.Commit();
            
            // Update in-memory object
            sismografo.Estado = nuevoEstado;
            
            // Reload CambioEstado list to reflect changes
            sismografo.CambioEstado = LoadCambiosEstado(sismografoId);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

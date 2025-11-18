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
    /// EstadoSismografoId is stored in the database and kept synchronized with active CambioEstado
    /// </summary>
    private Sismografo MaterializeSismografo(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        var sismografo = new Sismografo(nombre);
        
        // Set IdentificadorSismografo using reflection to bypass private setter
        var identificador = reader.GetInt32(reader.GetOrdinal("IdentificadorSismografo"));
        var property = typeof(Sismografo).GetProperty("IdentificadorSismografo");
        property?.SetValue(sismografo, identificador);
        
        // Load Estado from EstadoSismografoId foreign key
        var estadoIdOrdinal = reader.GetOrdinal("EstadoSismografoId");
        if (!reader.IsDBNull(estadoIdOrdinal))
        {
            var estadoId = reader.GetInt32(estadoIdOrdinal);
            var estado = _estadoRepository.GetSismografoById(estadoId);
            if (estado != null)
            {
                sismografo.Estado = estado;
            }
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
            SELECT CE.CambioEstadoId, CE.FechaHoraInicio, CE.FechaHoraFin, CE.EstadoSismografoId
            FROM CambioEstado CE
            WHERE CE.SismografoId = @sismografoId
            ORDER BY CE.FechaHoraInicio";
        command.Parameters.AddWithValue("@sismografoId", sismografoId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var cambioEstadoId = reader.GetInt32(reader.GetOrdinal("CambioEstadoId"));
            var fechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaHoraInicio"));
            var fechaFinOrdinal = reader.GetOrdinal("FechaHoraFin");
            DateTime? fechaFin = reader.IsDBNull(fechaFinOrdinal) ? null : reader.GetDateTime(fechaFinOrdinal);
            var estadoId = reader.GetInt32(reader.GetOrdinal("EstadoSismografoId"));
            var estado = _estadoRepository.GetSismografoById(estadoId);
            
            if (estado != null)
            {
                var motivos = LoadMotivosFueraServicio(cambioEstadoId);
                var cambio = new CambioEstado(fechaInicio, estado, motivos)
                {
                    FechaHoraFin = fechaFin
                };
                cambios.Add(cambio);
            }
        }
        
        return cambios;
    }

    /// <summary>
    /// Loads MotivoFueraServicio entries for a given CambioEstado
    /// </summary>
    private List<MotivoFueraServicio> LoadMotivosFueraServicio(int cambioEstadoId)
    {
        var motivos = new List<MotivoFueraServicio>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT M.Comentario, MT.Descripcion
            FROM MotivoFueraServicio M
            INNER JOIN MotivoTipo MT ON MT.MotivoTipoId = M.MotivoTipoId
            WHERE M.CambioEstadoId = @cambioId";
        command.Parameters.AddWithValue("@cambioId", cambioEstadoId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var comentarioOrdinal = reader.GetOrdinal("Comentario");
            string? comentario = reader.IsDBNull(comentarioOrdinal) ? null : reader.GetString(comentarioOrdinal);
            var descripcion = reader.GetString(reader.GetOrdinal("Descripcion"));
            var motivoTipo = new MotivoTipo(descripcion);
            motivos.Add(new MotivoFueraServicio(motivoTipo, comentario));
        }
        return motivos;
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
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoSismografoId 
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
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoSismografoId 
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
            SELECT SismografoId, IdentificadorSismografo, Nombre, EstadoSismografoId 
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
    /// Updates the estado of a sismografo at the specified timestamp
    /// Maintains synchronization: updates both Sismografo.EstadoSismografoId and CambioEstado history
    /// </summary>
    public void UpdateEstado(
        Sismografo sismografo,
        EstadoSismografo nuevoEstado,
        DateTime fechaHora,
        IEnumerable<MotivoFueraServicio>? motivosFueraServicio = null)
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
            updateCambioCommand.Parameters.AddWithValue("@fechaFin", fechaHora);
            updateCambioCommand.Parameters.AddWithValue("@sismografoId", sismografoId);
            updateCambioCommand.ExecuteNonQuery();
            
            // 2. Insert new CambioEstado
            using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO CambioEstado (SismografoId, EstadoSismografoId, FechaHoraInicio, FechaHoraFin)
                VALUES (@sismografoId, @estadoId, @fechaInicio, NULL)";
            insertCommand.Parameters.AddWithValue("@sismografoId", sismografoId);
            insertCommand.Parameters.AddWithValue("@estadoId", estadoId);
            insertCommand.Parameters.AddWithValue("@fechaInicio", fechaHora);
            insertCommand.ExecuteNonQuery();
            var cambioEstadoId = GetLastInsertRowId(connection);

            // 2b. Persist motives (if any)
            if (motivosFueraServicio != null)
            {
                foreach (var motivo in motivosFueraServicio)
                {
                    var motivoTipoId = GetMotivoTipoId(connection, motivo.Motivo.Descripcion);
                    using var insertMotivoCommand = connection.CreateCommand();
                    insertMotivoCommand.CommandText = @"
                        INSERT INTO MotivoFueraServicio (CambioEstadoId, MotivoTipoId, Comentario)
                        VALUES (@cambioId, @motivoTipoId, @comentario)";
                    insertMotivoCommand.Parameters.AddWithValue("@cambioId", cambioEstadoId);
                    insertMotivoCommand.Parameters.AddWithValue("@motivoTipoId", motivoTipoId);
                    insertMotivoCommand.Parameters.AddWithValue("@comentario", (object?)motivo.Comentario ?? DBNull.Value);
                    insertMotivoCommand.ExecuteNonQuery();
                }
            }
            
            // 3. Update Sismografo.EstadoSismografoId to keep it synchronized
            using var updateSismografoCommand = connection.CreateCommand();
            updateSismografoCommand.CommandText = @"
                UPDATE Sismografo 
                SET EstadoSismografoId = @estadoId 
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

    private static long GetLastInsertRowId(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT last_insert_rowid();";
        var result = command.ExecuteScalar();
        return result is long value ? value : Convert.ToInt64(result ?? 0);
    }

    private int GetMotivoTipoId(SqliteConnection connection, string descripcion)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT MotivoTipoId 
            FROM MotivoTipo 
            WHERE Descripcion = @descripcion";
        command.Parameters.AddWithValue("@descripcion", descripcion);
        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }

        throw new InvalidOperationException($"MotivoTipo '{descripcion}' no existe en la base de datos.");
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository for OrdenDeInspeccion entities with materialization/dematerialization logic
/// </summary>
public class OrdenDeInspeccionRepository
{
    private readonly string _connectionString;
    private readonly UsuarioRepository _usuarioRepository;
    private readonly EstadoRepository _estadoRepository;
    private readonly EstacionSismologicaRepository _estacionRepository;

    public OrdenDeInspeccionRepository(string connectionString)
    {
        _connectionString = connectionString;
        _usuarioRepository = new UsuarioRepository(connectionString);
        _estadoRepository = new EstadoRepository(connectionString);
        _estacionRepository = new EstacionSismologicaRepository(connectionString);
    }

    /// <summary>
    /// Materializes an OrdenDeInspeccion from a database row
    /// </summary>
    private OrdenDeInspeccion MaterializeOrden(SqliteDataReader reader)
    {
        var numeroOrden = reader.GetInt32(reader.GetOrdinal("NumeroOrden"));
        var fechaFinalizacion = DateTime.Parse(reader.GetString(reader.GetOrdinal("FechaFinalizacion")));
        var responsableId = reader.GetInt32(reader.GetOrdinal("ResponsableInspeccionId"));
        var estadoId = reader.GetInt32(reader.GetOrdinal("EstadoId"));
        var estacionId = reader.GetInt32(reader.GetOrdinal("EstacionId"));
        
        // Load related entities
        var responsable = _usuarioRepository.GetById(responsableId);
        var estado = _estadoRepository.GetById(estadoId);
        var estacion = _estacionRepository.GetById(estacionId);
        
        if (estado == null)
        {
            throw new InvalidOperationException($"Estado with ID {estadoId} not found");
        }
        if (estacion == null)
        {
            throw new InvalidOperationException($"Estacion with ID {estacionId} not found");
        }
        
        var orden = new OrdenDeInspeccion(numeroOrden, fechaFinalizacion, responsable, estado, estacion);
        
        // Set FechaHoraCierre if present (using reflection to bypass private setter)
        var fechaCierreOrdinal = reader.GetOrdinal("FechaHoraCierre");
        if (!reader.IsDBNull(fechaCierreOrdinal))
        {
            var fechaCierre = DateTime.Parse(reader.GetString(fechaCierreOrdinal));
            var property = typeof(OrdenDeInspeccion).GetProperty("FechaHoraCierre", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            property?.SetValue(orden, fechaCierre);
        }
        
        return orden;
    }

    /// <summary>
    /// Gets all ordenes from the database
    /// </summary>
    public List<OrdenDeInspeccion> GetAll()
    {
        var ordenes = new List<OrdenDeInspeccion>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT OrdenId, NumeroOrden, FechaFinalizacion, FechaHoraCierre, 
                   ResponsableInspeccionId, EstadoId, EstacionId 
            FROM OrdenDeInspeccion 
            ORDER BY NumeroOrden";
        
        System.Diagnostics.Debug.WriteLine("[OrdenDeInspeccionRepository] Executing GetAll query...");
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var numeroOrden = reader.GetInt32(reader.GetOrdinal("NumeroOrden"));
            System.Diagnostics.Debug.WriteLine($"[OrdenDeInspeccionRepository] Materializing orden #{numeroOrden}");
            ordenes.Add(MaterializeOrden(reader));
        }
        
        System.Diagnostics.Debug.WriteLine($"[OrdenDeInspeccionRepository] GetAll completed: {ordenes.Count} ordenes");
        return ordenes;
    }

    /// <summary>
    /// Gets an orden by numero de orden
    /// </summary>
    public OrdenDeInspeccion? GetByNumeroOrden(int numeroOrden)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT OrdenId, NumeroOrden, FechaFinalizacion, FechaHoraCierre, 
                   ResponsableInspeccionId, EstadoId, EstacionId 
            FROM OrdenDeInspeccion 
            WHERE NumeroOrden = @numeroOrden";
        command.Parameters.AddWithValue("@numeroOrden", numeroOrden);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MaterializeOrden(reader);
        }
        
        return null;
    }

    /// <summary>
    /// Gets all ordenes for a specific Responsable de Inspección
    /// </summary>
    public List<OrdenDeInspeccion> GetByResponsable(Usuario responsable)
    {
        var ordenes = new List<OrdenDeInspeccion>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT OrdenId, NumeroOrden, FechaFinalizacion, FechaHoraCierre, 
                   ResponsableInspeccionId, EstadoId, EstacionId 
            FROM OrdenDeInspeccion 
            WHERE ResponsableInspeccionId = @responsableId
            ORDER BY FechaFinalizacion DESC";
        command.Parameters.AddWithValue("@responsableId", responsable.Id);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ordenes.Add(MaterializeOrden(reader));
        }
        
        return ordenes;
    }

    /// <summary>
    /// Gets ordenes completamente realizadas for a specific RI
    /// </summary>
    public List<OrdenDeInspeccion> GetCompletamenteRealizadasByResponsable(Usuario responsable)
    {
        var ordenes = new List<OrdenDeInspeccion>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT o.OrdenId, o.NumeroOrden, o.FechaFinalizacion, o.FechaHoraCierre, 
                   o.ResponsableInspeccionId, o.EstadoId, o.EstacionId 
            FROM OrdenDeInspeccion o
            JOIN Estado e ON o.EstadoId = e.EstadoId
            WHERE o.ResponsableInspeccionId = @responsableId
              AND e.Nombre = 'Completamente Realizada'
              AND e.Ambito = 'Orden de Inspección'
            ORDER BY o.FechaFinalizacion DESC";
        command.Parameters.AddWithValue("@responsableId", responsable.Id);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ordenes.Add(MaterializeOrden(reader));
        }
        
        return ordenes;
    }

    /// <summary>
    /// Updates an orden (dematerialization) - typically after calling Cerrar()
    /// </summary>
    public void Update(OrdenDeInspeccion orden, Estado nuevoEstado, DateTime fechaHoraCierre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var estadoId = _estadoRepository.GetEstadoId(nuevoEstado);
        
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE OrdenDeInspeccion 
            SET EstadoId = @estadoId, 
                FechaHoraCierre = @fechaHoraCierre 
            WHERE NumeroOrden = @numeroOrden";
        command.Parameters.AddWithValue("@estadoId", estadoId);
        command.Parameters.AddWithValue("@fechaHoraCierre", fechaHoraCierre.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@numeroOrden", orden.NumeroOrden);
        
        command.ExecuteNonQuery();
    }
}

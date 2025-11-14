using System;
using Microsoft.Data.Sqlite;
using RedSismica.Models;

namespace RedSismica.Database.Repositories;

/// <summary>
/// Repository para estados de Ordenes de Inspección y estados de Sismógrafos.
/// </summary>
public class EstadoRepository
{
    private readonly string _connectionString;

    public EstadoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private static Estado MaterializeOrden(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        return new Estado(nombre);
    }

    private static EstadoSismografo MaterializeSismografo(SqliteDataReader reader)
    {
        var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
        return nombre switch
        {
            "Inhabilitado" => new Inhabilitado(),
            "Fuera de Servicio" => new FueraDeServicio(),
            "Disponible" => new Disponible(),
            "En Instalación" => new EnInstalacion(),
            "Reclamado" => new Reclamado(),
            "En Línea" => new EnLinea(),
            _ => new EstadoSismografoGenerico(nombre)
        };
    }

    public Estado? GetOrdenById(int estadoId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoOrdenId, Nombre
            FROM EstadoOrden
            WHERE EstadoOrdenId = @id";
        command.Parameters.AddWithValue("@id", estadoId);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MaterializeOrden(reader) : null;
    }

    public Estado? GetOrdenByNombre(string nombre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoOrdenId, Nombre
            FROM EstadoOrden
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", nombre);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MaterializeOrden(reader) : null;
    }

    public EstadoSismografo? GetSismografoById(int estadoId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoSismografoId, Nombre
            FROM EstadoSismografo
            WHERE EstadoSismografoId = @id";
        command.Parameters.AddWithValue("@id", estadoId);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MaterializeSismografo(reader) : null;
    }

    public int GetEstadoId(Estado estado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoOrdenId
            FROM EstadoOrden
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", estado.Nombre);

        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }

        throw new InvalidOperationException($"Estado de orden '{estado.Nombre}' no encontrado en la base de datos");
    }

    public int GetEstadoId(EstadoSismografo estado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EstadoSismografoId
            FROM EstadoSismografo
            WHERE Nombre = @nombre";
        command.Parameters.AddWithValue("@nombre", estado.Nombre);

        var result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }

        throw new InvalidOperationException($"Estado de sismógrafo '{estado.Nombre}' no encontrado en la base de datos");
    }
}

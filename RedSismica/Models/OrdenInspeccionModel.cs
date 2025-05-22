using System;

namespace RedSismica.Models;

public class OrdenDeInspeccion(
    int numeroOrden,
    DateTime fechaFinalizacion,
    Usuario? responsableInspeccion,
    Estado estado,
    EstacionSismologica estacion)
{
    public int NumeroOrden { get; set; } = numeroOrden;
    private DateTime FechaFinalizacion { get; set; } = fechaFinalizacion;
    private DateTime? FechaHoraCierre { get; set; }
    private Usuario? ResponsableInspeccion { get; set; } = responsableInspeccion;
    private Estado Estado { get; set; } = estado;
    public EstacionSismologica Estacion { get; set; } = estacion;

    public bool EsDeRi(Usuario ri)
    {
        return ResponsableInspeccion?.Id == ri.Id;
    }

    public void Cerrar(Estado estado, DateTime fechaHoraCierre)
    {
        FechaHoraCierre = fechaHoraCierre;
        Estado = estado;
    }

    public bool EsCompletamenteRealizada()
    {
        return Estado.EsCompletamenteRealizada();
    }

    public DatosOrdenInspeccion ObtenerDatos()
    {
        return new DatosOrdenInspeccion
        {
            NumeroOrden = NumeroOrden,
            FechaFinalizacion = FechaFinalizacion,
            NombreEstacion = Estacion.Nombre,
            NombreSismografo = Estacion.Sismografo.Nombre
        };
    }
}

public class DatosOrdenInspeccion
{
    public int NumeroOrden { get; init; }
    public DateTime FechaFinalizacion { get; init; }
    public required string  NombreEstacion { get; set; }
    public required string NombreSismografo { get; set; }
}
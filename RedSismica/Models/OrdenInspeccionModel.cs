using System;
using RedSismica.Models;

namespace RedSismica.Models;

public class OrdenDeInspeccion
{
    public int NumeroOrden { get; private set; }
    public DateTime FechaFinalizacion { get; private set; }
    public Usuario ResponsableInspeccion { get; private set; }
    public Estado Estado { get; private set; }
    public EstacionSismologica Estacion { get; private set; }

    public OrdenDeInspeccion(int numeroOrden, DateTime fechaFinalizacion, 
        Usuario responsableInspeccion, Estado estado, EstacionSismologica estacion)
    {
        NumeroOrden = numeroOrden;
        FechaFinalizacion = fechaFinalizacion;
        ResponsableInspeccion = responsableInspeccion;
        Estado = estado;
        Estacion = estacion;
    }

    public bool EsDeRI(Usuario ri)
    {
        return ResponsableInspeccion.Id == ri.Id;
    }

    public bool EsCompletamenteRealizada()
    {
        return Estado.EsCompletamenteRealizada();
    }

    public DatosOrdenInspeccion ObtenerDatos()
    {
        return new DatosOrdenInspeccion
        {
            NumeroOrden = this.NumeroOrden,
            FechaFinalizacion = this.FechaFinalizacion,
            NombreEstacion = this.Estacion.Nombre,
            NombreSismografo = this.Estacion.Sismografo.Nombre
        };
    }
}

public class DatosOrdenInspeccion
{
    public int NumeroOrden { get; set; }
    public DateTime FechaFinalizacion { get; set; }
    public required string  NombreEstacion { get; set; }
    public required string NombreSismografo { get; set; }
}
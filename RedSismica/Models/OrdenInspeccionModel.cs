using System;
using RedSismica.Models;

namespace RedSismica.Models;

/// Clase que representa una orden de inspección
public class OrdenDeInspeccion
{
    // Atributos
    public int NumeroOrden { get; private set; }
    public DateTime FechaFinalizacion { get; private set; }
    public Usuario ResponsableInspeccion { get; private set; }
    public Estado Estado { get; private set; }
    public EstacionSismologica Estacion { get; private set; }

    // Constructor
    public OrdenDeInspeccion(int numeroOrden, DateTime fechaFinalizacion, 
        Usuario responsableInspeccion, Estado estado, EstacionSismologica estacion)
    {
        NumeroOrden = numeroOrden;
        FechaFinalizacion = fechaFinalizacion;
        ResponsableInspeccion = responsableInspeccion;
        Estado = estado;
        Estacion = estacion;
    }

    // Métodos 
    
    // Verifica si la orden de inspección es de un responsable de inspección (RI) específico
    public bool EsDeRI(Usuario ri)
    {
        return ResponsableInspeccion.Id == ri.Id;
    }

    // Verifica si la orden de inspeccion está completamente realizada, osea que el estado es "Completamente Realizada"
    public bool EsCompletamenteRealizada()
    {
        return Estado.EsCompletamenteRealizada();
    }

    // Método para obtener los datos de la orden de inspección
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

/// Clase para almacenar los datos de la orden de inspección
public class DatosOrdenInspeccion
{
    // Atributos
    public int NumeroOrden { get; set; }
    public DateTime FechaFinalizacion { get; set; }
    public required string  NombreEstacion { get; set; }
    public required string NombreSismografo { get; set; }
}
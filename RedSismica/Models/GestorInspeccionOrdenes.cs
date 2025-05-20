using System;
using System.Collections.Generic;
using System.Linq;
using RedSismica.Models;

public class GestorCierreOrdenInspeccion
{
    private readonly Sesion _sesion;
    private List<OrdenDeInspeccion> _ordenesInspeccion;

    public GestorCierreOrdenInspeccion()
    {
        _sesion = new Sesion();
        // Mock data
        InicializarDatosPrueba();
    }

    private void InicializarDatosPrueba()
    {
        var ri = _sesion.obtenerRILogueado();
        var estadoCompletado = new Estado("Completamente Realizada");
        var estadoOtro = new Estado("En Proceso");

        var sismografo1 = new Sismografo("Sismógrafo A123");
        var sismografo2 = new Sismografo("Sismógrafo B456");

        var estacion1 = new EstacionSismologica("Estación Norte", sismografo1);
        var estacion2 = new EstacionSismologica("Estación Sur", sismografo2);

        _ordenesInspeccion = new List<OrdenDeInspeccion>
        {
            new OrdenDeInspeccion(1, DateTime.Now.AddDays(-5), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(2, DateTime.Now.AddDays(-7), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(3, DateTime.Now.AddDays(-12), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(4, DateTime.Now.AddDays(-23), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(5, DateTime.Now.AddDays(-54), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(6, DateTime.Now.AddDays(-2), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(7, DateTime.Now.AddDays(-3), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(8, DateTime.Now.AddDays(-3), ri, estadoCompletado, estacion2),
            new OrdenDeInspeccion(9, DateTime.Now.AddDays(-1), ri, estadoOtro, estacion1)
        };
    }

    public List<DatosOrdenInspeccion> BuscarOrdenes()
    {
        var riLogueado = _sesion.obtenerRILogueado();
        
        return _ordenesInspeccion
            .Where(o => o.EsDeRI(riLogueado) && o.EsCompletamenteRealizada())
            .OrderBy(o => o.FechaFinalizacion)
            .Select(o => o.ObtenerDatos())
            .ToList();
    }
}
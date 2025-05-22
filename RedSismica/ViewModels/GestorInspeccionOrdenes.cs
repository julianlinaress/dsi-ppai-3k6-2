using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RedSismica.Models;
using RedSismica.Views;

namespace RedSismica.ViewModels;

public class GestorCierreOrdenInspeccion(BaseDeDatosMock baseDeDatos, VentanaCierreOrden boundary) : ViewModelBase
{
    // Atributos
    // public string? Mensaje { get; set; }
    private Dictionary<string, string> MotivosYComentarios { get; set; } = new();
    
    private readonly Sesion? _sesion = SesionManager.SesionActual;
    private string? Observacion { get; set; }
    private OrdenDeInspeccion? OrdenSeleccionada { get; set; }
    private Estado? EstadoCierre { get; set; }
    private VentanaCierreOrden _boundary = boundary;
    
    // Metodos
    
    public void NuevoCierre(VentanaCierreOrden ventana)
    {
        _boundary = ventana;
        var riLogueado = _sesion?.ObtenerRILogueado();
        if (riLogueado == null) return;
        var ordenes = BuscarOdenes(riLogueado);
        if (ordenes.Count == 0)
        {
            VentanaCierreOrden.MostrarMensaje("No se encontraron ordenes completamente realizadas para su usuario");
            return;
        }
        var ordenesPorFecha = OrdenarPorFecha(ordenes);
        _boundary.MostrarOrdenesParaSeleccion(ordenesPorFecha);
    }

    private static IOrderedEnumerable<DatosOrdenInspeccion> OrdenarPorFecha(List<DatosOrdenInspeccion> ordenes)
    {
        return ordenes.OrderBy(o => o.FechaFinalizacion);
    }

    private List<DatosOrdenInspeccion> BuscarOdenes(Usuario riLogueado)
    {
        return [.. baseDeDatos.OrdenesDeInspeccion
            .Where(o => o.EsDeRi(riLogueado) && o.EsCompletamenteRealizada())
            .Select(o => o.ObtenerDatos())];
    }
    
    public void TomarSeleccionOrden(DatosOrdenInspeccion orden)
    {
        var ordenSeleccionada = baseDeDatos.OrdenesDeInspeccion.Find(o => orden.NumeroOrden == o.NumeroOrden);
        OrdenSeleccionada = ordenSeleccionada;
        _ = _boundary.PedirObservacion();
    }

    public void TomarTipos(Dictionary<string, string> motivosYComentarios)
    {
        MotivosYComentarios = motivosYComentarios;
        _ = _boundary.PedirConfirmacion();
    }

    public void TomarObservacion(string observacion)
    {
        Observacion = observacion;
        var motivotipos = baseDeDatos.MotivosTipos;
   
        _ = _boundary.PedirTipos(motivotipos);
    }


    private static bool ValidarDatosCierre(string? observacion, Dictionary<string, string> motivosYComentarios)
    {
        return !string.IsNullOrEmpty(observacion) && motivosYComentarios.Count > 0;
    }
    public void TomarConfirmacionCierre()
    {
        var fechaActual = DateTime.Now;
        Debug.WriteLine("Validando datos de cierre...");
        var datosValidos = ValidarDatosCierre(Observacion, MotivosYComentarios);
        if (!datosValidos) return;
        Debug.WriteLine("Buscando estado de cierre...");
        EstadoCierre = BuscarEstadoCierre();
        if (EstadoCierre == null) return;
        Debug.WriteLine("Cerrando orden...");
        OrdenSeleccionada?.Cerrar(EstadoCierre, fechaActual);
        Debug.WriteLine("Poniendo sismografo fuera de servicio...");
        var estadoFueraDeServicioSismografo = ObtenerEstadoFueraDeServicioSismografo();
        if (estadoFueraDeServicioSismografo == null) return;
        PonerSismografoEnFueraDeServicio(estadoFueraDeServicioSismografo);
        var riLogueado = _sesion?.ObtenerRILogueado();
        
        Debug.WriteLine("Orden de inspeccion cerrada correctamente!");
        Debug.WriteLine("Buscando mails de responsables de inspeccion...");

        var mails = ObtenerResponsablesDeInspeccion();
        if (mails.Count > 0)
        {
            Debug.WriteLine("Enviando mails...");
            EnviarEmails(mails, estadoFueraDeServicioSismografo.Nombre, fechaActual);
        }
        
        if (riLogueado == null) return;
        var ordenes = BuscarOdenes(riLogueado);
        if (ordenes.Count == 0)
        {
            VentanaCierreOrden.MostrarMensaje("No se encontraron ordenes completamente realizadas para su usuario");
            return;
        }
        var ordenesPorFecha = OrdenarPorFecha(ordenes);
        
        Debug.WriteLine(OrdenSeleccionada);
        _boundary.MostrarOrdenesParaSeleccion(ordenesPorFecha);
    }

    private void EnviarEmails(List<string?> mails, string nombreEstadoFueraServicio, DateTime fechaHoraCambioEstado)
    {
        var identificacion = OrdenSeleccionada?.NumeroOrden.ToString();
        var mensaje = $"Identificación: {identificacion}\n" +
                         $"Estado: {nombreEstadoFueraServicio}\n" +
                         $"Fecha y Hora: {fechaHoraCambioEstado}\n" +
                         $"Motivos y Comentarios: {FormatearMotivosYComentarios(MotivosYComentarios)}";

        foreach (var mail in mails.Where(m => !string.IsNullOrWhiteSpace(m)))
        {
            Console.WriteLine($"Enviando email a: {mail}");
            Console.WriteLine("Contenido:");
            Console.WriteLine(mensaje);
            Console.WriteLine("--------");
        }
    }


    private List<string?> ObtenerResponsablesDeInspeccion()
    {
        return baseDeDatos.Empleados
            .Where(e => e.EsResponsableDeInspeccion())
            .Select(e => e.Mail)
            .ToList();
    }
    
    private string FormatearMotivosYComentarios(Dictionary<string, string> motivosYComentarios)
    {
        if (motivosYComentarios.Count == 0)
            return "Sin motivos ni comentarios.";

        return string.Join("\n", motivosYComentarios.Select(kvp =>
            $"{kvp.Key}: {kvp.Value}"));
    }
    
    
    private void PonerSismografoEnFueraDeServicio(Estado estadoFueraDeServicioSismografo)
    {
        var fechaHoraActual = DateTime.Now;
        var cambioEstadoActual = OrdenSeleccionada?.Estacion.ObtenerCambioEstadoActual();
        if (cambioEstadoActual == null) return;
        cambioEstadoActual.FechaHoraFin = DateTime.Now;

        if (OrdenSeleccionada == null || EstadoCierre == null) return;
        OrdenSeleccionada.Estado = EstadoCierre;
        OrdenSeleccionada.Estacion.Sismografo.Estado = estadoFueraDeServicioSismografo;
        OrdenSeleccionada.Estacion.Sismografo.CambioEstado =
            new CambioEstado(fechaHoraActual, estadoFueraDeServicioSismografo);
    }
    
    private Estado? BuscarEstadoCierre()
    {
        var estados = baseDeDatos.Estados;
        return estados.Find(o => o.EsAmbitoOrdenInspeccion() && o.EsCerrada());
    }
    
    private Estado? ObtenerEstadoFueraDeServicioSismografo()
    {
        return baseDeDatos.Estados.Find(o => o.EsAmbitoSismografo() && o.EsFueraDeServicio());
    }
}
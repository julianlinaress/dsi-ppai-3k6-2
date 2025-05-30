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
    private Dictionary<MotivoTipo, string> MotivosYComentarios { get; set; } = new();

    private readonly Sesion? _sesion = SesionManager.SesionActual;
    private string? Observacion { get; set; }
    private OrdenDeInspeccion? OrdenSeleccionada { get; set; }
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
            _boundary.MostrarMensaje("No se encontraron ordenes completamente realizadas para su usuario");
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

    public void TomarTipos(Dictionary<MotivoTipo, string> motivosYComentarios)
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


    private static bool ValidarDatosCierre(string? observacion, Dictionary<MotivoTipo, string> motivosYComentarios)
    {
        return !string.IsNullOrEmpty(observacion) && motivosYComentarios.Count > 0;
    }

    private void PonerSismografoEnFueraDeServicio(Estado estadoFueraDeServicioSismografo)
    {
        OrdenSeleccionada?.Estacion.PonerSismografoEnFueraDeServicio(estadoFueraDeServicioSismografo, MotivosYComentarios);
    }
    public void TomarConfirmacionCierre()
    {
        var fechaActual = DateTime.Now;
        
        Debug.WriteLine("Validando datos de cierre...");
        
        if (OrdenSeleccionada == null) return; 
        var datosValidos = ValidarDatosCierre(Observacion, MotivosYComentarios);
        if (!datosValidos) return;
        
        Debug.WriteLine("Buscando estado de cierre...");
        
        var estadoCierre = BuscarEstadoCierre();
        if (estadoCierre == null) return;
        
        Debug.WriteLine("Cerrando orden...");
        
        OrdenSeleccionada.Cerrar(estadoCierre, fechaActual);
        
        Debug.WriteLine("Orden de inspeccion cerrada correctamente!");
        Debug.WriteLine("Obteniendo estado fuera de servicio...");
        
        var estadoFueraDeServicioSismografo = ObtenerEstadoFueraDeServicioSismografo();
        if (estadoFueraDeServicioSismografo == null) return;
        
        Debug.WriteLine("Estado encontrado, actualizando sismógrafo...");
        
        PonerSismografoEnFueraDeServicio(estadoFueraDeServicioSismografo);
        var nroIdentificadorSismografo = OrdenSeleccionada?.Estacion.Sismografo.IdentificadorSismografo;
        
        Debug.WriteLine("Sismografo actualizado, enviando mails...");
        Debug.WriteLine("Buscando mails de responsables de inspeccion...");
        
        var mails = ObtenerResponsablesDeInspeccion();
        if (mails.Count > 0)
        {
            Debug.WriteLine("Enviando mails...");
            EnviarEmails(mails, estadoFueraDeServicioSismografo.Nombre, fechaActual, nroIdentificadorSismografo);
        }
        else
        {
            Debug.WriteLine("No se encontraron mails de responsables de inspeccion...");
        }
        var riLogueado = _sesion?.ObtenerRILogueado();
        if (riLogueado == null) return;
        var ordenes = BuscarOdenes(riLogueado);
        if (ordenes.Count == 0)
        {
            _boundary.MostrarMensaje("No se encontraron ordenes completamente realizadas para su usuario");
            return;
        }
        var ordenesPorFecha = OrdenarPorFecha(ordenes);
        _boundary.MostrarOrdenesParaSeleccion(ordenesPorFecha);
    }

    private void EnviarEmails(
        List<string?> mails, 
        string nombreEstadoFueraServicio, 
        DateTime fechaHoraCambioEstado, 
        int? nroIdentificadorSismografo)
    {
        var mensaje = 
            $"Nro. ID Sismógrafo: {nroIdentificadorSismografo.ToString()}\n" + 
            $"Estado: {nombreEstadoFueraServicio}\n" + 
            $"Fecha y Hora: {fechaHoraCambioEstado}\n" + 
            $"Motivos y Comentarios: {FormatearMotivosYComentarios(MotivosYComentarios)}";

        
        foreach (var mail in mails)
        {
            if (string.IsNullOrEmpty(mail)) return;
            Debug.WriteLine("Enviado mail:");
            Debug.Write(mensaje);
            InterfazEmail.EnviarEmails(mail, "Cierre de Orden de Inspección", mensaje);
        }
    }


    private List<string?> ObtenerResponsablesDeInspeccion()
    {
        return baseDeDatos.Empleados
            .Where(e => e.EsResponsableDeInspeccion() && !string.IsNullOrEmpty(e.Mail))
            .Select(e => e.Mail)
            .ToList();
    }

    private static string FormatearMotivosYComentarios(Dictionary<MotivoTipo, string> motivosYComentarios)
    {
        if (motivosYComentarios.Count == 0)
            return "Sin motivos ni comentarios.";

        return string.Join("\n", motivosYComentarios.Select(kvp =>
            $"{kvp.Key.Descripcion}: {kvp.Value}"));
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
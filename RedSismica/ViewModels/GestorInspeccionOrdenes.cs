using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RedSismica.Models;
using RedSismica.Views;
using RedSismica.Database;

namespace RedSismica.ViewModels;

public class GestorCierreOrdenInspeccion(VentanaCierreOrden boundary) : ViewModelBase
{
    // Atributos
    private Dictionary<MotivoTipo, string> MotivosYComentarios { get; set; } = new();
    private readonly RedSismicaDataContext _context = RedSismicaDataContext.Create();
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
        var ordenes = _context.Ordenes.GetCompletamenteRealizadasByResponsable(riLogueado);
        return ordenes.Select(o => o.ObtenerDatos()).ToList();
    }

    public void TomarSeleccionOrden(DatosOrdenInspeccion orden)
    {
        OrdenSeleccionada = _context.Ordenes.GetByNumeroOrden(orden.NumeroOrden);
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
        // Load motivo tipos from database
        var motivotipos = LoadMotivoTipos();
        _ = _boundary.PedirTipos(motivotipos);
    }

    private List<MotivoTipo> LoadMotivoTipos()
    {
        // For now, create in-memory list since we don't have MotivoTipoRepository yet
        // This matches the seed data
        return
        [
            new MotivoTipo("Reparacion"),
            new MotivoTipo("Renovacion"),
            new MotivoTipo("Cambio de Sismografo"),
            new MotivoTipo("Otro")
        ];
    }


    private static bool ValidarDatosCierre(string? observacion, Dictionary<MotivoTipo, string> motivosYComentarios)
    {
        return !string.IsNullOrEmpty(observacion) && motivosYComentarios.Count > 0;
    }

    private void PonerSismografoEnFueraDeServicio(Estado estadoFueraDeServicioSismografo)
    {
        if (OrdenSeleccionada == null) return;
        
        // Update domain object
        OrdenSeleccionada.Estacion.PonerSismografoEnFueraDeServicio(estadoFueraDeServicioSismografo, MotivosYComentarios);
        
        // Persist to database
        _context.Sismografos.UpdateEstado(OrdenSeleccionada.Estacion.Sismografo, estadoFueraDeServicioSismografo);
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
        
        // Update domain object
        OrdenSeleccionada.Cerrar(estadoCierre, fechaActual);
        
        // Persist orden cierre to database
        _context.Ordenes.Update(OrdenSeleccionada, estadoCierre, fechaActual);
        
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
        
        // Reload ordenes after update
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
        // For now, return hardcoded emails from seed data
        // Later: implement EmpleadoRepository to load from database
        return
        [
            "julian@linares.com.ar"
        ];
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
        return _context.Estados.GetByNombreAndAmbito("Cerrada", "Orden de Inspección");
    }

    private Estado? ObtenerEstadoFueraDeServicioSismografo()
    {
        return _context.Estados.GetByNombreAndAmbito("Fuera de Servicio", "Sismografo");
    }
}
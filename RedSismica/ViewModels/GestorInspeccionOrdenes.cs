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

    private void PonerSismografoEnFueraDeServicio(DateTime fechaActual)
    {
        if (OrdenSeleccionada == null) return;
        
        // Update domain object
        Empleado? responsable = _sesion?.ObtenerRILogueado()?.ObtenerEmpleado();
        OrdenSeleccionada.Estacion.PonerSismografoEnFueraDeServicio(MotivosYComentarios, fechaActual, responsable);
        
        // Persist to database
        var nuevoEstado = OrdenSeleccionada.Estacion.Sismografo.Estado;
        if (nuevoEstado != null)
        {
            var motivos = MotivosYComentarios
                .Select(kvp => new MotivoFueraServicio(kvp.Key, kvp.Value))
                .ToList();
            
            _context.Sismografos.UpdateEstado(
                OrdenSeleccionada.Estacion.Sismografo,
                nuevoEstado,
                fechaActual,
                motivos
            );
        }
    }
    
    public async void TomarConfirmacionCierre()
    {
        var fechaActual = DateTime.Now;
        
        Debug.WriteLine("Validando datos de cierre...");
        
        if (OrdenSeleccionada == null) return; 
        var numeroOrden = OrdenSeleccionada.NumeroOrden;
        var datosValidos = ValidarDatosCierre(Observacion, MotivosYComentarios);
        if (!datosValidos) return;
        
        Debug.WriteLine("Buscando estado de cierre...");
        
        var estadoCierre = BuscarEstadoCierre();
        if (estadoCierre == null) return;
        
        Debug.WriteLine("Cerrando orden...");
        
        OrdenSeleccionada.Cerrar(estadoCierre, fechaActual);
        _context.Ordenes.Update(OrdenSeleccionada, estadoCierre, fechaActual);
        
        Debug.WriteLine("Orden de inspeccion cerrada correctamente!");
        Debug.WriteLine("Actualizando sismógrafo a Inhabilitado...");
        PonerSismografoEnFueraDeServicio(fechaActual);
        var nroIdentificadorSismografo = OrdenSeleccionada?.Estacion.Sismografo.IdentificadorSismografo;
        
        Debug.WriteLine("Sismografo actualizado, enviando mails...");
        Debug.WriteLine("Buscando mails de responsables de inspeccion...");
        
        var mails = ObtenerResponsablesDeInspeccion();
        if (mails.Count > 0)
        {
            Debug.WriteLine("Enviando mails...");
            var nombreEstado = OrdenSeleccionada?.Estacion?.Sismografo?.Estado?.Nombre ?? "Fuera de Servicio";
            _ = EnviarEmailsAsync(mails, nombreEstado, fechaActual, nroIdentificadorSismografo);
        }
        else
        {
            Debug.WriteLine("No se encontraron mails de responsables de inspeccion...");
        }

        await _boundary.MostrarMensajeExito($"La orden #{numeroOrden} se cerró correctamente.");
        
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

    private async System.Threading.Tasks.Task EnviarEmailsAsync(
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

        var total = mails.Count;
        var mainWindow = ObtenerMainWindow();
        int enviados = 0;

        foreach (var mail in mails)
        {
            if (string.IsNullOrEmpty(mail)) continue;
            enviados++;
            mainWindow?.ActualizarEstadoEnvio($"Enviando mails {enviados}/{total}");
            Debug.WriteLine($"[Email] Enviando {enviados}/{total} a {mail}");
            await System.Threading.Tasks.Task.Run(() =>
            {
                InterfazEmail.EnviarEmails(mail, "Cierre de Orden de Inspección", mensaje);
            });
        }

        mainWindow?.ActualizarEstadoEnvio("");
    }

    private static MainWindow? ObtenerMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow as MainWindow;
        }
        return null;
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

    // Eliminado: obtener estado de FdS de repositorio no es necesario, el patrón State lo crea
}

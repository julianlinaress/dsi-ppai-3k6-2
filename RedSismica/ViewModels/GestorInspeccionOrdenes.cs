using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using RedSismica.Models;
using RedSismica.Views;

namespace RedSismica.ViewModels;

using System.Diagnostics;

public class GestorCierreOrdenInspeccion : ViewModelBase
{
    // Atributos
    public string? Mensaje { get; set; }
    public GestorCierreOrdenInspeccion Gestor { get; set; }
    private Dictionary<string, string> MotivosYComentarios { get; set; } = new();
    
    private readonly Sesion? _sesion;
    private List<Usuario> UsuariosList { get; set; }
    private List<MotivoTipo> _motivoTipos = [];
    private List<OrdenDeInspeccion> _ordenesDeInspeccion = [];
    private string? Observacion { get; set; }
    private OrdenDeInspeccion? OrdenSeleccionada { get; set; }
    private VentanaCierreOrden _boundary;
    
    // Constructor
    public GestorCierreOrdenInspeccion()
    {
        List<Usuario> usuariosMock = [
            new(1, "jlinares", "123", true),
            new(2, "mperez", "123", true),
            new(3, "cgomez", "123", true)
        ];
        UsuariosList = usuariosMock;
        _sesion = SesionManager.SesionActual;
        InicializarDatosPrueba();
    }

    // Metodos
    private void InicializarDatosPrueba()
    { 
        var estadoCompletado = new Estado("Completamente Realizada", "Orden de Inspeccion");
        var estadoOtro = new Estado("En Proceso", "Orden de Inspeccion");

        var sismografo1 = new Sismografo("Sismógrafo A123");
        var sismografo2 = new Sismografo("Sismógrafo B456");

        var estacion1 = new EstacionSismologica("Estación Norte", sismografo1);
        var estacion2 = new EstacionSismologica("Estación Sur", sismografo2);
        
        _ordenesDeInspeccion =
        [
            new OrdenDeInspeccion(1, DateTime.Now.AddDays(-5), UsuariosList[0], estadoCompletado, estacion1),
            new OrdenDeInspeccion(2, DateTime.Now.AddDays(-7), UsuariosList[0], estadoCompletado, estacion1),
            new OrdenDeInspeccion(3, DateTime.Now.AddDays(-12), UsuariosList[1], estadoCompletado, estacion1),
            new OrdenDeInspeccion(4, DateTime.Now.AddDays(-23), UsuariosList[0], estadoCompletado, estacion1),
            new OrdenDeInspeccion(5, DateTime.Now.AddDays(-54), UsuariosList[1], estadoCompletado, estacion1),
            new OrdenDeInspeccion(6, DateTime.Now.AddDays(-2), UsuariosList[1], estadoCompletado, estacion1),
            new OrdenDeInspeccion(7, DateTime.Now.AddDays(-3), UsuariosList[0], estadoCompletado, estacion1),
            new OrdenDeInspeccion(8, DateTime.Now.AddDays(-3), UsuariosList[1], estadoCompletado, estacion2),
            new OrdenDeInspeccion(9, DateTime.Now.AddDays(-1), UsuariosList[1], estadoOtro, estacion1)
        ];
    }
    
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
        return [.. _ordenesDeInspeccion
            .Where(o => o.EsDeRi(riLogueado) && o.EsCompletamenteRealizada())
            .Select(o => o.ObtenerDatos())];
    }
    
    public void TomarSeleccionOrden(DatosOrdenInspeccion orden)
    {
        var ordenSeleccionada = _ordenesDeInspeccion.Find(o => orden.NumeroOrden == o.NumeroOrden);
        OrdenSeleccionada = ordenSeleccionada;
        _boundary.PedirObservacion();
    }

    public void TomarTipos(Dictionary<string, string> motivosYComentarios)
    {
        MotivosYComentarios = motivosYComentarios;
        _boundary.PedirConfirmacion();
    }

    public void TomarObservacion(string observacion)
    {
        Observacion = observacion;
        _motivoTipos =
        [
            new MotivoTipo("Reparacion"),
            new MotivoTipo("Renovacion"),
            new MotivoTipo("Cambio de Sismografo"),
            new MotivoTipo("Otro")
        ];   
        _boundary.PedirTipos(_motivoTipos);
    }


    private static bool ValidarDatosCierre(string? observacion, Dictionary<string, string> motivosYComentarios)
    {
        return !string.IsNullOrEmpty(observacion) && motivosYComentarios.Count > 0;
    }
    public void TomarConfirmacionCierre()
    {
        var datosValidos = ValidarDatosCierre(Observacion, MotivosYComentarios);
        if (!datosValidos) return;
        var estado = BuscarEstadoCierre();
        if (estado == null) return;
        var fechaActual = DateTime.Now;
        OrdenSeleccionada?.Cerrar(estado, fechaActual);
    }

    private static Estado? BuscarEstadoCierre()
    {
        
        List<Estado> estados =
        [
            new("Algun Estado", "Orden de Inspeccion"),
            
            new("Estado Inicial", "Orden de Inspeccion"),
            
            new("Fuera de Orden", "Sismografo"),
            
            new("Cerrada", "Orden de Inspeccion"),
        ];
        return estados.Find(o => o.EsAmbitoOrdenInspeccion() && o.EsCerrada());
    }
}
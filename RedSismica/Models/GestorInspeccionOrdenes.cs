using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using RedSismica.ViewModels;

namespace RedSismica.Models;

// Control.
public class GestorCierreOrdenInspeccion
{
    // Atributos
    private readonly Sesion _sesion;
    private List<MotivoTipo> _motivoTipos = [];
    private List<OrdenDeInspeccion> _ordenesDeInspeccion = [];
    private string? Observacion { get; set; }
    private VentanaCierreViewModel? _boundary;
    
    // Constructor
    public GestorCierreOrdenInspeccion()
    {
        _sesion = new Sesion();
        // Mock data
        InicializarDatosPrueba();
    }

    // Metodos
    private void InicializarDatosPrueba()
    { 
        var ri = _sesion.obtenerRILogueado();
        var estadoCompletado = new Estado("Completamente Realizada");
        var estadoOtro = new Estado("En Proceso");

        var sismografo1 = new Sismografo("Sismógrafo A123");
        var sismografo2 = new Sismografo("Sismógrafo B456");

        var estacion1 = new EstacionSismologica("Estación Norte", sismografo1);
        var estacion2 = new EstacionSismologica("Estación Sur", sismografo2);

        _motivoTipos =
        [
            new MotivoTipo("Reparacion"),
            new MotivoTipo("Renovacion"),
            new MotivoTipo("Cambio de Sismografo"),
            new MotivoTipo("Otro")
        ];        
        _ordenesDeInspeccion =
        [
            new OrdenDeInspeccion(1, DateTime.Now.AddDays(-5), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(2, DateTime.Now.AddDays(-7), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(3, DateTime.Now.AddDays(-12), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(4, DateTime.Now.AddDays(-23), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(5, DateTime.Now.AddDays(-54), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(6, DateTime.Now.AddDays(-2), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(7, DateTime.Now.AddDays(-3), ri, estadoCompletado, estacion1),
            new OrdenDeInspeccion(8, DateTime.Now.AddDays(-3), ri, estadoCompletado, estacion2),
            new OrdenDeInspeccion(9, DateTime.Now.AddDays(-1), ri, estadoOtro, estacion1)
        ];
    }
    
    public void NuevoCierre(VentanaCierreViewModel ventana)
    {
        _boundary = ventana;
        var riLogueado = _sesion.obtenerRILogueado();
        var ordenes = BuscarOdenes(riLogueado);
        var ordenesPorFecha = OrdenarPorFecha(ordenes);
        _boundary.MostrarOrdenesParaSeleccion(ordenesPorFecha);
    }

    private static IOrderedEnumerable<DatosOrdenInspeccion> OrdenarPorFecha(List<DatosOrdenInspeccion> ordenes)
    {
        return ordenes.OrderBy(o => o.FechaFinalizacion);
    }

    private List<DatosOrdenInspeccion> BuscarOdenes(Usuario riLogueado)
    {
        return _ordenesDeInspeccion
            .Where(o => o.EsDeRI(riLogueado) && o.EsCompletamenteRealizada())
            .Select(o => o.ObtenerDatos())
            .ToList();
    }
    
    public async Task ProcesarOrdenSeleccionada(DatosOrdenInspeccion orden, Window parentWindow)
    {
        var input = await PedirObservacion(parentWindow);
        if (input == null)
        {
            return; // El usuario canceló/cerró la ventana, se cancela el metodo.
        }
        Observacion = input;
        Debug.WriteLine($"Orden seleccionada: {orden.NumeroOrden}, observación: {Observacion}");

        var motivosYComentarios = await PedirTipos(parentWindow);
        if (motivosYComentarios == null)
        {
            return; // El usuario canceló/cerró la ventana, se cancela el metodo.
        }
        Debug.WriteLine($"Comentarios: {motivosYComentarios}");
    }

    private async Task<string?> PedirObservacion(Window parent)
    {
        var textBox = new TextBox
        {
            Name = "ObservacionInput",
            Height = 80,
            AcceptsReturn = true
        };

        var aceptarClicked = false; // Se usará para determinar si el botón aceptar fue presionado
        var button = new Button
        {
            Content = "Aceptar",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0),
            IsDefault = true,
            IsEnabled = false // Deshabilitado inicialmente
        };

        textBox.GetObservable(TextBox.TextProperty).Subscribe(text =>
        {
            button.IsEnabled = !string.IsNullOrWhiteSpace(text); // Habilitar solo si hay texto
        });

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Children =
            {
                new TextBlock { Text = "Ingrese una observación:" },
                textBox,
                button
            }
        };

        var dialog = new Window
        {
            Title = "Observación",
            Width = 300,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = panel
        };

        button.Click += (_, _) =>
        {
            aceptarClicked = true; // Indicar que se aceptó
            dialog.Close();
        };

        await dialog.ShowDialog(parent);

        // Si no se presionó "Aceptar", devolver null
        return aceptarClicked ? textBox.Text : null;
    }
    
    private async Task<Dictionary<string, string>?> PedirTipos(Window parent)
    {
        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 10
        };
        var aceptarClicked = false; // Se usará para determinar si el botón aceptar fue presionado

        // Diccionarios para mantener estado
        var checkBoxes = new Dictionary<string, CheckBox>();
        var commentBoxes = new Dictionary<string, TextBox>();

        foreach (var motivo in _motivoTipos)
        {
            var motivoPanel = new StackPanel { Spacing = 5 };

            var check = new CheckBox
            {
                Content = motivo.Descripcion
            };

            var comentario = new TextBox
            {
                IsEnabled = false,
                Watermark = "Comentario...",
                Height = 60,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            // Habilitar el cuadro de texto solo cuando el motivo está seleccionado
            check.IsCheckedChanged += (_, _) => comentario.IsEnabled = check.IsChecked == true;

            motivoPanel.Children.Add(check);
            motivoPanel.Children.Add(comentario);

            panel.Children.Add(motivoPanel);

            checkBoxes[motivo.Descripcion] = check;
            commentBoxes[motivo.Descripcion] = comentario;
        }

        var aceptarButton = new Button
        {
            Content = "Aceptar",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            IsDefault = true,
            Margin = new Thickness(0, 10, 0, 0),
            IsEnabled = false // Inicialmente deshabilitado
        };

        // Función para verificar si al menos un CheckBox está seleccionado
        void UpdateButtonIsEnabled()
        {
            aceptarButton.IsEnabled = checkBoxes.Values.Any(cb => cb.IsChecked == true);
        }

        // Suscribir cada CheckBox al evento IsCheckedChanged
        foreach (var checkBox in checkBoxes.Values)
        {
            checkBox.IsCheckedChanged += (_, _) => UpdateButtonIsEnabled();
        }

        var dialog = new Window
        {
            Title = "Motivos",
            Width = 400,
            Height = 500,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new ScrollViewer
            {
                Content = new StackPanel
                {
                    Children =
                    {
                        panel,
                        aceptarButton // Agregar el botón después de los motivos
                    }
                }
            }
        };

        aceptarButton.Click += (_, _) =>
        {
            aceptarClicked = true;
            // Cierra el diálogo
            dialog.Close();
        };

        await dialog.ShowDialog(parent);

        // Obtener resultados seleccionados
        var resultado = new Dictionary<string, string>();

        foreach (var motivo in _motivoTipos)
        {
            var check = checkBoxes[motivo.Descripcion];
            var comentario = commentBoxes[motivo.Descripcion];

            if (check.IsChecked == true)
            {
                resultado[motivo.Descripcion] = comentario.Text ?? "";
            }
        }

        return aceptarClicked ? resultado : null;
    }

}
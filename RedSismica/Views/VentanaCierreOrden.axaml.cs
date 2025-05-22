using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using RedSismica.Models;
using RedSismica.ViewModels;

namespace RedSismica.Views;

public partial class VentanaCierreOrden : Window
{
    private GestorCierreOrdenInspeccion? Gestor { get; set; }
    public VentanaCierreOrden()
    {
        InitializeComponent();
        if (Program.BaseDeDatosMock == null) return;
        Gestor = new GestorCierreOrdenInspeccion(Program.BaseDeDatosMock, this);
        Gestor.NuevoCierre(this);
        
        var screen = Screens.Primary;
        if (screen != null)
        {
            var workingArea = screen.WorkingArea;
            Width = workingArea.Width * 0.8;
            Height = workingArea.Height * 0.8;
        }
        else
        {
            Width = 1024;
            Height = 768;
        }
    }

    private void BotonCancelar_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void TomarSeleccionOrden(object? sender, RoutedEventArgs e)
    {
        if (OrdenesDataGrid.SelectedItem is not DatosOrdenInspeccion seleccion) return;
        Gestor?.TomarSeleccionOrden(seleccion);
    }

    public void MostrarMensaje(string mensaje)
    {
        OrdenesDataGrid.IsVisible = false;
        MensajeTextBlock.Text = mensaje;
        MensajeTextBlock.IsVisible = true;
    }


    public void MostrarOrdenesParaSeleccion(IOrderedEnumerable<DatosOrdenInspeccion> ordenesData)
    {
        OrdenesDataGrid.ItemsSource = ordenesData;
        // Ordenes = new ObservableCollection<DatosOrdenInspeccion>(ordenesData);
    }
    
    public async Task PedirConfirmacion()
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Confirmacion", "¿Confirma el cierre de la orden de inspección?",
                ButtonEnum.OkCancel);

        var result = await box.ShowAsync();
        if (result == ButtonResult.Ok)
        {
            Gestor?.TomarConfirmacionCierre();
        }
        
    }

    public async Task PedirObservacion()
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

        textBox.TextChanged += (_, _) =>
        {
            button.IsEnabled = !string.IsNullOrWhiteSpace(textBox.Text);
        };
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

        await dialog.ShowDialog(this);
        if (!aceptarClicked || string.IsNullOrEmpty(textBox.Text)) return;
        // Si no se presionó "Aceptar", devolver null
        Gestor?.TomarObservacion(textBox.Text);
    }
    
        
    public async Task PedirTipos(List<MotivoTipo> motivoTipos)
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

        foreach (var motivo in motivoTipos)
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

        await dialog.ShowDialog(this);

        // Obtener resultados seleccionados
        var resultado = new Dictionary<MotivoTipo, string>();

        foreach (var motivo in motivoTipos)
        {
            var check = checkBoxes[motivo.Descripcion];
            var comentario = commentBoxes[motivo.Descripcion];

            if (check.IsChecked == true)
            {
                resultado[motivo] = comentario.Text ?? "";
            }
        }

        if (aceptarClicked) {
            Gestor?.TomarTipos(resultado);
        }

        return;

        // Función para verificar si al menos un CheckBox está seleccionado
        void UpdateButtonIsEnabled()
        {
            aceptarButton.IsEnabled = checkBoxes.Values.Any(cb => cb.IsChecked == true);
        }
    }
}
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RedSismica.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var usuarioActual = SesionManager.SesionActual?.ObtenerRILogueado();
        if (usuarioActual == null) return;
        var nombreCompleto = usuarioActual.Nombre;
        var textBlock = new TextBlock
        {
            Text = nombreCompleto,
            FontSize = 20,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        };
        this.FindControl<StackPanel>("MainPanel")?.Children.Insert(0, textBlock);
    }
    
    
    private void SeleccionarOpcionRegistarCierre(object? sender, RoutedEventArgs e)
    {
        var ventanaCierre = new VentanaCierreOrden();
        ventanaCierre.Show();
    }
}
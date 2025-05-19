using System.Diagnostics;
using Avalonia;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RedSismica.ViewModels;

namespace RedSismica.Views;

public partial class VentanaCierreOrden : Window
{
    public VentanaCierreOrden()
    {
        InitializeComponent();
        DataContext = new VentanaCierreViewModel();
        
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

    public class OrdenInspeccionListadoItem
    {
        public int NumeroOrden { get; set; }
        public required string FechaFinalizacion { get; set; }
        public required string NombreEstacion { get; set; }
        public required string NombreSismografo { get; set; }
    }

    private void BotonCancelar_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
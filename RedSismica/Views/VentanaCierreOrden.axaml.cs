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
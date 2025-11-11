using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RedSismica.Models;

namespace RedSismica.Views;

public partial class VentanaHistorialEstados : Window
{
    public VentanaHistorialEstados(Sismografo sismografo)
    {
        InitializeComponent();
        CargarHistorial(sismografo);
    }

    private void CargarHistorial(Sismografo sismografo)
    {
        // Update sismografo info header
        var infoTextBlock = this.FindControl<TextBlock>("SismografoInfo");
        if (infoTextBlock != null)
        {
            infoTextBlock.Text = $"Sismógrafo: {sismografo.Nombre} (ID: {sismografo.IdentificadorSismografo})";
        }

        // Convert CambioEstado to DTO for display
        var historialData = sismografo.CambioEstado?
            .OrderByDescending(c => c.FechaHoraInicio)
            .Select(cambio => new DatosHistorialCambioEstado
            {
                Estado = cambio.Estado.Nombre,
                FechaHoraInicio = cambio.FechaHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                FechaHoraFin = cambio.FechaHoraFin?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Actual",
                Duracion = CalcularDuracion(cambio.FechaHoraInicio, cambio.FechaHoraFin),
                Motivos = FormatearMotivos(cambio.Motivos)
            })
            .ToList() ?? new List<DatosHistorialCambioEstado>();

        var dataGrid = this.FindControl<DataGrid>("HistorialDataGrid");
        if (dataGrid != null)
        {
            dataGrid.ItemsSource = historialData;
        }
    }

    private string CalcularDuracion(DateTime inicio, DateTime? fin)
    {
        var finReal = fin ?? DateTime.Now;
        var duracion = finReal - inicio;

        if (duracion.TotalDays >= 1)
        {
            return $"{(int)duracion.TotalDays} día(s)";
        }
        else if (duracion.TotalHours >= 1)
        {
            return $"{(int)duracion.TotalHours} hora(s)";
        }
        else if (duracion.TotalMinutes >= 1)
        {
            return $"{(int)duracion.TotalMinutes} minuto(s)";
        }
        else
        {
            return "< 1 minuto";
        }
    }

    private string FormatearMotivos(List<MotivoFueraServicio> motivos)
    {
        if (motivos == null || motivos.Count == 0)
        {
            return "-";
        }

        return string.Join("; ", motivos.Select(m => 
        {
            var motivoTexto = m.Motivo.Descripcion;
            if (!string.IsNullOrWhiteSpace(m.Comentario))
            {
                motivoTexto += $" ({m.Comentario})";
            }
            return motivoTexto;
        }));
    }

    private void Cerrar(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

/// <summary>
/// DTO for displaying state change history in DataGrid
/// </summary>
public class DatosHistorialCambioEstado
{
    public string Estado { get; set; } = string.Empty;
    public string FechaHoraInicio { get; set; } = string.Empty;
    public string FechaHoraFin { get; set; } = string.Empty;
    public string Duracion { get; set; } = string.Empty;
    public string Motivos { get; set; } = string.Empty;
}

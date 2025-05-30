 using System;
 using System.Collections.Generic;

 namespace RedSismica.Models;

public class EstacionSismologica(string nombre, Sismografo sismografo)
{
    public string Nombre { get; private set; } = nombre;
    public Sismografo Sismografo { get; } = sismografo;
    
    private CambioEstado? ObtenerCambioEstadoActual()
    {
        return Sismografo.CambioEstado;

    }
    
    public void PonerSismografoEnFueraDeServicio(Estado estadoFueraDeServicioSismografo, Dictionary<MotivoTipo, string> motivosYComentarios)
    {
        var fechaHoraActual = DateTime.Now;
        var cambioEstadoActual = ObtenerCambioEstadoActual();
        if (cambioEstadoActual == null) return;
        cambioEstadoActual.FechaHoraFin = DateTime.Now;
        Sismografo.Estado = estadoFueraDeServicioSismografo;
        List<MotivoFueraServicio> motivosFueraServicio = [];
        foreach (var motivo in motivosYComentarios)
        {
            var motivoFueraServicio = new MotivoFueraServicio(motivo.Key, motivo.Value);
            motivosFueraServicio.Add(motivoFueraServicio);
        }
        Sismografo.CambioEstado = new CambioEstado(fechaHoraActual, estadoFueraDeServicioSismografo, motivosFueraServicio);
    }
}
using System;

namespace RedSismica.Models;

public class CambioEstado(DateTime fechaHoraInicio, Estado estado)
{
    private DateTime FechaHoraInicio { get; set; } = fechaHoraInicio;
    public required Estado Estado { get; set; } = estado;
    public DateTime? FechaHoraFin { get; set; }

    public bool EsEstadoActual()
    {
        var fechaActual = DateTime.Now;
        return fechaActual >= FechaHoraInicio && FechaHoraFin == null;
    }
}
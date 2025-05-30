using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class CambioEstado(DateTime fechaHoraInicio, Estado estado, List<MotivoFueraServicio> motivosFueraServicio)
{
    private DateTime FechaHoraInicio { get; set; } = fechaHoraInicio;

    public List<MotivoFueraServicio>? MotivosFueraServicio { get; set; } = motivosFueraServicio;
    public Estado Estado { get; set; } = estado;
    public DateTime? FechaHoraFin { get; set; }

    public bool EsEstadoActual()
    {
        var fechaActual = DateTime.Now;
        return fechaActual >= FechaHoraInicio && FechaHoraFin == null;
    }
}
using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class CambioEstado(DateTime fechaHoraInicio, EstadoSismografo estado, List<MotivoFueraServicio> motivosFueraServicio)
{
    public DateTime FechaHoraInicio { get; set; } = fechaHoraInicio;

    public List<MotivoFueraServicio> Motivos { get; set; } = motivosFueraServicio;
    public EstadoSismografo Estado { get; set; } = estado;
    public DateTime? FechaHoraFin { get; set; }

    public bool EsEstadoActual()
    {
        return FechaHoraFin == null;
    }
}

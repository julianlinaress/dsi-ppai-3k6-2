using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class CambioEstado(DateTime fechaHoraInicio, Estado estado, List<MotivoFueraServicio> motivosFueraServicio)
{
    public DateTime FechaHoraInicio { get; set; } = fechaHoraInicio;

    public List<MotivoFueraServicio> Motivos { get; set; } = motivosFueraServicio;
    public Estado Estado { get; set; } = estado;
    public DateTime? FechaHoraFin { get; set; }

    public bool EsEstadoActual()
    {
        return FechaHoraFin == null;
    }
}
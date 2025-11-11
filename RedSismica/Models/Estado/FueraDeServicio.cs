using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class FueraDeServicio : Estado
{
    public FueraDeServicio() : base("Fuera de Servicio", "Sismografo")
    {
    }

    public override bool EsFueraDeServicio() => true;

    public override void PonerFueraDeServicio(
        Empleado responsable,
        List<MotivoFueraServicio> motivos,
        DateTime fechaYHora,
        Sismografo self,
        List<CambioEstado> cambiosDeEstado)
    {
        // Ya está fuera de servicio, no hace nada
    }
}

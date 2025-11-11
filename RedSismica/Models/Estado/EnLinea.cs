using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class EnLinea : Estado
{
    public EnLinea() : base("En Línea", "Sismografo")
    {
    }

    public override void PonerFueraDeServicio(
        Empleado responsable,
        List<MotivoFueraServicio> motivos,
        DateTime fechaYHora,
        Sismografo self,
        List<CambioEstado> cambiosDeEstado)
    {
        // Finalizar el cambio de estado actual
        var cambioActual = cambiosDeEstado.Find(c => c.EsEstadoActual());
        if (cambioActual != null)
        {
            cambioActual.FechaHoraFin = fechaYHora;
        }

        // Crear nuevo estado
        var nuevoEstado = CrearNuevoEstado();
        
        // Crear nuevo cambio de estado
        var nuevoCambio = CrearNuevoCambioDeEstado(nuevoEstado, fechaYHora, motivos, responsable);
        
        // Actualizar sismógrafo
        self.Estado = nuevoEstado;
        cambiosDeEstado.Add(nuevoCambio);
    }

    private static FueraDeServicio CrearNuevoEstado()
    {
        return new FueraDeServicio();
    }

    private static CambioEstado CrearNuevoCambioDeEstado(
        Estado estado,
        DateTime fechaYHora,
        List<MotivoFueraServicio> motivos,
        Empleado responsable)
    {
        return new CambioEstado(fechaYHora, estado, motivos);
    }
}

using System;
using System.Collections.Generic;

namespace RedSismica.Models;

/// <summary>
/// Clase base abstracta para los estados del Sismógrafo.
/// Implementa el comportamiento del patrón State.
/// </summary>
public abstract class EstadoSismografo
{
    public string Nombre { get; protected set; } = string.Empty;

    public virtual bool EsFueraDeServicio() => false;
    public virtual bool EsInhabilitado() => false;
    public virtual bool EsDisponible() => false;
    public virtual bool EsEnInstalacion() => false;
    public virtual bool EsReclamado() => false;
    public virtual bool EsEnLinea() => false;

    public virtual void FueraDeServicio(
        Empleado responsable,
        List<MotivoFueraServicio> motivos,
        DateTime fechaYHora,
        Sismografo self,
        List<CambioEstado> cambiosDeEstado)
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
        public virtual void Habilitar()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void AsignarProyectoIns()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void ComenzarInstalacion()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void Reclamar()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void Reanudar()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void Conectar()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
    public virtual void Inhabilitar()
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
}

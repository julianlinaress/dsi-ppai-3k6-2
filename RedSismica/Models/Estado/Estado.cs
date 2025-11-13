using System;
using System.Collections.Generic;

namespace RedSismica.Models;

/// <summary>
/// Clase base abstracta para estados
/// - Estados de Sismógrafo: implementan patrón State (EnLinea, FueraDeServicio)
/// - Estados de Orden: usan comparación por nombre sin patrón State (EstadoGenerico)
/// </summary>
public abstract class Estado
{
    public string Nombre { get; protected set; }
    public string Ambito { get; protected set; }

    protected Estado(string nombre, string ambito)
    {
        Nombre = nombre;
        Ambito = ambito;
    }

    // Métodos para Orden de Inspección (sin patrón State - comparación por nombre)
    public bool EsCompletamenteRealizada() => Nombre == "Completamente Realizada";
    public bool EsAmbitoOrdenInspeccion() => Ambito == "Orden de Inspección";
    public bool EsCerrada() => Nombre == "Cerrada";
    
    // Métodos para Sismógrafo (con patrón State - override en subclases)
    public virtual bool EsAmbitoSismografo() => Ambito == "Sismografo";
    public virtual bool EsFueraDeServicio() => false;
    public virtual bool EsInhabilitado() => false;
    
    public virtual void FueraDeServicio(
        Empleado responsable,
        List<MotivoFueraServicio> motivos,
        DateTime fechaYHora,
        Sismografo self,
        List<CambioEstado> cambiosDeEstado)
    {
        throw new NotImplementedException("Método no implementado para este estado.");
    }
}
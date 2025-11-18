namespace RedSismica.Models;

/// <summary>
/// Representa un estado simple de una Orden de Inspección (sin patrón State).
/// </summary>
public class Estado(string nombre, string ambito)
{
    public string Nombre { get; private set; } = nombre;
    public string Ambito { get; private set; } = ambito;

    public bool EsCompletamenteRealizada() => Nombre == "Completamente Realizada";
    public bool EsDeOrdenDeInspeccion() => Ambito == "Orden de Inspección";
    public bool EsCerrada() => Nombre == "Cerrada";
}

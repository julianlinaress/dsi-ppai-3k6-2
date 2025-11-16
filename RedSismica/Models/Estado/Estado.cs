namespace RedSismica.Models;

/// <summary>
/// Representa un estado simple de una Orden de Inspección (sin patrón State).
/// </summary>
public class Estado
{
    public Estado(string nombre, string ambito)
    {
        Nombre = nombre;
        Ambito = ambito;
    }

    public string Nombre { get; protected set; }
    public string Ambito { get; protected set; }

    public bool EsCompletamenteRealizada() => Nombre == "Completamente Realizada";
    public bool EsDeOrdenDeInspeccion() => Ambito == "Orden de Inspección";
    public bool EsCerrada() => Nombre == "Cerrada";
}

namespace RedSismica.Models;

/// <summary>
/// Representa un estado simple de una Orden de Inspección (sin patrón State).
/// </summary>
public class Estado
{
    public Estado(string nombre)
    {
        Nombre = nombre;
    }

    public string Nombre { get; protected set; }

    public bool EsCompletamenteRealizada() => Nombre == "Completamente Realizada";
    public bool EsCerrada() => Nombre == "Cerrada";
}

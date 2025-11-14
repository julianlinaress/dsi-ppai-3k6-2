namespace RedSismica.Models;

/// <summary>
/// Estado de Sismógrafo sin comportamiento personalizado.
/// </summary>
public class EstadoSismografoGenerico : EstadoSismografo
{
    public EstadoSismografoGenerico(string nombre) : base(nombre)
    {
    }
}

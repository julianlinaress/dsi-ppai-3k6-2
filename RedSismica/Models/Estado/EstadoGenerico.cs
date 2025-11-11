
namespace RedSismica.Models;

/// <summary>
/// Estado genérico para estados que no requieren comportamiento polimórfico completo
/// Usado principalmente para estados de Orden de Inspección
/// </summary>
public class EstadoGenerico : Estado
{
    public EstadoGenerico(string nombre, string ambito) : base(nombre, ambito)
    {
    }
}

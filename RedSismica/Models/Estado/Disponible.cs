namespace RedSismica.Models;

public class Disponible : EstadoSismografo
{
    public Disponible() : base("Disponible")
    {
    }

    public override bool EsDisponible() => true;
}

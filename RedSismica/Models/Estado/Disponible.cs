namespace RedSismica.Models;

public class Disponible : EstadoSismografo
{
    public Disponible()
    {
        Nombre = "Disponible";
    }

    public override bool EsDisponible() => true;
}

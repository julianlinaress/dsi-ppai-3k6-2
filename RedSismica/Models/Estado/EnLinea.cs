namespace RedSismica.Models;

public class EnLinea : EstadoSismografo
{
    public EnLinea() : base("En Línea")
    {
    }

    public override bool EsEnLinea() => true;
}

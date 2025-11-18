namespace RedSismica.Models;

public class EnLinea : EstadoSismografo
{
    public EnLinea()
    {
        Nombre = "En Línea";
    }

    public override bool EsEnLinea() => true;
}

namespace RedSismica.Models;

public class FueraDeServicio : EstadoSismografo
{
    public FueraDeServicio() : base("Fuera de Servicio")
    {
    }

    public override bool EsFueraDeServicio() => true;
}

namespace RedSismica.Models;

public class FueraDeServicio : EstadoSismografo
{
    public FueraDeServicio()
    {
        Nombre = "Fuera de Servicio";
    }

    public override bool EsFueraDeServicio() => true;
}

namespace RedSismica.Models;

public class EnInstalacion : EstadoSismografo
{
    public EnInstalacion() : base("En Instalación")
    {
    }

    public override bool EsEnInstalacion() => true;
}

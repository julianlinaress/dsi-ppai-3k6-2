namespace RedSismica.Models;

public class EnInstalacion : EstadoSismografo
{
    public EnInstalacion()
    {
        Nombre = "En Instalación";
    }

    public override bool EsEnInstalacion() => true;
}

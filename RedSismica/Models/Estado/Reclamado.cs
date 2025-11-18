namespace RedSismica.Models;

public class Reclamado : EstadoSismografo
{
    public Reclamado()
    {
        Nombre = "Reclamado";
    }

    public override bool EsReclamado() => true;
}

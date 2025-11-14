namespace RedSismica.Models;

public class Reclamado : EstadoSismografo
{
    public Reclamado() : base("Reclamado")
    {
    }

    public override bool EsReclamado() => true;
}

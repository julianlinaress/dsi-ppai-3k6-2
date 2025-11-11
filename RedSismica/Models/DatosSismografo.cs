namespace RedSismica.Models;

public class DatosSismografo
{
    public int Identificador { get; init; }
    public required string Nombre { get; init; }
    public required string Estado { get; init; }
    
    // Internal reference to full sismografo for accessing state history
    internal Sismografo? SismografoCompleto { get; init; }
}

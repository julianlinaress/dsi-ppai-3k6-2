namespace RedSismica.Models;

public class Sismografo(string nombre)
{
    public string Nombre { get; private set; } = nombre;
    
    public CambioEstado? CambioEstado { get; set; }
    public Estado? Estado { get; set; }
}
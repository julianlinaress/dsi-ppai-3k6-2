using System.Collections.Generic;

namespace RedSismica.Models;

public class Sismografo(string nombre)
{
    private static int _contador = 18122021;
    public string Nombre { get; private set; } = nombre;

    public int IdentificadorSismografo { get; private set; } = _contador++;
    public List<CambioEstado>? CambioEstado { get; set; }
    public Estado? Estado { get; set; }
}
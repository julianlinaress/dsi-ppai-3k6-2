using System.Collections.Generic;
using System.Linq;

namespace RedSismica.Models;

public class Sismografo(string nombre)
{
    private static int _contador = 18122021;
    public string Nombre { get; private set; } = nombre;

    public int IdentificadorSismografo { get; private set; } = _contador++;
    public List<CambioEstado>? CambioEstado { get; set; }
    
    /// <summary>
    /// Direct reference to current estado (synchronized with active CambioEstado in database)
    /// This provides efficient access without querying the CambioEstado list
    /// </summary>
    public Estado? Estado { get; set; }

    public DatosSismografo ObtenerDatos()
    {
        return new DatosSismografo
        {
            Identificador = IdentificadorSismografo,
            Nombre = Nombre,
            Estado = Estado?.Nombre ?? "Sin estado",
            SismografoCompleto = this
        };
    }
}
namespace RedSismica.Models;

public class Estado(string nombre)
{
    private string Nombre { get; set; } = nombre;

    public bool EsCompletamenteRealizada()
    {
        return Nombre == "Completamente Realizada";
    }
}
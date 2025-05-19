namespace RedSismica.Models;

public class Estado
{
    public string Nombre { get; private set; }

    public Estado(string nombre)
    {
        Nombre = nombre;
    }

    public bool EsCompletamenteRealizada()
    {
        return Nombre == "Completamente Realizada";
    }
}
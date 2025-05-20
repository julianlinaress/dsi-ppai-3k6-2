namespace RedSismica.Models;

public class Estado
{
    //atributos
    public string Nombre { get; private set; }

    //constructor
    public Estado(string nombre)
    {
        Nombre = nombre;
    }

    //métodos
    public bool EsCompletamenteRealizada()
    {
        return Nombre == "Completamente Realizada";
    }
}
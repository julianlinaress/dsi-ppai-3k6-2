public class EstacionSismologica
{
    // atributos 
    public string Nombre { get; private set; }
    public Sismografo Sismografo { get; private set; }
    
    // métodos
    public EstacionSismologica(string nombre, Sismografo sismografo)
    {
        Nombre = nombre;
        Sismografo = sismografo;
    }
}

public class Sismografo
{
    // atributos
    public string Nombre { get; private set; }

    // métodos
    public Sismografo(string nombre)
    {
        Nombre = nombre;
    }
}
public class EstacionSismologica
{
    public string Nombre { get; private set; }
    public Sismografo Sismografo { get; private set; }

    public EstacionSismologica(string nombre, Sismografo sismografo)
    {
        Nombre = nombre;
        Sismografo = sismografo;
    }
}

public class Sismografo
{
    public string Nombre { get; private set; }

    public Sismografo(string nombre)
    {
        Nombre = nombre;
    }
}
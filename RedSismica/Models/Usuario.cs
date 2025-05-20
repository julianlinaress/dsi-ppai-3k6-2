public class Usuario
{
    // Atributos
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public bool EsRI { get; private set; }

    // Constructor
    public Usuario(int id, string nombre, bool esRI)
    {
        Id = id;
        Nombre = nombre;
        EsRI = esRI;
    }

    // Métodos
    public Usuario obtenerEmpleado()
    {
        return this;
    }
}
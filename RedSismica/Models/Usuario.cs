public class Usuario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public bool EsRI { get; private set; }

    public Usuario(int id, string nombre, bool esRI)
    {
        Id = id;
        Nombre = nombre;
        EsRI = esRI;
    }

    public Usuario obtenerEmpleado()
    {
        return this;
    }
}
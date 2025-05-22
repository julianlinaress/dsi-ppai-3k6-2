using System.Collections.Generic;

namespace RedSismica.Models;

public class Usuario(int id, string nombre, string password, bool esRI)
{
    internal string Password = password;
    public int Id { get; private set; } = id;
    public string Nombre { get; private set; } = nombre;
    public bool EsRI { get; private set; } = esRI;

    public Usuario ObtenerEmpleado()
    {
        return this;
    }
}
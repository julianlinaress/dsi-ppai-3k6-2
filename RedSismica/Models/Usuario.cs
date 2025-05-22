using System.Collections.Generic;

namespace RedSismica.Models;

public class Usuario(int id, string nombre, string password, bool esRi)
{
    internal string Password = password;
    public int Id { get; private set; } = id;
    public string Nombre { get; private set; } = nombre;
    
    public Empleado? Empleado { get; private set; }
    public bool EsRi { get; private set; } = esRi;

    public Empleado? ObtenerEmpleado()
    {
        return Empleado;
    }
}
namespace RedSismica.Models;

public class Rol(string nombre)
{
    public string? DescripcionRol { get; set; }
    public string? Nombre { get; set; } = nombre;
}
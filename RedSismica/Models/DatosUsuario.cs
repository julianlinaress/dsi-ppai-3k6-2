namespace RedSismica.Models;

public class DatosUsuario
{
    public int Id { get; init; }
    public required string Nombre { get; init; }
    public required string NombreUsuario { get; init; }
    public required string NombreRol { get; init; }
}

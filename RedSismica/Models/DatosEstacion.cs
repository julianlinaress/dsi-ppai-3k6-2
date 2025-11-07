namespace RedSismica.Models;

public class DatosEstacion
{
    public int Id { get; init; }
    public required string Nombre { get; init; }
    public required string NombreSismografo { get; init; }
    public required string EstadoSismografo { get; init; }
}

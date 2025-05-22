namespace RedSismica.Models;

public class Empleado(string nombre, string apellido, string telefono, string mail, Rol rol)
{
    public string? Apellido { get; set; } = apellido;
    public string? Mail { get; set; } = mail;
    public string? Nombre { get; set; } = nombre;
    public string? Telefono { get; set; } = telefono;
    public Rol? Rol { get; set; } = rol;

    public bool EsResponsableDeInspeccion()
    {
        return Rol?.Nombre == "Responsable de Inspección";
    }
}
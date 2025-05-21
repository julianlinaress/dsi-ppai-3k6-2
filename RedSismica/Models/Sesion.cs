namespace RedSismica.Models;

public class Sesion
{
    private Usuario _usuarioActual;

    public Sesion()
    {
        // Mock data
        _usuarioActual = new Usuario(1, "Julian Linares", true);
    }

    public Usuario obtenerRILogueado()
    {
        return _usuarioActual;
    }
}
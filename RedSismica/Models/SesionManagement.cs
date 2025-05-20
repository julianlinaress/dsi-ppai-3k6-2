public class Sesion
{
    private Usuario usuarioActual;

    public Sesion()
    {
        // Mock data
        usuarioActual = new Usuario(1, "Julian Linares", true);
    }

    public Usuario obtenerRILogueado()
    {
        return usuarioActual;
    }
}
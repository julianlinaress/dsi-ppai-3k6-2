public class Sesion
{
    // Atributos
    private Usuario usuarioActual;

    // Constructor, con datos de prueba
    public Sesion()
    {
        // Mock data
        usuarioActual = new Usuario(1, "Julian Linares", true);
    }

    // Verifica si el usuario logueado es un responsable de inspección (RI)

    public Usuario obtenerRILogueado()
    {
        return usuarioActual;
    }
}
using RedSismica.Models;

namespace RedSismica
{
    public static class SesionManager
    {

        public static Sesion? SesionActual { get; private set; }
        public static void InicializarSesion(Sesion sesion)
        {
            SesionActual = sesion;
        }
    }
}
 namespace RedSismica.Models;

public class EstacionSismologica(string nombre, Sismografo sismografo)
{
    public string Nombre { get; private set; } = nombre;
    public Sismografo Sismografo { get; private set; } = sismografo;
    
    public CambioEstado? ObtenerCambioEstadoActual()
    {
        if (Program.BaseDeDatosMock == null) return null;
        var cambioEstado = Program.BaseDeDatosMock.CambiosDeEstado.Find(ce => ce.EsEstadoActual());
        return cambioEstado;

    }
}
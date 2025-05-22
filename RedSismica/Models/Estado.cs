namespace RedSismica.Models;

public class Estado(string nombre, string ambito)
{
    private string Ambito { get; set; } = ambito;
    private string Nombre { get; set; } = nombre;

    public bool EsCerrada()
    {
        return Nombre == "Cerrada";
    }
    
    public bool EsAmbitoOrdenInspeccion()
    {
        return Ambito == "Orden de Inspección";
    }   
    public bool EsCompletamenteRealizada()
    {
        return Nombre == "Completamente Realizada";
    }
}
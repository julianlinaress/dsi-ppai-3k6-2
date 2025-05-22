namespace RedSismica.Models;

public class Estado(string nombre, string ambito)
{
    public string Ambito { get; set; } = ambito;
    public string Nombre { get; set; } = nombre;

    public bool EsCerrada()
    {
        return Nombre == "Cerrada";
    }
    
    public bool EsAmbitoOrdenInspeccion()
    {
        return Ambito == "Orden de Inspección";
    }       
    public bool EsAmbitoSismografo()
    {
        return Ambito == "Sismografo";
    }   
    public bool EsCompletamenteRealizada()
    {
        return Nombre == "Completamente Realizada";
    }
    public bool EsFueraDeServicio()
    {
        return Nombre == "Fuera de Servicio";
    }
}
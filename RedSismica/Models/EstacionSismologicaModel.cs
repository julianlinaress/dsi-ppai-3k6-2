 using System;
 using System.Collections.Generic;

 namespace RedSismica.Models;

public class EstacionSismologica(string nombre, Sismografo sismografo)
{
    public string Nombre { get; private set; } = nombre;
    public Sismografo Sismografo { get; } = sismografo;
    
    public void PonerSismografoEnFueraDeServicio(Estado estadoFueraDeServicioSismografo, Dictionary<MotivoTipo, string> motivosYComentarios)
    {
        if (Sismografo.Estado == null || Sismografo.CambioEstado == null) return;
        
        // Convertir motivos a lista
        List<MotivoFueraServicio> motivosFueraServicio = [];
        foreach (var motivo in motivosYComentarios)
        {
            var motivoFueraServicio = new MotivoFueraServicio(motivo.Key, motivo.Value);
            motivosFueraServicio.Add(motivoFueraServicio);
        }
        
        // Delegar al estado del sismógrafo (patrón State)
        Sismografo.Estado.PonerFueraDeServicio(
            responsable: null!, // TODO: Obtener empleado del contexto
            motivos: motivosFueraServicio,
            fechaYHora: DateTime.Now,
            self: Sismografo,
            cambiosDeEstado: Sismografo.CambioEstado
        );
    }

    public DatosEstacion ObtenerDatos()
    {
        return new DatosEstacion
        {
            Id = Sismografo.IdentificadorSismografo,
            Nombre = Nombre,
            NombreSismografo = Sismografo.Nombre,
            EstadoSismografo = Sismografo.Estado?.Nombre ?? "Sin estado"
        };
    }
}
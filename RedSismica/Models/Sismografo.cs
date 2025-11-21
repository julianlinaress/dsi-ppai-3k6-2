using System;
using System.Collections.Generic;
using System.Linq;

namespace RedSismica.Models;

public class Sismografo(string nombre)
{
    private static int _contador = 18122021;
    public string Nombre { get; private set; } = nombre;

    public int IdentificadorSismografo { get; private set; } = _contador++;
    public List<CambioEstado> CambioEstado { get; set; } = [];
    
    public EstadoSismografo Estado { get; set; } = new Inhabilitado();

    public void PonerSismografoEnFueraDeServicio(            
            Empleado responsable,
            List<MotivoFueraServicio> motivos,
            DateTime fechaYHora
        )
    {
        Estado.FueraDeServicio(
            responsable: responsable,
            motivos: motivos,
            fechaYHora: fechaYHora,
            self: this,
            cambiosDeEstado: CambioEstado
        );
    }

    public DatosSismografo ObtenerDatos()
    {
        return new DatosSismografo
        {
            Identificador = IdentificadorSismografo,
            Nombre = Nombre,
            Estado = Estado.Nombre,
            SismografoCompleto = this
        };
    }

    public void SetEstado(EstadoSismografo nuevoEstado)
    {
        Estado = nuevoEstado;
    }
}

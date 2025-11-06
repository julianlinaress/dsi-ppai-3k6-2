using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedSismica.Models;

public class Sesion()
{
    private Usuario? _usuarioActual;

    private readonly DateTime _fechaHoraInicio = DateTime.Now;
    // private DateTime FechaHoraFin;

    public bool AutenticarUsuario(string? username, string? password)
    {
        var usuario = Program.BaseDeDatosMock?.Usuarios.FirstOrDefault(u => u.Nombre == username && u.Password == password);
        Debug.WriteLine(usuario);
        if (usuario == null) return false;
        _usuarioActual = usuario;
        return true;
    }

    public void CerrarSesion()
    {
        _usuarioActual = null;
    }

    public Usuario? ObtenerRILogueado()
    {
        return _usuarioActual;
    }
}
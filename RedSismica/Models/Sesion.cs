using System;
using System.Diagnostics;
using RedSismica.Database;

namespace RedSismica.Models;

public class Sesion()
{
    private Usuario? _usuarioActual;
    private readonly DateTime _fechaHoraInicio = DateTime.Now;

    public bool AutenticarUsuario(string? username, string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        try
        {
            var context = RedSismicaDataContext.Create();
            var usuario = context.Usuarios.Authenticate(username, password);
            
            Debug.WriteLine(usuario != null 
                ? $"Usuario autenticado: {usuario.Nombre}" 
                : "Autenticación fallida");
            
            if (usuario == null) return false;
            _usuarioActual = usuario;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error en autenticación: {ex.Message}");
            return false;
        }
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
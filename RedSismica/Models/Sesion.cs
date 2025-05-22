using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedSismica.Models;

public class Sesion()
{
    private Usuario? _usuarioActual;

    private List<Usuario> _usuariosMock = [
        new(1, "jlinares", "123", true),
        new(2, "mperez", "123", true),
        new(3, "cgomez", "123", true)
    ];
    
    public bool AutenticarUsuario(string? username, string? password)
    {
        var usuario = _usuariosMock.FirstOrDefault(u => u.Nombre == username && u.Password == password);
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

    public List<Usuario> GetMockUsers()
    {
        return _usuariosMock;
    }
}
using System.Collections.Generic;

namespace RedSismica.Models;

public class BaseDeDatosMock(
    List<OrdenDeInspeccion> ordenesDeInspeccion,
    List<Sismografo> sismografos,
    List<EstacionSismologica> estacionesSismologicas,
    List<MotivoFueraServicio> motivosFueraServicio,
    List<Estado> estados,
    List<Usuario> usuarios,
    List<CambioEstado> cambiosDeEstado,
    List<Empleado> empleados,
    List<Rol> roles,
    List<MotivoTipo> motivoTipos
    )
{
    public List<OrdenDeInspeccion> OrdenesDeInspeccion { get; set; } = ordenesDeInspeccion;
    public List<Sismografo> Sismografos { get; set; } = sismografos;
    public List<EstacionSismologica> EstacionesSismologicas { get; set; } = estacionesSismologicas;
    public List<MotivoFueraServicio> MotivosFueraServicio { get; set; } = motivosFueraServicio;
    public List<Estado> Estados { get; set; } = estados;
    public List<Usuario> Usuarios { get; set; } = usuarios;
    public List<Rol> Roles { get; set; } = roles;
    public List<CambioEstado> CambiosDeEstado { get; set; } = cambiosDeEstado;
    public List<Empleado> Empleados { get; set; } = empleados;
    public List<MotivoTipo> MotivosTipos { get; set; } = motivoTipos;
}
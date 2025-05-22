using Avalonia;
using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using RedSismica.Models;

namespace RedSismica;


public class Program
{
    public static BaseDeDatosMock? BaseDeDatosMock { get; set; }
    private static void InicializarDatosDePrueba()
    {
        var estadoCompletado = new Estado("Completamente Realizada", "Orden de Inspeccion");
        var estadoOtro = new Estado("En Proceso", "Orden de Inspeccion");
        List<Estado> estados =
        [
            estadoCompletado,
            estadoOtro,
            new("Algun Estado", "Orden de Inspección"),
            
            new("Estado Inicial", "Orden de Inspección"),
            
            new("Fuera de Servicio", "Sismografo"),
            
            new("Cerrada", "Orden de Inspección"),
        ];

        var ri = new Rol("Responsable de Inspección");
        var rol2 = new Rol("Otro");
        List<Rol> roles = [
            ri,
            rol2
        ];
        
        List<Empleado> empleados =
        [
            new("Julian", "Linares", "12345678", "julian@linares.com.ar", ri),
            // new("Mauro", "Bastasini", "12345678", "maurobastasiniprof@gmail.com", ri),
            // new("Inés", "Haefeli", "12345678", "ineshaefeli@gmail.com", ri),
            // new("Huenu", "Capdevila", "12345678", "huecap7@gmail.com", ri),
            // new("Julian", "Linares", "12345678", "julianlinares2003@gmail.com", rol2),
        ];

            
        var sismografo1 = new Sismografo("Sismógrafo A123");
        var sismografo2 = new Sismografo("Sismógrafo B456");

        var estacion1 = new EstacionSismologica("Estación Norte", sismografo1);
        var estacion2 = new EstacionSismologica("Estación Sur", sismografo2);
        
        List<Sismografo> sismografos = [sismografo1, sismografo2];
        List<EstacionSismologica> estacionesSismologicas = [estacion1, estacion2];
        List<MotivoFueraServicio> motivosFueraServicio = [];
        List<CambioEstado> cambiosDeEstado = [];
        List<Usuario> usuarios =
        [
            new(1, "jlinares", "123", true),
            new(2, "mperez", "123", true),
            new(3, "cgomez", "123", true)
        ];
        
        
        List<OrdenDeInspeccion> ordenesDeInspeccion =
        [
            new(1, DateTime.Now.AddDays(-5), usuarios[0], estadoCompletado, estacion1),
            new(2, DateTime.Now.AddDays(-7), usuarios[0], estadoCompletado, estacion1),
            new(3, DateTime.Now.AddDays(-12),usuarios[1], estadoCompletado, estacion1),
            new(4, DateTime.Now.AddDays(-23),usuarios[0], estadoCompletado, estacion1),
            new(5, DateTime.Now.AddDays(-54),usuarios[1], estadoCompletado, estacion1),
            new(6, DateTime.Now.AddDays(-2), usuarios[1], estadoCompletado, estacion1),
            new(7, DateTime.Now.AddDays(-3), usuarios[0], estadoCompletado, estacion1),
            new(8, DateTime.Now.AddDays(-3), usuarios[1], estadoCompletado, estacion2),
            new(9, DateTime.Now.AddDays(-1), usuarios[1], estadoOtro, estacion1)
        ];
        
        List<MotivoTipo> motivoTipos =
        [
            new("Reparacion"),
            new("Renovacion"),
            new("Cambio de Sismografo"),
            new("Otro")
        ];

        BaseDeDatosMock = new BaseDeDatosMock(
            ordenesDeInspeccion,
            sismografos,
            estacionesSismologicas,
            motivosFueraServicio,
            estados,
            usuarios,
            cambiosDeEstado,
            empleados,
            roles,
            motivoTipos
        );
    }
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    [STAThread]
    public static void Main(string[] args)
    {
        InicializarDatosDePrueba();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace();
}
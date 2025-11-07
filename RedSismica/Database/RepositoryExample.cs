using System;
using System.Linq;
using RedSismica.Database;

namespace RedSismica.Database;

/// <summary>
/// Example usage of the repository layer showing materialization/dematerialization
/// </summary>
public static class RepositoryExample
{
    public static void RunExamples()
    {
        Console.WriteLine("=== Repository Layer Examples ===\n");

        // Create data context
        var context = RedSismicaDataContext.Create();

        // Example 1: Materialize all usuarios
        Console.WriteLine("1. Loading all usuarios from database:");
        var usuarios = context.Usuarios.GetAll();
        foreach (var usuario in usuarios)
        {
            Console.WriteLine($"   - {usuario.Nombre} (RI: {usuario.EsRi})");
        }

        // Example 2: Authenticate a user
        Console.WriteLine("\n2. Authenticating user 'jlinares' with password '123':");
        var usuarioAutenticado = context.Usuarios.Authenticate("jlinares", "123");
        if (usuarioAutenticado != null)
        {
            Console.WriteLine($"   ✓ Authentication successful! User ID: {usuarioAutenticado.Id}");
        }
        else
        {
            Console.WriteLine("   ✗ Authentication failed!");
        }

        // Example 3: Get estados for Orden de Inspección
        Console.WriteLine("\n3. Loading estados for 'Orden de Inspección':");
        var estadosOrden = context.Estados.GetByAmbito("Orden de Inspección");
        foreach (var estado in estadosOrden)
        {
            Console.WriteLine($"   - {estado.Nombre}");
        }

        // Example 4: Load all estaciones with their sismografos
        Console.WriteLine("\n4. Loading all estaciones (with sismografos):");
        var estaciones = context.Estaciones.GetAll();
        foreach (var estacion in estaciones)
        {
            Console.WriteLine($"   - {estacion.Nombre}");
            Console.WriteLine($"     Sismografo: {estacion.Sismografo.Nombre}");
            Console.WriteLine($"     ID: {estacion.Sismografo.IdentificadorSismografo}");
        }

        // Example 5: Get ordenes completamente realizadas for a specific RI
        if (usuarioAutenticado != null)
        {
            Console.WriteLine($"\n5. Loading ordenes completamente realizadas for {usuarioAutenticado.Nombre}:");
            var ordenesCompletas = context.Ordenes.GetCompletamenteRealizadasByResponsable(usuarioAutenticado);
            Console.WriteLine($"   Found {ordenesCompletas.Count} ordenes:");
            foreach (var orden in ordenesCompletas.Take(3))
            {
                Console.WriteLine($"   - Orden #{orden.NumeroOrden}");
                Console.WriteLine($"     Estacion: {orden.Estacion.Nombre}");
                Console.WriteLine($"     Fecha: {orden.ObtenerDatos().FechaFinalizacion:dd/MM/yyyy}");
            }
        }

        // Example 6: Update an orden (dematerialization example)
        Console.WriteLine("\n6. Example: Closing an orden (dematerialization):");
        var ordenParaCerrar = context.Ordenes.GetByNumeroOrden(1);
        if (ordenParaCerrar != null)
        {
            var estadoCerrada = context.Estados.GetByNombreAndAmbito("Cerrada", "Orden de Inspección");
            if (estadoCerrada != null)
            {
                // In real usage, you would call: ordenParaCerrar.Cerrar(estadoCerrada, DateTime.Now);
                // Then persist to database:
                // context.Ordenes.Update(ordenParaCerrar, estadoCerrada, DateTime.Now);
                Console.WriteLine($"   ✓ Orden #{ordenParaCerrar.NumeroOrden} would be closed");
                Console.WriteLine($"     (Not actually updating to preserve test data)");
            }
        }

        Console.WriteLine("\n=== Examples Complete ===");
    }
}

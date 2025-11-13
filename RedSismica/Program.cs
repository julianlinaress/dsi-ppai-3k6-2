using Avalonia;
using System;
using Avalonia.ReactiveUI;
using RedSismica.Database;
using System.Diagnostics;

namespace RedSismica;

public class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    [STAThread]
    public static void Main(string[] args)
    {
        // Initialize SQLite database (creates if doesn't exist)
        Console.WriteLine("=== Red Sísmica - Initializing ===");
        Console.WriteLine("Initializing database...");
        DatabaseInitializer.Initialize();
        DatabaseInitializer.TestConnection();
        
        Console.WriteLine("Starting application...\n");
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
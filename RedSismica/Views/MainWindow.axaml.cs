using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using RedSismica.Models;
using RedSismica.ViewModels;

namespace RedSismica.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var usuarioActual = SesionManager.SesionActual?.ObtenerRILogueado();
        if (usuarioActual == null) return;
        var nombreCompleto = usuarioActual.Nombre;
        var textBlock = new TextBlock
        {
            Text = nombreCompleto,
            FontSize = 20,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        };
        this.FindControl<Grid>("MainPanel")?.Children.Insert(0, textBlock);
    }
    
        
    private void CerrarSesion(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        SesionManager.SesionActual?.CerrarSesion();
        SesionManager.InicializarSesion(new Sesion());
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close();
        loginWindow.Closed += (_, _) =>
        {
            if (loginWindow.IsLoginSuccessful)
            {
                // Inicializar la sesión aquí
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow.Show();
            }
            else
            {
                desktop.Shutdown();
            }
        };
    }

    
    private void SeleccionarOpcionRegistarCierre(object? sender, RoutedEventArgs e)
    {
        var ventanaCierre = new VentanaCierreOrden();
        ventanaCierre.Show();
    }
}
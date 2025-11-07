using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using RedSismica.Models;
using RedSismica.ViewModels;
using RedSismica.Database;

namespace RedSismica.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CargarDatosUsuario();
        _ = CargarDatosTablasAsync();
    }

    private void CargarDatosUsuario()
    {
        var usuarioActual = SesionManager.SesionActual?.ObtenerRILogueado();
        if (usuarioActual == null) return;
        
        var userInfoPanel = this.FindControl<StackPanel>("UserInfoPanel");
        if (userInfoPanel == null) return;

        var userIcon = new TextBlock
        {
            Text = "👤",
            FontSize = 16,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Foreground = Brushes.White
        };

        var userName = new TextBlock
        {
            Text = usuarioActual.Nombre,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Foreground = Brushes.White,
            Margin = new Thickness(5, 0, 0, 0)
        };

        userInfoPanel.Children.Add(userIcon);
        userInfoPanel.Children.Add(userName);
    }

    private async Task CargarDatosTablasAsync()
    {
        // Load data in background thread
        var (estacionesData, ordenesData, usuariosData) = await Task.Run(() =>
        {
            var context = RedSismicaDataContext.Create();
            
            var estaciones = context.Estaciones.GetAll()
                .Select(e => e.ObtenerDatos())
                .ToList();
            
            var ordenes = context.Ordenes.GetAll()
                .Select(o => o.ObtenerDatos())
                .ToList();
            
            var usuarios = context.Usuarios.GetAll()
                .Select(u => u.ObtenerDatos())
                .ToList();
            
            return (estaciones, ordenes, usuarios);
        });

        // Update UI on main thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var estacionesGrid = this.FindControl<DataGrid>("EstacionesDataGrid");
            if (estacionesGrid != null)
            {
                estacionesGrid.ItemsSource = estacionesData;
            }

            var ordenesGrid = this.FindControl<DataGrid>("OrdenesDataGrid");
            if (ordenesGrid != null)
            {
                ordenesGrid.ItemsSource = ordenesData;
            }

            var usuariosGrid = this.FindControl<DataGrid>("UsuariosDataGrid");
            if (usuariosGrid != null)
            {
                usuariosGrid.ItemsSource = usuariosData;
            }
        });
    }

    private async void ActualizarDatos(object? sender, RoutedEventArgs e)
    {
        var boton = sender as Button;
        
        // Disable button while refreshing
        if (boton != null)
        {
            boton.IsEnabled = false;
            boton.Content = "⏳ Actualizando...";
        }

        try
        {
            await CargarDatosTablasAsync();
        }
        finally
        {
            // Re-enable button
            if (boton != null)
            {
                boton.IsEnabled = true;
                boton.Content = "🔄 Actualizar Datos";
            }
        }
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
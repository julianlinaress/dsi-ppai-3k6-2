using System.Diagnostics;
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

    public void ActualizarEstadoEnvio(string texto)
    {
        var status = this.FindControl<TextBlock>("StatusText");
        if (status == null) return;
        Dispatcher.UIThread.Post(() =>
        {
            status.Text = texto;
            status.IsVisible = !string.IsNullOrWhiteSpace(texto);
        });
    }

    private void CargarDatosUsuario()
    {
        var usuarioActual = SesionManager.SesionActual?.ObtenerRILogueado();
        if (usuarioActual == null) return;
        
        var userInfoPanel = this.FindControl<StackPanel>("UserInfoPanel");
        if (userInfoPanel == null) return;

        var userName = new TextBlock
        {
            Text = usuarioActual.Nombre,
            FontSize = 12,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        userInfoPanel.Children.Add(userName);
    }

    private async Task CargarDatosTablasAsync()
    {
        // Load data in background thread
        var (sismografosData, estacionesData, ordenesData, usuariosData) = await Task.Run(() =>
        {
            var context = RedSismicaDataContext.Create();
            
            var sismografos = context.Sismografos.GetAll()
                .Select(s => s.ObtenerDatos())
                .ToList();
            Debug.WriteLine($"[MainWindow] Loaded {sismografos.Count} sismógrafos");
            
            var estaciones = context.Estaciones.GetAll()
                .Select(e => e.ObtenerDatos())
                .ToList();
            Debug.WriteLine($"[MainWindow] Loaded {estaciones.Count} estaciones");
            
            var ordenes = context.Ordenes.GetAll()
                .Select(o => o.ObtenerDatos())
                .ToList();
            Debug.WriteLine($"[MainWindow] Loaded {ordenes.Count} órdenes de inspección");
            
            var usuarios = context.Usuarios.GetAll()
                .Select(u => u.ObtenerDatos())
                .ToList();
            Debug.WriteLine($"[MainWindow] Loaded {usuarios.Count} usuarios");
            
            return (sismografos, estaciones, ordenes, usuarios);
        });

        // Update UI on main thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var sismografosGrid = this.FindControl<DataGrid>("SismografosDataGrid");
            if (sismografosGrid != null)
            {
                sismografosGrid.ItemsSource = sismografosData;
            }

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
            boton.Content = "Actualizando...";
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
                boton.Content = "Actualizar Datos";
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

    private void VerHistorialSismografo(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.Tag is not DatosSismografo datosSismografo) return;
        if (datosSismografo.SismografoCompleto == null) return;

        var ventanaHistorial = new VentanaHistorialEstados(datosSismografo.SismografoCompleto);
        ventanaHistorial.Show();
    }
}
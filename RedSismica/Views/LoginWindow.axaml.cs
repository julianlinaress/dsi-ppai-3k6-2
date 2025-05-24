using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Notifications;

namespace RedSismica.Views;

public partial class LoginWindow : Window
{
    public bool IsLoginSuccessful { get; private set; }
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        var username = UsernameTextBox.Text;
        var password = PasswordBox.Text;
        var sesion = SesionManager.SesionActual;
        if (sesion == null) return;
        // Verificar credenciales contra usuarios de prueba
        if (sesion.AutenticarUsuario(username, password))
        {
            IsLoginSuccessful = true;
            Close();
        }
        else
        {
            // Autenticación fallida
            IsLoginSuccessful = false;
            var messageBox = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.BottomRight
            };
            messageBox.Show(new Notification("Error", "Credenciales incorrectas", NotificationType.Error));
        }
    }
}


using System;
using System.Windows;
using System.Windows.Controls;

namespace Vistas
{
    public partial class UserControlLogin : UserControl
    {
        // Evento propio que avisa "afuera" cuando el login fue exitoso
        public event EventHandler<LoginEventArgs> IngresoExitoso;

        public UserControlLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            string rol = null;

            if (usuario != "admin" && usuario != "vendedor")
            {
                MessageBox.Show("El usuario es incorrecto.", "Ingreso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsuario.Focus();
                return;
            }

            if (usuario == "admin" && password == "admin123") rol = "Admin";
            if (usuario == "vendedor" && password == "vend123") rol = "Vendedor";

            if (rol == null)
            {
                MessageBox.Show("La contraseña es incorrecta.", "Ingreso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Avisamos hacia afuera (a la ventana Login) que el ingreso fue exitoso
            if (IngresoExitoso != null)
            {
                IngresoExitoso(this, new LoginEventArgs(usuario, rol));
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea salir?", "Confirmar salida",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }
    }

    // Clase auxiliar para viajar usuario y rol junto con el evento
    public class LoginEventArgs : EventArgs
    {
        public string Usuario { get; private set; }
        public string Rol { get; private set; }

        public LoginEventArgs(string usuario, string rol)
        {
            Usuario = usuario;
            Rol = rol;
        }
    }
}

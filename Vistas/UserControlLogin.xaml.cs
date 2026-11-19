
using System;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

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

            if (usuario == "")
            {
                MessageBox.Show("Ingrese el nombre de usuario.", "Ingreso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsuario.Focus();
                return;
            }
            if (password == "")
            {
                MessageBox.Show("Ingrese la contraseña.", "Ingreso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            Usuario autenticado;
            ResultadoLogin resultado;
            try
            {
                TrabajarUsuarios.AsegurarTabla();
                autenticado = TrabajarUsuarios.Autenticar(usuario, password, out resultado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar el usuario en la base de datos.\n" + ex.Message,
                    "Ingreso", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (resultado == ResultadoLogin.UsuarioInexistente)
            {
                MessageBox.Show("El usuario es incorrecto.", "Ingreso");
                txtUsuario.Focus();
                return;
            }
            if (resultado == ResultadoLogin.PasswordIncorrecta)
            {
                MessageBox.Show("La contraseña es incorrecta.", "Ingreso");
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }
            if (resultado == ResultadoLogin.UsuarioInactivo)
            {
                MessageBox.Show("El usuario está desactivado. Consulte con un administrador.", "Ingreso");
                return;
            }

            // Avisamos hacia afuera (a la ventana Login) que el ingreso fue exitoso
            if (IngresoExitoso != null)
            {
                IngresoExitoso(this, new LoginEventArgs(autenticado.NombreUsuario, autenticado.Rol));
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

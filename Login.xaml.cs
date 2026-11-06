using System.Windows;

namespace Vistas
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void controlLogin_IngresoExitoso(object sender, LoginEventArgs e)
        {
            MainWindow menu = new MainWindow(e.Usuario, e.Rol);
            Application.Current.MainWindow = menu;
            menu.Show();
            Close();
        }
    }
}

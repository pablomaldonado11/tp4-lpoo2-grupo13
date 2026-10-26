using System.Windows;

namespace Vistas
{
    public partial class ListaDeEstados : Window
    {
        public ListaDeEstados()
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

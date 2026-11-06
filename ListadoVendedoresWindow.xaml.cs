using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class ListadoVendedoresWindow : Window
    {
        private DataView vista;

        public ListadoVendedoresWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vista = new TrabajarVendedores().TraerVendedores().DefaultView;
                vista.Sort = "Apellido ASC";
                dgVendedores.ItemsSource = vista;
                lblCantidad.Text = "Vendedores: " + vista.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los vendedores.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vista == null) return;

            string t = Utilidades.EscapeLike(txtFiltro.Text.Trim());
            vista.RowFilter = t.Length == 0 ? "" :
                "Legajo LIKE '%" + t + "%' OR Apellido LIKE '%" + t + "%' OR Nombre LIKE '%" + t + "%'";
            lblCantidad.Text = "Vendedores: " + vista.Count;
        }
    }
}

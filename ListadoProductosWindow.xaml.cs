using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class ListadoProductosWindow : Window
    {
        private DataView vista;

        public ListadoProductosWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vista = new TrabajarProductos().TraerProductos().DefaultView;
                dgProductos.ItemsSource = vista;
                lblCantidad.Text = "Productos: " + vista.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los productos.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vista == null) return;

            string t = Utilidades.EscapeLike(txtFiltro.Text.Trim());
            vista.RowFilter = t.Length == 0 ? "" :
                "CodProducto LIKE '%" + t + "%' OR Categoria LIKE '%" + t + "%' OR " +
                "Color LIKE '%" + t + "%' OR Descripcion LIKE '%" + t + "%'";
            lblCantidad.Text = "Productos: " + vista.Count;
        }
    }
}

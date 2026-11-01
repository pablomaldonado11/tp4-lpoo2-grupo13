using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClasesBase;

namespace Vistas
{
    public partial class ListadoProveedoresWindow : Window
    {
        private ICollectionView vista;

        public ListadoProveedoresWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ObservableCollection<Proveedor> proveedores =
                    new ObservableCollection<Proveedor>(TrabajarProveedores.TraerProveedores());

                vista = CollectionViewSource.GetDefaultView(proveedores);
                vista.SortDescriptions.Add(new SortDescription("RazonSocial", ListSortDirection.Ascending));
                vista.Filter = FiltrarPorRazonSocial;
                dgProveedores.ItemsSource = vista;
                ActualizarCantidad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los proveedores.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool FiltrarPorRazonSocial(object item)
        {
            Proveedor p = item as Proveedor;
            if (p == null) return false;

            string texto = txtFiltro.Text.Trim();
            if (texto.Length == 0) return true;

            return p.RazonSocial != null &&
                   p.RazonSocial.IndexOf(texto, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void txtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vista == null) return;
            vista.Refresh();
            ActualizarCantidad();
        }

        private void ActualizarCantidad()
        {
            int n = 0;
            foreach (object o in vista) n++;
            lblCantidad.Text = "Proveedores: " + n;
        }
    }
}

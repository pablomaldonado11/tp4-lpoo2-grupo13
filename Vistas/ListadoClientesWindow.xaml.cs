using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClasesBase;

namespace Vistas
{
    public partial class ListadoClientesWindow : Window
    {
        private ICollectionView vista;

        public ListadoClientesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Cliente> clientes = TrabajarClientes.TraerClientes();
            vista = CollectionViewSource.GetDefaultView(clientes);

            // Orden por apellido ascendente
            vista.SortDescriptions.Add(
                new SortDescription("Apellido", ListSortDirection.Ascending));

            // Filtro por apellido
            vista.Filter = FiltrarPorApellido;

            dgClientes.ItemsSource = vista;
        }

        private bool FiltrarPorApellido(object item)
        {
            Cliente c = item as Cliente;
            if (c == null) return false;

            string texto = txtFiltroApellido.Text;
            if (string.IsNullOrEmpty(texto)) return true;

            return c.Apellido != null &&
                   c.Apellido.IndexOf(texto, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void txtFiltroApellido_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vista != null)
                vista.Refresh();
        }

        private void btnVistaPrevia_Click(object sender, RoutedEventArgs e)
        {
            if (vista == null) return;

            // Se copian solamente los clientes visibles en la grilla.
            // Si no hay filtro, se incluyen todos.
            List<Cliente> clientesVisibles = vista.Cast<Cliente>().ToList();

            VistaPreviaImpresionWindow ventana =
                new VistaPreviaImpresionWindow(clientesVisibles);
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

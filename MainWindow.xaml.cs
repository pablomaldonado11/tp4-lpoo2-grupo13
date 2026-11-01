using System;
using System.Windows;

namespace Vistas
{
    public partial class MainWindow : Window
    {
        private readonly string rolActual;

        public MainWindow(string usuario, string rol)
        {
            InitializeComponent();

            rolActual = rol;

            lblTitulo.Text = "Bienvenido, " + usuario;
            lblRol.Text = "Rol actual: " + rol;

            AplicarPermisos(rol);
        }

        private void AplicarPermisos(string rol)
        {
            // Acceso denegado por defecto: cualquier rol desconocido queda sin permisos.
            btn_Proveedores.IsEnabled = false;
            btn_Clientes.IsEnabled = false;
            btn_Productos.IsEnabled = false;
            btn_Vendedores.IsEnabled = false;
            btn_Ventas.IsEnabled = false;
            btn_ListadoVentas.IsEnabled = false;
            btn_ListadoProductos.IsEnabled = false;
            btn_ListadoVendedores.IsEnabled = false;
            btn_ListadoProveedores.IsEnabled = false;

            if (rol == "Admin")
            {
                btn_Proveedores.IsEnabled = true;
                btn_Clientes.IsEnabled = true;
                btn_Productos.IsEnabled = true;
                btn_Vendedores.IsEnabled = true;
                btn_Ventas.IsEnabled = true;
                btn_ListadoVentas.IsEnabled = true;
                btn_ListadoProductos.IsEnabled = true;
                btn_ListadoVendedores.IsEnabled = true;
                btn_ListadoProveedores.IsEnabled = true;
            }
            else if (rol == "Vendedor")
            {
                btn_Proveedores.IsEnabled = true;
                btn_Clientes.IsEnabled = true;
                btn_Productos.IsEnabled = true;
                btn_Ventas.IsEnabled = true;
                btn_ListadoVentas.IsEnabled = true;
                btn_ListadoProductos.IsEnabled = true;
                btn_ListadoProveedores.IsEnabled = true;
            }
        }

        private void btnProveedores_Click(object sender, RoutedEventArgs e)
        {
            ProveedorWindow ventana = new ProveedorWindow();
            ventana.ShowDialog();
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            ClienteWindow ventana = new ClienteWindow();
            ventana.ShowDialog();
        }

        private void btnProductos_Click(object sender, RoutedEventArgs e)
        {
            ProductoWindow ventana = new ProductoWindow();
            ventana.ShowDialog();
        }

        private void btnVendedores_Click(object sender, RoutedEventArgs e)
        {
            if (rolActual != "Admin")
            {
                return;
            }

            VendedorWindow ventana = new VendedorWindow();
            ventana.ShowDialog();
        }

        private void btnEstados_Click(object sender, RoutedEventArgs e)
        {
            ListaDeEstados ventana = new ListaDeEstados();
            ventana.ShowDialog();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea salir?",
                "Confirmar salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
                Close();
        }

        // ---------- Agregado: ventas, listados y cerrar sesión ----------

        private void btnVentas_Click(object sender, RoutedEventArgs e)
        {
            VentaWindow ventana = new VentaWindow(rolActual);
            ventana.ShowDialog();
        }

        private void btnListadoVentas_Click(object sender, RoutedEventArgs e)
        {
            ListadoVentasWindow ventana = new ListadoVentasWindow(rolActual);
            ventana.ShowDialog();
        }

        private void btnListadoProductos_Click(object sender, RoutedEventArgs e)
        {
            ListadoProductosWindow ventana = new ListadoProductosWindow();
            ventana.ShowDialog();
        }

        private void btnListadoVendedores_Click(object sender, RoutedEventArgs e)
        {
            if (rolActual != "Admin") return;

            ListadoVendedoresWindow ventana = new ListadoVendedoresWindow();
            ventana.ShowDialog();
        }

        private void btnListadoProveedores_Click(object sender, RoutedEventArgs e)
        {
            ListadoProveedoresWindow ventana = new ListadoProveedoresWindow();
            ventana.ShowDialog();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea cerrar la sesión?", "Cerrar sesión",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes) return;

            Login login = new Login();
            Application.Current.MainWindow = login;
            login.Show();
            Close();
        }

        private void btnListadoClientes_Click(object sender, RoutedEventArgs e)
        {
            ListadoClientesWindow ventana = new ListadoClientesWindow();
            ventana.Show();
        }
    }
}

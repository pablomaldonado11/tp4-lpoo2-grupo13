using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class VentaWindow : Window
    {
        private readonly string rolActual;
        private bool cargado = false;

        public VentaWindow(string rol)
        {
            InitializeComponent();
            rolActual = rol;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbCliente.ItemsSource = TrabajarClientes.TraerClientes();

                DataTable vendedores = new TrabajarVendedores().TraerVendedores();
                vendedores.Columns.Add("Texto", typeof(string), "Legajo + ' - ' + Apellido + ', ' + Nombre");
                cbVendedor.ItemsSource = vendedores.DefaultView;

                DataTable productos = new TrabajarProductos().TraerProductos();
                productos.Columns.Add("Texto", typeof(string), "CodProducto + ' - ' + Descripcion");
                cbProducto.ItemsSource = productos.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los datos.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            cargado = true;
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            try { txtNroFactura.Text = TrabajarVentas.ProximoNroFactura().ToString(); }
            catch (Exception ex)
            {
                txtNroFactura.Text = "";
                MessageBox.Show("No se pudo obtener el número de factura.\n" + ex.Message,
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            dpFecha.SelectedDate = DateTime.Today;
            cbCliente.SelectedIndex = -1;
            cbVendedor.SelectedIndex = -1;
            cbProducto.SelectedIndex = -1;
            txtCantidad.Text = "1";
            Recalcular();
        }

        private decimal PrecioElegido()
        {
            DataRowView fila = cbProducto.SelectedItem as DataRowView;
            return fila == null ? 0 : Convert.ToDecimal(fila["Precio"]);
        }

        // Muestra el precio del producto elegido y el importe (precio x cantidad)
        private void Recalcular()
        {
            if (!cargado) return;

            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad < 0) cantidad = 0;

            decimal precio = PrecioElegido();
            txtPrecio.Text = precio.ToString("N2");
            txtImporte.Text = (precio * cantidad).ToString("N2");
        }

        private void cbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recalcular();
        }

        private void txtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            Recalcular();
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            Cliente cliente = cbCliente.SelectedItem as Cliente;
            int cantidad;
            bool cantidadOk = int.TryParse(txtCantidad.Text, out cantidad) && cantidad > 0;

            StringBuilder errores = new StringBuilder();
            if (!dpFecha.SelectedDate.HasValue) errores.AppendLine("- La fecha es obligatoria.");
            if (cliente == null) errores.AppendLine("- Seleccione un cliente.");
            if (cbVendedor.SelectedValue == null) errores.AppendLine("- Seleccione un vendedor.");
            if (cbProducto.SelectedValue == null) errores.AppendLine("- Seleccione un producto.");
            if (!cantidadOk) errores.AppendLine("- La cantidad debe ser un número entero mayor a cero.");

            if (errores.Length > 0)
            {
                MessageBox.Show("Revise los datos de la venta:\n\n" + errores.ToString(),
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Venta venta = new Venta();
            venta.FechaFactura = dpFecha.SelectedDate.Value;
            venta.DNI = cliente.DNI;
            venta.Legajo = Convert.ToString(cbVendedor.SelectedValue);
            venta.CodProducto = Convert.ToString(cbProducto.SelectedValue);
            venta.Precio = PrecioElegido();
            venta.Cantidad = cantidad;
            venta.Importe = venta.Precio * cantidad;

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea registrar la venta por $" + venta.Importe.ToString("N2") + "?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                TrabajarVentas.AgregarVenta(venta);
                MessageBox.Show("Venta registrada. Factura Nº " + venta.NroFactura + ".",
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la venta.\n" + ex.Message,
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnVerVentas_Click(object sender, RoutedEventArgs e)
        {
            ListadoVentasWindow ventana = new ListadoVentasWindow(rolActual);
            ventana.ShowDialog();
            LimpiarFormulario();   // por si se anuló una venta y cambió el próximo número
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

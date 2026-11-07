using System;
using System.Windows;
using System.Data;
using ClasesBase;

namespace Vistas
{
    public partial class VentasWindow : Window
    {
        public VentasWindow()
        {
            InitializeComponent();
            dpFecha.SelectedDate = DateTime.Now;
            CargarCombos();
            txtNroVenta.Text = ObtenerProximoNroVenta().ToString();
        }

        private void CargarCombos()
        {
            TrabajarVendedores tv = new TrabajarVendedores();
            cmbVendedor.ItemsSource = tv.TraerVendedores().DefaultView;
            cmbVendedor.DisplayMemberPath = "Apellido";
            cmbVendedor.SelectedValuePath = "Legajo";

            cmbCliente.ItemsSource = TrabajarClientes.TraerClientes();
            cmbCliente.DisplayMemberPath = "Apellido";
            cmbCliente.SelectedValuePath = "DNI";

            TrabajarProductos tp = new TrabajarProductos();
            cmbProducto.ItemsSource = tp.TraerProductos().DefaultView;
            cmbProducto.DisplayMemberPath = "Descripcion";
            cmbProducto.SelectedValuePath = "CodProducto";
        }

        private int ObtenerProximoNroVenta()
        {
            return new TrabajarVentas().ProximoNroVenta();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (dpFecha.SelectedDate == null || cmbVendedor.SelectedItem == null ||
                cmbCliente.SelectedItem == null || cmbProducto.SelectedItem == null)
            {
                MessageBox.Show("Complete todos los campos.", "Ventas",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a 0.", "Ventas",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaProducto = (DataRowView)cmbProducto.SelectedItem;
            decimal precio = Convert.ToDecimal(filaProducto["Precio"]);

            Venta venta = new Venta();
            venta.NroFactura = int.Parse(txtNroVenta.Text);
            venta.FechaFactura = dpFecha.SelectedDate.Value;
            venta.Legajo = cmbVendedor.SelectedValue.ToString();
            venta.DNI = cmbCliente.SelectedValue.ToString();
            venta.CodProducto = cmbProducto.SelectedValue.ToString();
            venta.Precio = precio;
            venta.Cantidad = cantidad;
            venta.Importe = precio * cantidad;

            try
            {
                new TrabajarVentas().AgregarVenta(venta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la venta.\n" + ex.Message, "Ventas",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Venta confirmada.", "Ventas");

            ComprobanteWindow comprobante = new ComprobanteWindow(
                venta,
                (Cliente)cmbCliente.SelectedItem,
                filaProducto,
                (DataRowView)cmbVendedor.SelectedItem);
            comprobante.ShowDialog();

            txtNroVenta.Text = ObtenerProximoNroVenta().ToString();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
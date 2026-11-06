using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using ClasesBase;
using Microsoft.Win32;

namespace Vistas
{
    public partial class ListadoVentasWindow : Window
    {
        private DataView vista;

        public ListadoVentasWindow(string rol)
        {
            InitializeComponent();

            // Solo el administrador puede anular ventas
            btnAnular.IsEnabled = (rol == "Admin");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<KeyValuePair<string, string>> vendedores = new List<KeyValuePair<string, string>>();
                vendedores.Add(new KeyValuePair<string, string>("", "(Todos)"));

                foreach (DataRow fila in new TrabajarVendedores().TraerVendedores().Rows)
                {
                    string legajo = Convert.ToString(fila["Legajo"]).Trim();
                    vendedores.Add(new KeyValuePair<string, string>(legajo,
                        legajo + " - " + Convert.ToString(fila["Apellido"]).Trim() + ", " +
                        Convert.ToString(fila["Nombre"]).Trim()));
                }

                cbVendedor.ItemsSource = vendedores;
                cbVendedor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los vendedores.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Cargar();
        }

        private string LegajoElegido()
        {
            string legajo = Convert.ToString(cbVendedor.SelectedValue);
            return legajo.Length == 0 ? null : legajo;
        }

        private void Cargar()
        {
            DateTime? desde = dpDesde.SelectedDate;
            DateTime? hasta = dpHasta.SelectedDate;

            if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
            {
                MessageBox.Show("La fecha \"Desde\" no puede ser posterior a la fecha \"Hasta\".",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                vista = TrabajarVentas.TraerVentas(desde, hasta, LegajoElegido()).DefaultView;
                dgVentas.ItemsSource = vista;

                lblCantidad.Text = "Ventas: " + vista.Count;
                lblTotal.Text = "Total: $" + TrabajarVentas.TotalVentas(desde, hasta, LegajoElegido()).ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron leer las ventas.\n" + ex.Message,
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            Cargar();
        }

        private void btnLimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {
            dpDesde.SelectedDate = null;
            dpHasta.SelectedDate = null;
            cbVendedor.SelectedIndex = 0;
            Cargar();
        }

        private void btnAnular_Click(object sender, RoutedEventArgs e)
        {
            DataRowView fila = dgVentas.SelectedItem as DataRowView;
            if (fila == null)
            {
                MessageBox.Show("Seleccione la venta que desea anular.", "Ventas");
                return;
            }

            int nro = Convert.ToInt32(fila["NroFactura"]);

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea anular (eliminar) la factura Nº " + nro + "?",
                "Confirmar anulación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                TrabajarVentas.EliminarVenta(nro);
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo anular la venta.\n" + ex.Message,
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnResumen_Click(object sender, RoutedEventArgs e)
        {
            ResumenVentasWindow ventana = new ResumenVentasWindow(dpDesde.SelectedDate, dpHasta.SelectedDate);
            ventana.ShowDialog();
        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            if (vista == null || vista.Count == 0)
            {
                MessageBox.Show("No hay ventas para exportar.", "Exportar");
                return;
            }

            SaveFileDialog dialogo = new SaveFileDialog();
            dialogo.Filter = "Archivo CSV (*.csv)|*.csv";
            dialogo.FileName = "ventas_" + DateTime.Today.ToString("yyyyMMdd") + ".csv";

            if (dialogo.ShowDialog() == true)
            {
                try
                {
                    Utilidades.ExportarCsv(vista, dialogo.FileName);
                    MessageBox.Show("Archivo exportado correctamente.", "Exportar");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo exportar.\n" + ex.Message,
                        "Exportar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

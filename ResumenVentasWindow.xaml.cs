using System;
using System.Windows;
using ClasesBase;

namespace Vistas
{
    public partial class ResumenVentasWindow : Window
    {
        private readonly DateTime? desde;
        private readonly DateTime? hasta;

        public ResumenVentasWindow(DateTime? desde, DateTime? hasta)
        {
            InitializeComponent();
            this.desde = desde;
            this.hasta = hasta;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string d = desde.HasValue ? desde.Value.ToString("dd/MM/yyyy") : "inicio";
            string h = hasta.HasValue ? hasta.Value.ToString("dd/MM/yyyy") : "hoy";
            lblPeriodo.Text = "Período: " + d + " - " + h;

            try
            {
                dgVendedores.ItemsSource = TrabajarVentas.ResumenPorVendedor(desde, hasta).DefaultView;
                dgProductos.ItemsSource = TrabajarVentas.ResumenPorProducto(desde, hasta).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar el resumen.\n" + ex.Message,
                    "Ventas", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

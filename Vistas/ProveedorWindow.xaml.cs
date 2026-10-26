using System.Windows;
using ClasesBase;

namespace Vistas
{
    public partial class ProveedorWindow : Window
    {
        private Proveedor oProveedor;

        public ProveedorWindow()
        {
            InitializeComponent();
            ModoConsulta();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            HabilitarCampos(true);
            HabilitarBotonesEdicion(true);
            txtCUIT.Focus();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtCUIT.Text.Trim() == "" || txtRazonSocial.Text.Trim() == "" ||
                txtDomicilio.Text.Trim() == "" || txtTelefono.Text.Trim() == "")
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtCUIT.Text.Trim().Length != 11 || !SoloNumeros(txtCUIT.Text.Trim()))
            {
                MessageBox.Show("El CUIT debe tener 11 números.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCUIT.Focus();
                return;
            }

            if (!SoloNumeros(txtTelefono.Text.Trim()))
            {
                MessageBox.Show("El teléfono sólo puede contener números.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTelefono.Focus();
                return;
            }

            if (MessageBox.Show("¿Desea guardar los datos?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                oProveedor = new Proveedor();
                oProveedor.CUIT = txtCUIT.Text.Trim();
                oProveedor.RazonSocial = txtRazonSocial.Text.Trim();
                oProveedor.Domicilio = txtDomicilio.Text.Trim();
                oProveedor.Telefono = txtTelefono.Text.Trim();

                MessageBox.Show("CUIT: " + oProveedor.CUIT +
                    "\nRazón social: " + oProveedor.RazonSocial +
                    "\nDomicilio: " + oProveedor.Domicilio +
                    "\nTeléfono: " + oProveedor.Telefono,
                    "Datos guardados", MessageBoxButton.OK, MessageBoxImage.Information);

                ModoConsulta();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            ModoConsulta();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea salir?", "Confirmar salida",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Close();
        }

        private bool SoloNumeros(string texto)
        {
            foreach (char caracter in texto)
                if (!char.IsDigit(caracter)) return false;
            return true;
        }

        private void LimpiarCampos()
        {
            txtCUIT.Clear(); txtRazonSocial.Clear(); txtDomicilio.Clear(); txtTelefono.Clear();
        }

        private void HabilitarCampos(bool valor)
        {
            txtCUIT.IsEnabled = valor; txtRazonSocial.IsEnabled = valor;
            txtDomicilio.IsEnabled = valor; txtTelefono.IsEnabled = valor;
        }

        private void ModoConsulta()
        {
            HabilitarCampos(false);
            HabilitarBotonesEdicion(false);
        }

        private void HabilitarBotonesEdicion(bool editando)
        {
            btnGuardar.IsEnabled = editando; btnCancelar.IsEnabled = editando;
            btnNuevo.IsEnabled = !editando; btnModificar.IsEnabled = !editando; btnEliminar.IsEnabled = !editando;
            btnPrimero.IsEnabled = !editando; btnAnterior.IsEnabled = !editando;
            btnSiguiente.IsEnabled = !editando; btnUltimo.IsEnabled = !editando;
        }
    }
}

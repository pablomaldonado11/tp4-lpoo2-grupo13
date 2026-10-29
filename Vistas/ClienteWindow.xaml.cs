using System;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class ClienteWindow : Window
    {
        private Cliente oCliente;

        public ClienteWindow()
        {
            InitializeComponent();
            ModoConsulta();
            try
            {
                lstClientes.ItemsSource = new TrabajarClientes().TraerClientes();
                if (lstClientes.Items.Count > 0) lstClientes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los clientes.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            datosCliente.DataContext = lstClientes.SelectedItem as Cliente;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            datosCliente.DataContext = new Cliente();
            LimpiarCampos(); HabilitarCampos(true); HabilitarBotonesEdicion(true); txtDNI.Focus();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtDNI.Text.Trim() == "" || txtApellido.Text.Trim() == "" ||
                txtNombre.Text.Trim() == "" || txtDireccion.Text.Trim() == "")
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string dni = txtDNI.Text.Trim();
            if ((dni.Length != 7 && dni.Length != 8) || !SoloNumeros(dni))
            {
                MessageBox.Show("El DNI debe tener 7 u 8 números.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDNI.Focus();
                return;
            }

            if (MessageBox.Show("¿Desea guardar los datos?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                oCliente = new Cliente();
                oCliente.DNI = dni;
                oCliente.Apellido = txtApellido.Text.Trim();
                oCliente.Nombre = txtNombre.Text.Trim();
                oCliente.Dirección = txtDireccion.Text.Trim();

                MessageBox.Show("DNI: " + oCliente.DNI +
                    "\nApellido: " + oCliente.Apellido +
                    "\nNombre: " + oCliente.Nombre +
                    "\nDirección: " + oCliente.Dirección,
                    "Datos guardados", MessageBoxButton.OK, MessageBoxImage.Information);

                ModoConsulta();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            datosCliente.DataContext = lstClientes.SelectedItem as Cliente;
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
            txtDNI.Clear(); txtApellido.Clear(); txtNombre.Clear(); txtDireccion.Clear();
        }

        private void HabilitarCampos(bool valor)
        {
            txtDNI.IsEnabled = valor; txtApellido.IsEnabled = valor;
            txtNombre.IsEnabled = valor; txtDireccion.IsEnabled = valor;
        }

        private void ModoConsulta()
        {
            HabilitarCampos(false); HabilitarBotonesEdicion(false);
        }

        private void HabilitarBotonesEdicion(bool editando)
        {
            lstClientes.IsEnabled = !editando;
            btnGuardar.IsEnabled = editando; btnCancelar.IsEnabled = editando;
            btnNuevo.IsEnabled = !editando; btnModificar.IsEnabled = !editando; btnEliminar.IsEnabled = !editando;
            btnPrimero.IsEnabled = !editando; btnAnterior.IsEnabled = !editando;
            btnSiguiente.IsEnabled = !editando; btnUltimo.IsEnabled = !editando;
        }
    }
}

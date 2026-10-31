using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class ClienteWindow : Window
    {
        private ObservableCollection<Cliente> clientes;
        private Cliente clienteSeleccionado;
        private bool esNuevo;
        private string dniOriginal;

        public ClienteWindow()
        {
            InitializeComponent();
            ModoConsulta();
            CargarClientes();
        }

        private void CargarClientes()
        {
            try
            {
                clientes = TrabajarClientes.TraerClientes();
                lstClientes.ItemsSource = clientes;
                if (clientes.Count > 0)
                    lstClientes.SelectedIndex = 0;
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
            esNuevo = true;
            clienteSeleccionado = null;
            dniOriginal = "";
            datosCliente.DataContext = new Cliente();
            HabilitarCampos(true);
            HabilitarBotonesEdicion(true);
            txtDNI.Focus();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            clienteSeleccionado = lstClientes.SelectedItem as Cliente;

            if (clienteSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Modificar",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            esNuevo = false;
            dniOriginal = clienteSeleccionado.DNI;

            // Se usa una copia para no cambiar la lista hasta presionar Guardar.
            datosCliente.DataContext = new Cliente
            {
                DNI = clienteSeleccionado.DNI,
                Apellido = clienteSeleccionado.Apellido,
                Nombre = clienteSeleccionado.Nombre,
                Direccion = clienteSeleccionado.Direccion
            };

            HabilitarCampos(true);
            HabilitarBotonesEdicion(true);
            txtApellido.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            Cliente cliente = lstClientes.SelectedItem as Cliente;

            if (cliente == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Eliminar",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar al cliente " + cliente.Apellido + ", " + cliente.Nombre + "?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            try
            {
                int posicion = lstClientes.SelectedIndex;
                TrabajarClientes.EliminarCliente(cliente.DNI);
                clientes.Remove(cliente);

                if (clientes.Count > 0)
                    lstClientes.SelectedIndex = Math.Min(posicion, clientes.Count - 1);
                else
                    datosCliente.DataContext = null;

                MessageBox.Show("Cliente eliminado correctamente.", "Eliminar",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el cliente. Puede tener ventas asociadas.\n" + ex.Message,
                    "Eliminar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            Cliente clienteEditado = datosCliente.DataContext as Cliente;
            if (clienteEditado == null)
                return;

            if (MessageBox.Show("¿Desea guardar los datos?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                clienteEditado.DNI = dni;
                clienteEditado.Apellido = txtApellido.Text.Trim();
                clienteEditado.Nombre = txtNombre.Text.Trim();
                clienteEditado.Direccion = txtDireccion.Text.Trim();

                if (esNuevo)
                {
                    if (TrabajarClientes.ExisteDNI(clienteEditado.DNI))
                    {
                        MessageBox.Show("Ya existe un cliente con ese DNI.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtDNI.Focus();
                        return;
                    }

                    TrabajarClientes.AgregarCliente(clienteEditado);
                    clientes.Add(clienteEditado);
                    lstClientes.SelectedItem = clienteEditado;
                }
                else
                {
                    if (clienteEditado.DNI != dniOriginal && TrabajarClientes.ExisteDNI(clienteEditado.DNI))
                    {
                        MessageBox.Show("Ya existe un cliente con ese DNI.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtDNI.Focus();
                        return;
                    }

                    TrabajarClientes.ModificarCliente(clienteEditado, dniOriginal);
                    clienteSeleccionado.DNI = clienteEditado.DNI;
                    clienteSeleccionado.Apellido = clienteEditado.Apellido;
                    clienteSeleccionado.Nombre = clienteEditado.Nombre;
                    clienteSeleccionado.Direccion = clienteEditado.Direccion;
                    lstClientes.SelectedItem = clienteSeleccionado;
                }

                ModoConsulta();
                datosCliente.DataContext = lstClientes.SelectedItem as Cliente;
                MessageBox.Show("Los datos se guardaron correctamente.", "Clientes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message,
                    "Clientes", MessageBoxButton.OK, MessageBoxImage.Error);
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

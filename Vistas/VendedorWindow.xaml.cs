using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class VendedorWindow : Window
    {
        private readonly TrabajarVendedores datosVendedores;
        private bool esNuevo;

        public VendedorWindow()
        {
            InitializeComponent();
            datosVendedores = new TrabajarVendedores();
            CargarGrilla();
            ModoConsulta();
        }

        private void CargarGrilla()
        {
            try
            {
                dgvVendedores.ItemsSource = datosVendedores.TraerVendedores().DefaultView;
                if (dgvVendedores.Items.Count > 0)
                    dgvVendedores.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los vendedores.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = true;
            LimpiarCampos();
            HabilitarEdicion(true);
            txtLegajo.IsEnabled = true;
            txtLegajo.Focus();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgvVendedores.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un vendedor.", "Vendedores");
                return;
            }

            esNuevo = false;
            MostrarVendedorSeleccionado();
            HabilitarEdicion(true);
            txtLegajo.IsEnabled = false;
            txtApellido.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView fila = dgvVendedores.SelectedItem as DataRowView;
            if (fila == null)
            {
                MessageBox.Show("Seleccione un vendedor.", "Vendedores");
                return;
            }

            string legajo = fila["Legajo"].ToString();
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea eliminar el vendedor " + legajo + "?", "Confirmar baja",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                datosVendedores.EliminarVendedor(legajo);
                MessageBox.Show("Vendedor eliminado correctamente.", "Vendedores");
                CargarGrilla();
                ModoConsulta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el vendedor.\n" + ex.Message,
                    "Vendedores", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtLegajo.Text.Trim() == "" || txtApellido.Text.Trim() == "" ||
                txtNombre.Text.Trim() == "")
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Vendedor vendedor = new Vendedor();
            vendedor.Legajo = txtLegajo.Text.Trim();
            vendedor.Apellido = txtApellido.Text.Trim();
            vendedor.Nombre = txtNombre.Text.Trim();

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea guardar los datos?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                if (esNuevo)
                    datosVendedores.AgregarVendedor(vendedor);
                else
                    datosVendedores.ModificarVendedor(vendedor);

                MessageBox.Show("Datos guardados correctamente.", "Vendedores");
                CargarGrilla();
                ModoConsulta();
                MostrarVendedorSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message,
                    "Vendedores", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ModoConsulta();
            MostrarVendedorSeleccionado();
        }

        private void MostrarVendedorSeleccionado()
        {
            if (!dgvVendedores.IsEnabled) return;

            DataRowView fila = dgvVendedores.SelectedItem as DataRowView;
            if (fila == null)
            {
                LimpiarCampos();
                return;
            }

            txtLegajo.Text = fila["Legajo"].ToString();
            txtApellido.Text = fila["Apellido"].ToString();
            txtNombre.Text = fila["Nombre"].ToString();
        }

        private void HabilitarEdicion(bool editando)
        {
            txtLegajo.IsEnabled = editando;
            txtApellido.IsEnabled = editando;
            txtNombre.IsEnabled = editando;
            dgvVendedores.IsEnabled = !editando;

            btnGuardar.IsEnabled = editando;
            btnCancelar.IsEnabled = editando;
            btnNuevo.IsEnabled = !editando;
            btnModificar.IsEnabled = !editando;
            btnEliminar.IsEnabled = !editando;
            btnPrimero.IsEnabled = !editando;
            btnAnterior.IsEnabled = !editando;
            btnSiguiente.IsEnabled = !editando;
            btnUltimo.IsEnabled = !editando;
        }

        private void ModoConsulta()
        {
            HabilitarEdicion(false);
        }

        private void LimpiarCampos()
        {
            txtLegajo.Clear();
            txtApellido.Clear();
            txtNombre.Clear();
        }

        private void dgvVendedores_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            MostrarVendedorSeleccionado();
        }

        private void btnPrimero_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(0);
        }

        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvVendedores.SelectedIndex - 1);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvVendedores.SelectedIndex + 1);
        }

        private void btnUltimo_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvVendedores.Items.Count - 1);
        }

        private void SeleccionarFila(int indice)
        {
            if (indice < 0 || indice >= dgvVendedores.Items.Count) return;
            dgvVendedores.SelectedIndex = indice;
            dgvVendedores.ScrollIntoView(dgvVendedores.SelectedItem);
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea salir?", "Confirmar salida",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta == MessageBoxResult.Yes) Close();
        }
    }
}

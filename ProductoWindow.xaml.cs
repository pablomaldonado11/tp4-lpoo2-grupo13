using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class ProductoWindow : Window
    {
        private readonly TrabajarProductos datosProductos;
        private Producto productoActual;
        private bool esNuevo;

        public ProductoWindow()
        {
            InitializeComponent();
            datosProductos = new TrabajarProductos();
            CargarGrilla();
            ModoConsulta();
        }

        private void CargarGrilla()
        {
            try
            {
                dgvProductos.ItemsSource = datosProductos.TraerProductos().DefaultView;

                if (dgvProductos.Items.Count > 0)
                    dgvProductos.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los productos.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = true;
            productoActual = new Producto();
            DataContext = productoActual;
            HabilitarEdicion(true);
            txtCodProducto.IsEnabled = true;
            txtCodProducto.Focus();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (!CargarProductoSeleccionado())
            {
                MessageBox.Show("Seleccione un producto.", "Productos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            esNuevo = false;
            HabilitarEdicion(true);
            txtCodProducto.IsEnabled = false;
            txtCategoria.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView fila = dgvProductos.SelectedItem as DataRowView;
            if (fila == null)
            {
                MessageBox.Show("Seleccione un producto.", "Productos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string codigo = fila["CodProducto"].ToString();
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea eliminar el producto " + codigo + "?", "Confirmar baja",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                datosProductos.EliminarProducto(codigo);
                MessageBox.Show("Producto eliminado correctamente.", "Productos");
                CargarGrilla();
                ModoConsulta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el producto.\n" + ex.Message,
                    "Productos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (productoActual == null || productoActual.Error != "")
            {
                string mensaje = productoActual == null ?
                    "Debe ingresar los datos." : productoActual.Error;
                MessageBox.Show(mensaje, "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea guardar los datos?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                if (esNuevo)
                    datosProductos.AgregarProducto(productoActual);
                else
                    datosProductos.ModificarProducto(productoActual);

                MessageBox.Show("Datos guardados correctamente.", "Productos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                CargarGrilla();
                ModoConsulta();
                MostrarProductoSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message,
                    "Productos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ModoConsulta();
            MostrarProductoSeleccionado();
        }

        private bool CargarProductoSeleccionado()
        {
            DataRowView fila = dgvProductos.SelectedItem as DataRowView;
            if (fila == null) return false;

            productoActual = new Producto();
            productoActual.CodProducto = fila["CodProducto"].ToString();
            productoActual.Categoria = fila["Categoria"].ToString();
            productoActual.Color = fila["Color"].ToString();
            productoActual.Descripcion = fila["Descripcion"].ToString();
            productoActual.Precio = Convert.ToDecimal(fila["Precio"]).ToString("0.00");
            DataContext = productoActual;
            return true;
        }

        private void MostrarProductoSeleccionado()
        {
            if (dgvProductos.IsEnabled)
                CargarProductoSeleccionado();
        }

        private void HabilitarEdicion(bool editando)
        {
            txtCodProducto.IsEnabled = editando;
            txtCategoria.IsEnabled = editando;
            txtColor.IsEnabled = editando;
            txtDescripcion.IsEnabled = editando;
            txtPrecio.IsEnabled = editando;
            dgvProductos.IsEnabled = !editando;

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

        private void dgvProductos_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            MostrarProductoSeleccionado();
        }

        private void btnPrimero_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(0);
        }

        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvProductos.SelectedIndex - 1);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvProductos.SelectedIndex + 1);
        }

        private void btnUltimo_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvProductos.Items.Count - 1);
        }

        private void SeleccionarFila(int indice)
        {
            if (indice < 0 || indice >= dgvProductos.Items.Count) return;
            dgvProductos.SelectedIndex = indice;
            dgvProductos.ScrollIntoView(dgvProductos.SelectedItem);
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

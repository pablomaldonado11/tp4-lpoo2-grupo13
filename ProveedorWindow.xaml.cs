using System;
using System.Collections.Generic;
using System.Windows;
using ClasesBase;

namespace Vistas
{
    public partial class ProveedorWindow : Window
    {
        private Proveedor oProveedor;

        // --- Agregado: persistencia en base de datos y navegación ---
        private List<Proveedor> lista = new List<Proveedor>();
        private int pos = -1;
        private bool esNuevo = false;

        public ProveedorWindow()
        {
            InitializeComponent();
            CargarProveedores();
            ModoConsulta();
            MostrarActual();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = true;
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

                try
                {
                    if (esNuevo)
                    {
                        if (TrabajarProveedores.ExisteCUIT(oProveedor.CUIT))
                        {
                            MessageBox.Show("Ya existe un proveedor con ese CUIT.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        TrabajarProveedores.AgregarProveedor(oProveedor);
                    }
                    else
                    {
                        TrabajarProveedores.ModificarProveedor(oProveedor);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message, "Base de datos",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("CUIT: " + oProveedor.CUIT +
                    "\nRazón social: " + oProveedor.RazonSocial +
                    "\nDomicilio: " + oProveedor.Domicilio +
                    "\nTeléfono: " + oProveedor.Telefono,
                    "Datos guardados", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarProveedores();
                pos = lista.FindIndex(x => x.CUIT == oProveedor.CUIT);
                esNuevo = false;
                ModoConsulta();
                MostrarActual();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = false;
            LimpiarCampos();
            ModoConsulta();
            MostrarActual();
        }

        // ---------- Agregado: modificar, eliminar, navegar y listar ----------

        private void CargarProveedores()
        {
            try
            {
                lista = TrabajarProveedores.TraerProveedores();
                if (pos >= lista.Count) pos = lista.Count - 1;
                if (pos < 0 && lista.Count > 0) pos = 0;
            }
            catch (Exception ex)
            {
                lista = new List<Proveedor>();
                pos = -1;
                MessageBox.Show("No se pudieron cargar los proveedores.\n" + ex.Message, "Base de datos",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Muestra el proveedor de la posición actual (o deja los campos vacíos si no hay)
        private void MostrarActual()
        {
            if (pos < 0 || pos >= lista.Count)
            {
                LimpiarCampos();
                return;
            }

            txtCUIT.Text = lista[pos].CUIT;
            txtRazonSocial.Text = lista[pos].RazonSocial;
            txtDomicilio.Text = lista[pos].Domicilio;
            txtTelefono.Text = lista[pos].Telefono;
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (pos < 0 || pos >= lista.Count)
            {
                MessageBox.Show("No hay un proveedor para modificar.", "Proveedores");
                return;
            }

            esNuevo = false;
            HabilitarCampos(true);
            txtCUIT.IsEnabled = false;   // el CUIT es la clave: no se modifica
            HabilitarBotonesEdicion(true);
            txtRazonSocial.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (pos < 0 || pos >= lista.Count)
            {
                MessageBox.Show("No hay un proveedor para eliminar.", "Proveedores");
                return;
            }

            Proveedor actual = lista[pos];
            if (MessageBox.Show("¿Desea eliminar al proveedor " + actual.RazonSocial + "?", "Confirmar baja",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                TrabajarProveedores.EliminarProveedor(actual.CUIT);
                MessageBox.Show("Proveedor eliminado correctamente.", "Proveedores");
                CargarProveedores();
                MostrarActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el proveedor.\n" + ex.Message, "Proveedores",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IrA(int nuevaPos)
        {
            if (nuevaPos < 0 || nuevaPos >= lista.Count) return;
            pos = nuevaPos;
            MostrarActual();
        }

        private void btnPrimero_Click(object sender, RoutedEventArgs e) { IrA(0); }
        private void btnAnterior_Click(object sender, RoutedEventArgs e) { IrA(pos - 1); }
        private void btnSiguiente_Click(object sender, RoutedEventArgs e) { IrA(pos + 1); }
        private void btnUltimo_Click(object sender, RoutedEventArgs e) { IrA(lista.Count - 1); }

        private void btnListar_Click(object sender, RoutedEventArgs e)
        {
            ListadoProveedoresWindow ventana = new ListadoProveedoresWindow();
            ventana.ShowDialog();
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
            btnListar.IsEnabled = !editando;
        }
    }
}

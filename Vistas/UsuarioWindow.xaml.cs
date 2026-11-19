using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class UsuarioWindow : Window
    {
        private readonly string usuarioActual;
        private bool esNuevo;
        private int idSeleccionado;

        public UsuarioWindow(string usuarioActual)
        {
            InitializeComponent();
            this.usuarioActual = usuarioActual;
            cmbRol.ItemsSource = new string[] { "Admin", "Vendedor" };
            PrepararDatos();
        }

        private void PrepararDatos()
        {
            try
            {
                TrabajarUsuarios.AsegurarTabla();
                CargarGrilla();
                ModoConsulta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los usuarios.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarGrilla()
        {
            dgvUsuarios.ItemsSource = TrabajarUsuarios.TraerUsuarios().DefaultView;
            if (dgvUsuarios.Items.Count > 0) dgvUsuarios.SelectedIndex = 0;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = true;
            idSeleccionado = 0;
            LimpiarCampos();
            cmbRol.SelectedItem = "Vendedor";
            chkActivo.IsChecked = true;
            HabilitarEdicion(true);
            txtUsuario.Focus();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgvUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios");
                return;
            }
            esNuevo = false;
            MostrarSeleccionado();
            HabilitarEdicion(true);
            txtUsuario.IsEnabled = false;
            if (EsUsuarioActual(txtUsuario.Text))
            {
                cmbRol.IsEnabled = false;
                chkActivo.IsEnabled = false;
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
            if (fila == null) { MessageBox.Show("Seleccione un usuario."); return; }

            string nombre = fila["NombreUsuario"].ToString();
            if (EsUsuarioActual(nombre))
            {
                MessageBox.Show("No puede eliminar el usuario con el que inició sesión.");
                return;
            }
            if (fila["Rol"].ToString() == "Admin" && (bool)fila["Activo"] &&
                TrabajarUsuarios.ContarAdministradoresActivos() <= 1)
            {
                MessageBox.Show("No se puede eliminar el último administrador activo.");
                return;
            }
            if (MessageBox.Show("¿Desea eliminar el usuario " + nombre + "?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                TrabajarUsuarios.EliminarUsuario((int)fila["IdUsuario"]);
                CargarGrilla();
                ModoConsulta();
            }
            catch (Exception ex) { MessageBox.Show("No se pudo eliminar.\n" + ex.Message); }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string rol = cmbRol.SelectedItem as string;
            string password = txtPassword.Password;
            string confirmar = txtConfirmar.Password;

            if (nombreUsuario == "" || apellido == "" || nombre == "" || rol == null)
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación");
                return;
            }
            if (esNuevo || password != "" || confirmar != "")
            {
                if (password.Length < 4)
                {
                    MessageBox.Show("La contraseña debe tener al menos 4 caracteres.");
                    return;
                }
                if (password != confirmar)
                {
                    MessageBox.Show("Las contraseñas no coinciden.");
                    return;
                }
            }

            try
            {
                if (esNuevo && TrabajarUsuarios.ExisteUsuario(nombreUsuario))
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre.");
                    return;
                }

                DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
                if (!esNuevo && fila != null && fila["Rol"].ToString() == "Admin" &&
                    (bool)fila["Activo"] && (rol != "Admin" || chkActivo.IsChecked != true) &&
                    TrabajarUsuarios.ContarAdministradoresActivos() <= 1)
                {
                    MessageBox.Show("Debe quedar al menos un administrador activo.");
                    return;
                }

                if (MessageBox.Show("¿Desea guardar los datos?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

                Usuario usuario = new Usuario();
                usuario.IdUsuario = idSeleccionado;
                usuario.NombreUsuario = nombreUsuario;
                usuario.Apellido = apellido;
                usuario.Nombre = nombre;
                usuario.Rol = rol;
                usuario.Activo = chkActivo.IsChecked == true;
                usuario.Password = password;

                if (esNuevo) TrabajarUsuarios.AgregarUsuario(usuario);
                else TrabajarUsuarios.ModificarUsuario(usuario);

                MessageBox.Show("Datos guardados correctamente.");
                CargarGrilla();
                ModoConsulta();
                MostrarSeleccionado();
            }
            catch (Exception ex) { MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message); }
        }

        private bool EsUsuarioActual(string nombre)
        {
            return String.Equals(nombre, usuarioActual, StringComparison.OrdinalIgnoreCase);
        }

        private void MostrarSeleccionado()
        {
            if (!dgvUsuarios.IsEnabled) return;
            DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
            if (fila == null) { LimpiarCampos(); return; }
            idSeleccionado = (int)fila["IdUsuario"];
            txtUsuario.Text = fila["NombreUsuario"].ToString();
            txtApellido.Text = fila["Apellido"].ToString();
            txtNombre.Text = fila["Nombre"].ToString();
            cmbRol.SelectedItem = fila["Rol"].ToString();
            chkActivo.IsChecked = (bool)fila["Activo"];
            txtPassword.Clear();
            txtConfirmar.Clear();
        }

        private void HabilitarEdicion(bool valor)
        {
            txtUsuario.IsEnabled = valor; txtApellido.IsEnabled = valor;
            txtNombre.IsEnabled = valor; txtPassword.IsEnabled = valor;
            txtConfirmar.IsEnabled = valor; cmbRol.IsEnabled = valor;
            chkActivo.IsEnabled = valor; dgvUsuarios.IsEnabled = !valor;
            btnGuardar.IsEnabled = valor; btnCancelar.IsEnabled = valor;
            btnNuevo.IsEnabled = !valor; btnModificar.IsEnabled = !valor;
            btnEliminar.IsEnabled = !valor; btnPrimero.IsEnabled = !valor;
            btnAnterior.IsEnabled = !valor; btnSiguiente.IsEnabled = !valor;
            btnUltimo.IsEnabled = !valor;
        }

        private void ModoConsulta() { HabilitarEdicion(false); }
        private void LimpiarCampos()
        {
            txtUsuario.Clear(); txtApellido.Clear(); txtNombre.Clear();
            txtPassword.Clear(); txtConfirmar.Clear();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        { ModoConsulta(); MostrarSeleccionado(); }
        private void dgvUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { MostrarSeleccionado(); }
        private void btnPrimero_Click(object sender, RoutedEventArgs e) { SeleccionarFila(0); }
        private void btnAnterior_Click(object sender, RoutedEventArgs e) { SeleccionarFila(dgvUsuarios.SelectedIndex - 1); }
        private void btnSiguiente_Click(object sender, RoutedEventArgs e) { SeleccionarFila(dgvUsuarios.SelectedIndex + 1); }
        private void btnUltimo_Click(object sender, RoutedEventArgs e) { SeleccionarFila(dgvUsuarios.Items.Count - 1); }
        private void SeleccionarFila(int indice)
        {
            if (indice < 0 || indice >= dgvUsuarios.Items.Count) return;
            dgvUsuarios.SelectedIndex = indice;
            dgvUsuarios.ScrollIntoView(dgvUsuarios.SelectedItem);
        }
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea salir?", "Confirmar salida",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) Close();
        }
    }
}

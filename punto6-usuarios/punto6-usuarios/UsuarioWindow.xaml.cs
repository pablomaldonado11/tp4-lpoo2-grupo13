using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ClasesBase;

namespace Vistas
{
    public partial class UsuarioWindow : Window
    {
        private const int LargoMinimoPassword = 4;

        private readonly string usuarioActual;   // usuario que inicio sesion
        private bool esNuevo;
        private int idSeleccionado;

        public UsuarioWindow(string usuarioActual)
        {
            InitializeComponent();
            this.usuarioActual = usuarioActual;

            cmbRol.ItemsSource = new string[] { "Admin", "Vendedor" };
            cmbRol.SelectedItem = "Vendedor";

            try
            {
                TrabajarUsuarios.AsegurarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo preparar la tabla de usuarios.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            CargarGrilla();
            ModoConsulta();
        }

        private void CargarGrilla()
        {
            try
            {
                dgvUsuarios.ItemsSource = TrabajarUsuarios.TraerUsuarios().DefaultView;
                if (dgvUsuarios.Items.Count > 0)
                    dgvUsuarios.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los usuarios.\n" + ex.Message,
                    "Base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            esNuevo = true;
            idSeleccionado = 0;
            LimpiarCampos();
            cmbRol.SelectedItem = "Vendedor";
            chkActivo.IsChecked = true;
            HabilitarEdicion(true);
            txtUsuario.IsEnabled = true;
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
            MostrarUsuarioSeleccionado();
            HabilitarEdicion(true);
            txtUsuario.IsEnabled = false;   // el nombre de usuario no se cambia

            // Un usuario no puede cambiarse a si mismo el rol ni desactivarse
            if (EsUsuarioActual(txtUsuario.Text))
            {
                cmbRol.IsEnabled = false;
                chkActivo.IsEnabled = false;
            }

            txtApellido.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
            if (fila == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios");
                return;
            }

            string nombre = fila["NombreUsuario"].ToString();

            if (EsUsuarioActual(nombre))
            {
                MessageBox.Show("No puede eliminar el usuario con el que inició sesión.",
                    "Usuarios", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool esAdminActivo = fila["Rol"].ToString() == "Admin" && (bool)fila["Activo"];
            if (esAdminActivo && TrabajarUsuarios.ContarAdministradoresActivos() <= 1)
            {
                MessageBox.Show("No se puede eliminar el último administrador activo.",
                    "Usuarios", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea eliminar el usuario " + nombre + "?", "Confirmar baja",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes) return;

            try
            {
                TrabajarUsuarios.EliminarUsuario((int)fila["IdUsuario"]);
                MessageBox.Show("Usuario eliminado correctamente.", "Usuarios");
                CargarGrilla();
                ModoConsulta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el usuario.\n" + ex.Message,
                    "Usuarios", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string rol = cmbRol.SelectedItem as string;
            bool activo = chkActivo.IsChecked == true;
            string password = txtPassword.Password;
            string confirmar = txtConfirmar.Password;

            if (nombreUsuario == "" || apellido == "" || nombre == "" || rol == null)
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // La contraseña es obligatoria al crear; al modificar, vacia = no cambiarla
            if (esNuevo || password != "" || confirmar != "")
            {
                if (password.Length < LargoMinimoPassword)
                {
                    MessageBox.Show("La contraseña debe tener al menos " + LargoMinimoPassword +
                        " caracteres.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (password != confirmar)
                {
                    MessageBox.Show("Las contraseñas no coinciden.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConfirmar.Clear();
                    txtConfirmar.Focus();
                    return;
                }
            }

            try
            {
                if (esNuevo && TrabajarUsuarios.ExisteUsuario(nombreUsuario))
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre.", "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsuario.Focus();
                    return;
                }

                // No dejar el sistema sin ningun administrador activo
                if (!esNuevo)
                {
                    DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
                    bool eraAdminActivo = fila != null &&
                        fila["Rol"].ToString() == "Admin" && (bool)fila["Activo"];
                    bool seguiraSiendoAdminActivo = rol == "Admin" && activo;

                    if (eraAdminActivo && !seguiraSiendoAdminActivo &&
                        TrabajarUsuarios.ContarAdministradoresActivos() <= 1)
                    {
                        MessageBox.Show("Debe quedar al menos un administrador activo.",
                            "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                MessageBoxResult respuesta = MessageBox.Show(
                    "¿Desea guardar los datos?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (respuesta != MessageBoxResult.Yes) return;

                Usuario usuario = new Usuario();
                usuario.IdUsuario = idSeleccionado;
                usuario.NombreUsuario = nombreUsuario;
                usuario.Apellido = apellido;
                usuario.Nombre = nombre;
                usuario.Rol = rol;
                usuario.Activo = activo;
                usuario.Password = password;

                if (esNuevo)
                    TrabajarUsuarios.AgregarUsuario(usuario);
                else
                    TrabajarUsuarios.ModificarUsuario(usuario);

                MessageBox.Show("Datos guardados correctamente.", "Usuarios");
                CargarGrilla();
                ModoConsulta();
                MostrarUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron guardar los datos.\n" + ex.Message,
                    "Usuarios", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ModoConsulta();
            MostrarUsuarioSeleccionado();
        }

        private bool EsUsuarioActual(string nombreUsuario)
        {
            return string.Equals(nombreUsuario, usuarioActual,
                StringComparison.OrdinalIgnoreCase);
        }

        private void MostrarUsuarioSeleccionado()
        {
            if (!dgvUsuarios.IsEnabled) return;

            DataRowView fila = dgvUsuarios.SelectedItem as DataRowView;
            if (fila == null)
            {
                LimpiarCampos();
                return;
            }

            idSeleccionado = (int)fila["IdUsuario"];
            txtUsuario.Text = fila["NombreUsuario"].ToString();
            txtApellido.Text = fila["Apellido"].ToString();
            txtNombre.Text = fila["Nombre"].ToString();
            cmbRol.SelectedItem = fila["Rol"].ToString();
            chkActivo.IsChecked = (bool)fila["Activo"];
            txtPassword.Clear();
            txtConfirmar.Clear();
        }

        private void HabilitarEdicion(bool editando)
        {
            txtUsuario.IsEnabled = editando;
            txtApellido.IsEnabled = editando;
            txtNombre.IsEnabled = editando;
            txtPassword.IsEnabled = editando;
            txtConfirmar.IsEnabled = editando;
            cmbRol.IsEnabled = editando;
            chkActivo.IsEnabled = editando;
            dgvUsuarios.IsEnabled = !editando;

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
            txtUsuario.Clear();
            txtApellido.Clear();
            txtNombre.Clear();
            txtPassword.Clear();
            txtConfirmar.Clear();
        }

        private void dgvUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarUsuarioSeleccionado();
        }

        private void btnPrimero_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(0);
        }

        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvUsuarios.SelectedIndex - 1);
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvUsuarios.SelectedIndex + 1);
        }

        private void btnUltimo_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarFila(dgvUsuarios.Items.Count - 1);
        }

        private void SeleccionarFila(int indice)
        {
            if (indice < 0 || indice >= dgvUsuarios.Items.Count) return;
            dgvUsuarios.SelectedIndex = indice;
            dgvUsuarios.ScrollIntoView(dgvUsuarios.SelectedItem);
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

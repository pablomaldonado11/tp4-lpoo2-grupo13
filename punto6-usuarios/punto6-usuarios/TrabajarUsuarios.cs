using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ClasesBase
{
    public class TrabajarUsuarios
    {
        private static string ObtenerCadenaConexion()
        {
            return ConfigurationManager
                .ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        // ------------------------------------------------------------------
        // Contraseñas: en la base se guarda el hash SHA-256 (nunca el texto)
        // ------------------------------------------------------------------
        public static string CalcularHash(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }

        // ------------------------------------------------------------------
        // Crea la tabla Usuario si no existe y carga los usuarios iniciales
        // (admin / admin123 y vendedor / vend123, los mismos de antes).
        // Se llama al abrir el Login, asi funciona en cualquier PC sin tener
        // que ejecutar el script a mano.
        // ------------------------------------------------------------------
        public static void AsegurarTabla()
        {
            string consulta =
                "IF OBJECT_ID('Usuario', 'U') IS NULL " +
                "BEGIN " +
                "  CREATE TABLE Usuario ( " +
                "    IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "    NombreUsuario VARCHAR(30) NOT NULL UNIQUE, " +
                "    Password VARCHAR(64) NOT NULL, " +
                "    Apellido VARCHAR(50) NOT NULL, " +
                "    Nombre VARCHAR(50) NOT NULL, " +
                "    Rol VARCHAR(20) NOT NULL " +
                "      CHECK (Rol IN ('Admin', 'Vendedor')), " +
                "    Activo BIT NOT NULL DEFAULT 1 " +
                "  ) " +
                "END";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            {
                conexion.Open();

                using (SqlCommand crear = new SqlCommand(consulta, conexion))
                {
                    crear.ExecuteNonQuery();
                }

                int cantidad;
                using (SqlCommand contar = new SqlCommand("SELECT COUNT(*) FROM Usuario", conexion))
                {
                    cantidad = (int)contar.ExecuteScalar();
                }

                if (cantidad == 0)
                {
                    InsertarUsuarioInicial(conexion, "admin", "admin123",
                        "Administrador", "Sistema", "Admin");
                    InsertarUsuarioInicial(conexion, "vendedor", "vend123",
                        "Vendedor", "Sistema", "Vendedor");
                }
            }
        }

        private static void InsertarUsuarioInicial(SqlConnection conexion, string usuario,
            string password, string apellido, string nombre, string rol)
        {
            string consulta = "INSERT INTO Usuario (NombreUsuario, Password, Apellido, Nombre, Rol, Activo) " +
                              "VALUES (@usuario, @password, @apellido, @nombre, @rol, 1)";
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@password", CalcularHash(password));
                comando.Parameters.AddWithValue("@apellido", apellido);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@rol", rol);
                comando.ExecuteNonQuery();
            }
        }

        // ------------------------------------------------------------------
        // LOGIN
        // ------------------------------------------------------------------
        public static Usuario Autenticar(string nombreUsuario, string password,
            out ResultadoLogin resultado)
        {
            string consulta = "SELECT IdUsuario, NombreUsuario, Password, Apellido, Nombre, Rol, Activo " +
                              "FROM Usuario WHERE NombreUsuario = @usuario";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        resultado = ResultadoLogin.UsuarioInexistente;
                        return null;
                    }

                    string hashGuardado = lector["Password"].ToString();
                    if (!string.Equals(hashGuardado, CalcularHash(password),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        resultado = ResultadoLogin.PasswordIncorrecta;
                        return null;
                    }

                    if (!(bool)lector["Activo"])
                    {
                        resultado = ResultadoLogin.UsuarioInactivo;
                        return null;
                    }

                    resultado = ResultadoLogin.Correcto;
                    return LeerUsuario(lector);
                }
            }
        }

        // ------------------------------------------------------------------
        // ABM
        // ------------------------------------------------------------------
        public static DataTable TraerUsuarios()
        {
            DataTable tabla = new DataTable();
            string consulta = "SELECT IdUsuario, NombreUsuario, Apellido, Nombre, Rol, Activo " +
                              "FROM Usuario ORDER BY NombreUsuario";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            {
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(tabla);
            }
            return tabla;
        }

        public static bool ExisteUsuario(string nombreUsuario)
        {
            string consulta = "SELECT COUNT(*) FROM Usuario WHERE NombreUsuario = @usuario";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        // Cantidad de administradores activos (para no quedarse sin ninguno)
        public static int ContarAdministradoresActivos()
        {
            string consulta = "SELECT COUNT(*) FROM Usuario WHERE Rol = 'Admin' AND Activo = 1";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();
                return (int)comando.ExecuteScalar();
            }
        }

        public static void AgregarUsuario(Usuario usuario)
        {
            string consulta = "INSERT INTO Usuario (NombreUsuario, Password, Apellido, Nombre, Rol, Activo) " +
                              "VALUES (@usuario, @password, @apellido, @nombre, @rol, @activo)";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", usuario.NombreUsuario);
                comando.Parameters.AddWithValue("@password", CalcularHash(usuario.Password));
                comando.Parameters.AddWithValue("@apellido", usuario.Apellido);
                comando.Parameters.AddWithValue("@nombre", usuario.Nombre);
                comando.Parameters.AddWithValue("@rol", usuario.Rol);
                comando.Parameters.AddWithValue("@activo", usuario.Activo);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        // Si Password viene vacia, se conserva la contraseña actual.
        public static void ModificarUsuario(Usuario usuario)
        {
            bool cambiaPassword = !string.IsNullOrEmpty(usuario.Password);

            string consulta = "UPDATE Usuario SET Apellido = @apellido, Nombre = @nombre, " +
                              "Rol = @rol, Activo = @activo" +
                              (cambiaPassword ? ", Password = @password" : "") +
                              " WHERE IdUsuario = @id";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@apellido", usuario.Apellido);
                comando.Parameters.AddWithValue("@nombre", usuario.Nombre);
                comando.Parameters.AddWithValue("@rol", usuario.Rol);
                comando.Parameters.AddWithValue("@activo", usuario.Activo);
                comando.Parameters.AddWithValue("@id", usuario.IdUsuario);
                if (cambiaPassword)
                    comando.Parameters.AddWithValue("@password", CalcularHash(usuario.Password));
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void EliminarUsuario(int idUsuario)
        {
            string consulta = "DELETE FROM Usuario WHERE IdUsuario = @id";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@id", idUsuario);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static Usuario LeerUsuario(SqlDataReader lector)
        {
            Usuario u = new Usuario();
            u.IdUsuario = (int)lector["IdUsuario"];
            u.NombreUsuario = lector["NombreUsuario"].ToString();
            u.Apellido = lector["Apellido"].ToString();
            u.Nombre = lector["Nombre"].ToString();
            u.Rol = lector["Rol"].ToString();
            u.Activo = (bool)lector["Activo"];
            return u;
        }
    }
}

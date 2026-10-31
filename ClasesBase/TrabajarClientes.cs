using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarClientes
    {
        private static string ObtenerCadenaConexion()
        {
            return ConfigurationManager.ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        public static ObservableCollection<Cliente> TraerClientes()
        {
            ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();
            string cadena = ObtenerCadenaConexion();
            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand("SELECT DNI, Apellido, Nombre, Direccion FROM Cliente", conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            DNI = lector["DNI"].ToString(),
                            Apellido = lector["Apellido"].ToString(),
                            Nombre = lector["Nombre"].ToString(),
                            Direccion = lector["Direccion"].ToString()
                        });
                    }
                }
            }
            return clientes;
        }

        public static bool ExisteDNI(string dni)
        {
            string cadena = ObtenerCadenaConexion();
            string consulta = "SELECT COUNT(*) FROM Cliente WHERE DNI = @DNI";

            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@DNI", dni);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public static void AgregarCliente(Cliente cliente)
        {
            string cadena = ObtenerCadenaConexion();
            string consulta = "INSERT INTO Cliente (DNI, Apellido, Nombre, Direccion) " +
                              "VALUES (@DNI, @Apellido, @Nombre, @Direccion)";

            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@DNI", cliente.DNI);
                comando.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                comando.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                comando.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void ModificarCliente(Cliente cliente, string dniOriginal)
        {
            string cadena = ObtenerCadenaConexion();
            string consulta = "UPDATE Cliente SET DNI = @DNI, Apellido = @Apellido, " +
                              "Nombre = @Nombre, Direccion = @Direccion " +
                              "WHERE DNI = @DNIOriginal";

            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@DNI", cliente.DNI);
                comando.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                comando.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                comando.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                comando.Parameters.AddWithValue("@DNIOriginal", dniOriginal);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void EliminarCliente(string dni)
        {
            string cadena = ObtenerCadenaConexion();
            string consulta = "DELETE FROM Cliente WHERE DNI = @DNI";

            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@DNI", dni);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}

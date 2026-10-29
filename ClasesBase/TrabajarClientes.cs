using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarClientes
    {
        public ObservableCollection<Cliente> TraerClientes()
        {
            ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();
            string cadena = ConfigurationManager.ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
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
                            Dirección = lector["Direccion"].ToString()
                        });
                    }
                }
            }
            return clientes;
        }
    }
}

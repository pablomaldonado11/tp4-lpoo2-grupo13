using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarVendedores
    {
        private readonly string cadenaConexion;

        public TrabajarVendedores()
        {
            cadenaConexion = ConfigurationManager
                .ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        public DataTable TraerVendedores()
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "SELECT Legajo, Apellido, Nombre " +
                                  "FROM Vendedor ORDER BY Legajo";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(tabla);
            }

            return tabla;
        }

        public void AgregarVendedor(Vendedor vendedor)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "INSERT INTO Vendedor (Legajo, Apellido, Nombre) " +
                                  "VALUES (@legajo, @apellido, @nombre)";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                CargarParametros(comando, vendedor);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void ModificarVendedor(Vendedor vendedor)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "UPDATE Vendedor SET Apellido=@apellido, Nombre=@nombre " +
                                  "WHERE Legajo=@legajo";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                CargarParametros(comando, vendedor);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void EliminarVendedor(string legajo)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "DELETE FROM Vendedor WHERE Legajo=@legajo";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@legajo", legajo);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private void CargarParametros(SqlCommand comando, Vendedor vendedor)
        {
            comando.Parameters.AddWithValue("@legajo", vendedor.Legajo);
            comando.Parameters.AddWithValue("@apellido", vendedor.Apellido);
            comando.Parameters.AddWithValue("@nombre", vendedor.Nombre);
        }
    }
}

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarProductos
    {
        private readonly string cadenaConexion;

        public TrabajarProductos()
        {
            cadenaConexion = ConfigurationManager
                .ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        public DataTable TraerProductos()
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "SELECT CodProducto, Categoria, Color, Descripcion, Precio " +
                                  "FROM Producto ORDER BY CodProducto";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(tabla);
            }

            return tabla;
        }

        public void AgregarProducto(Producto producto)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "INSERT INTO Producto " +
                    "(CodProducto, Categoria, Color, Descripcion, Precio) " +
                    "VALUES (@codigo, @categoria, @color, @descripcion, @precio)";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                CargarParametros(comando, producto);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void ModificarProducto(Producto producto)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "UPDATE Producto SET Categoria=@categoria, " +
                    "Color=@color, Descripcion=@descripcion, Precio=@precio " +
                    "WHERE CodProducto=@codigo";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                CargarParametros(comando, producto);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void EliminarProducto(string codigo)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "DELETE FROM Producto WHERE CodProducto=@codigo";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@codigo", codigo);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private void CargarParametros(SqlCommand comando, Producto producto)
        {
            comando.Parameters.AddWithValue("@codigo", producto.CodProducto);
            comando.Parameters.AddWithValue("@categoria", producto.Categoria);
            comando.Parameters.AddWithValue("@color", producto.Color);
            comando.Parameters.AddWithValue("@descripcion", producto.Descripcion);
            comando.Parameters.AddWithValue("@precio", producto.ObtenerPrecioNumerico());
        }
    }
}

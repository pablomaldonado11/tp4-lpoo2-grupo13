using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarVentas
    {
        private readonly string cadenaConexion;

        public TrabajarVentas()
        {
            cadenaConexion = ConfigurationManager
                .ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        public int ProximoNroVenta()
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                SqlCommand comando = new SqlCommand(
                    "SELECT ISNULL(MAX(NroFactura), 0) + 1 FROM Venta", conexion);
                conexion.Open();
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public void AgregarVenta(Venta venta)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string consulta = "INSERT INTO Venta " +
                    "(NroFactura, FechaFactura, Legajo, DNI, CodProducto, Precio, Cantidad, Importe) " +
                    "VALUES (@nro, @fecha, @legajo, @dni, @codigo, @precio, @cantidad, @importe)";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@nro", venta.NroFactura);
                comando.Parameters.AddWithValue("@fecha", venta.FechaFactura);
                comando.Parameters.AddWithValue("@legajo", venta.Legajo);
                comando.Parameters.AddWithValue("@dni", venta.DNI);
                comando.Parameters.AddWithValue("@codigo", venta.CodProducto);
                comando.Parameters.AddWithValue("@precio", venta.Precio);
                comando.Parameters.AddWithValue("@cantidad", venta.Cantidad);
                comando.Parameters.AddWithValue("@importe", venta.Importe);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
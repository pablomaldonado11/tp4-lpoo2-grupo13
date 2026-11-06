using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ClasesBase
{
    public class TrabajarVentas
    {
        private static string ObtenerCadenaConexion()
        {
            return ConfigurationManager.ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        // Arma el WHERE según los filtros que no sean nulos / vacíos
        private static string ArmarWhere(SqlCommand comando, DateTime? desde, DateTime? hasta, string legajo)
        {
            StringBuilder where = new StringBuilder();

            if (desde.HasValue)
            {
                where.Append(where.Length == 0 ? " WHERE " : " AND ");
                where.Append("v.FechaFactura >= @desde");
                comando.Parameters.AddWithValue("@desde", desde.Value.Date);
            }
            if (hasta.HasValue)
            {
                where.Append(where.Length == 0 ? " WHERE " : " AND ");
                where.Append("v.FechaFactura < @hasta");
                comando.Parameters.AddWithValue("@hasta", hasta.Value.Date.AddDays(1));   // el día "hasta" entra completo
            }
            if (!string.IsNullOrEmpty(legajo))
            {
                where.Append(where.Length == 0 ? " WHERE " : " AND ");
                where.Append("RTRIM(v.Legajo) = @legajo");
                comando.Parameters.AddWithValue("@legajo", legajo.Trim());
            }
            return where.ToString();
        }

        // Listado de ventas con los nombres de cliente, vendedor y producto
        public static DataTable TraerVentas(DateTime? desde, DateTime? hasta, string legajo)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand())
            {
                comando.Connection = conexion;
                comando.CommandText =
                    "SELECT v.NroFactura AS NroFactura, v.FechaFactura AS Fecha, " +
                    "RTRIM(v.Legajo) AS Legajo, RTRIM(ve.Apellido) + ', ' + RTRIM(ve.Nombre) AS Vendedor, " +
                    "RTRIM(v.DNI) AS DNI, RTRIM(c.Apellido) + ', ' + RTRIM(c.Nombre) AS Cliente, " +
                    "RTRIM(v.CodProducto) AS CodProducto, RTRIM(p.Descripcion) AS Producto, " +
                    "v.Precio AS Precio, v.Cantidad AS Cantidad, v.Importe AS Importe " +
                    "FROM Venta v " +
                    "LEFT JOIN Vendedor ve ON ve.Legajo = v.Legajo " +
                    "LEFT JOIN Cliente c ON c.DNI = v.DNI " +
                    "LEFT JOIN Producto p ON p.CodProducto = v.CodProducto" +
                    ArmarWhere(comando, desde, hasta, legajo) +
                    " ORDER BY v.NroFactura DESC";

                new SqlDataAdapter(comando).Fill(tabla);
            }
            return tabla;
        }

        // Próximo número de factura (máximo actual + 1; NroFactura no es identity)
        public static int ProximoNroFactura()
        {
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand("SELECT ISNULL(MAX(NroFactura), 0) + 1 FROM Venta", conexion))
            {
                conexion.Open();
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public static void AgregarVenta(Venta v)
        {
            // El número se calcula acá para que nunca se repita
            v.NroFactura = ProximoNroFactura();
            v.Importe = v.Precio * v.Cantidad;

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(
                "INSERT INTO Venta (NroFactura, FechaFactura, Legajo, DNI, CodProducto, Precio, Cantidad, Importe) " +
                "VALUES (@nro, @fecha, @legajo, @dni, @cod, @precio, @cantidad, @importe)", conexion))
            {
                comando.Parameters.AddWithValue("@nro", v.NroFactura);
                comando.Parameters.AddWithValue("@fecha", v.FechaFactura);
                comando.Parameters.AddWithValue("@legajo", v.Legajo);
                comando.Parameters.AddWithValue("@dni", v.DNI);
                comando.Parameters.AddWithValue("@cod", v.CodProducto);
                comando.Parameters.AddWithValue("@precio", v.Precio);
                comando.Parameters.AddWithValue("@cantidad", v.Cantidad);
                comando.Parameters.AddWithValue("@importe", v.Importe);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void EliminarVenta(int nroFactura)
        {
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand("DELETE FROM Venta WHERE NroFactura = @nro", conexion))
            {
                comando.Parameters.AddWithValue("@nro", nroFactura);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        // Total vendido en el período / vendedor indicado
        public static decimal TotalVentas(DateTime? desde, DateTime? hasta, string legajo)
        {
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand())
            {
                comando.Connection = conexion;
                comando.CommandText = "SELECT ISNULL(SUM(v.Importe), 0) FROM Venta v" +
                                      ArmarWhere(comando, desde, hasta, legajo);
                conexion.Open();
                return Convert.ToDecimal(comando.ExecuteScalar());
            }
        }

        // Resumen: cantidad de ventas e importe total por vendedor
        public static DataTable ResumenPorVendedor(DateTime? desde, DateTime? hasta)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand())
            {
                comando.Connection = conexion;
                comando.CommandText =
                    "SELECT RTRIM(v.Legajo) AS Legajo, RTRIM(ve.Apellido) + ', ' + RTRIM(ve.Nombre) AS Vendedor, " +
                    "COUNT(*) AS Ventas, SUM(v.Importe) AS Total " +
                    "FROM Venta v LEFT JOIN Vendedor ve ON ve.Legajo = v.Legajo" +
                    ArmarWhere(comando, desde, hasta, null) +
                    " GROUP BY RTRIM(v.Legajo), RTRIM(ve.Apellido), RTRIM(ve.Nombre) ORDER BY SUM(v.Importe) DESC";
                new SqlDataAdapter(comando).Fill(tabla);
            }
            return tabla;
        }

        // Resumen: unidades vendidas e importe total por producto
        public static DataTable ResumenPorProducto(DateTime? desde, DateTime? hasta)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand())
            {
                comando.Connection = conexion;
                comando.CommandText =
                    "SELECT RTRIM(v.CodProducto) AS CodProducto, RTRIM(p.Descripcion) AS Producto, " +
                    "SUM(v.Cantidad) AS Unidades, SUM(v.Importe) AS Total " +
                    "FROM Venta v LEFT JOIN Producto p ON p.CodProducto = v.CodProducto" +
                    ArmarWhere(comando, desde, hasta, null) +
                    " GROUP BY RTRIM(v.CodProducto), RTRIM(p.Descripcion) ORDER BY SUM(v.Importe) DESC";
                new SqlDataAdapter(comando).Fill(tabla);
            }
            return tabla;
        }
    }
}

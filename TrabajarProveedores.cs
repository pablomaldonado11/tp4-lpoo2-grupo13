using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarProveedores
    {
        private static string ObtenerCadenaConexion()
        {
            return ConfigurationManager.ConnectionStrings["MuebleriaConnectionString"].ConnectionString;
        }

        // La tabla Proveedor no venía en muebleria.mdf: se crea sola si falta
        // (mismo esquema que BaseDatos\CrearTablas.sql)
        public static void AsegurarTabla()
        {
            string consulta =
                "IF OBJECT_ID('Proveedor', 'U') IS NULL " +
                "CREATE TABLE Proveedor (" +
                "CUIT VARCHAR(11) NOT NULL PRIMARY KEY, " +
                "RazonSocial VARCHAR(80) NOT NULL, " +
                "Domicilio VARCHAR(100) NOT NULL, " +
                "Telefono VARCHAR(15) NOT NULL)";

            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static List<Proveedor> TraerProveedores()
        {
            AsegurarTabla();

            List<Proveedor> lista = new List<Proveedor>();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(
                "SELECT CUIT, RazonSocial, Domicilio, Telefono FROM Proveedor ORDER BY RazonSocial", conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Proveedor p = new Proveedor();
                        p.CUIT = lector["CUIT"].ToString().Trim();
                        p.RazonSocial = lector["RazonSocial"].ToString().Trim();
                        p.Domicilio = lector["Domicilio"].ToString().Trim();
                        p.Telefono = lector["Telefono"].ToString().Trim();
                        lista.Add(p);
                    }
                }
            }
            return lista;
        }

        public static bool ExisteCUIT(string cuit)
        {
            AsegurarTabla();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand("SELECT COUNT(*) FROM Proveedor WHERE CUIT = @cuit", conexion))
            {
                comando.Parameters.AddWithValue("@cuit", cuit);
                conexion.Open();
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        public static void AgregarProveedor(Proveedor p)
        {
            AsegurarTabla();
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(
                "INSERT INTO Proveedor (CUIT, RazonSocial, Domicilio, Telefono) " +
                "VALUES (@cuit, @razon, @domicilio, @telefono)", conexion))
            {
                CargarParametros(comando, p);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void ModificarProveedor(Proveedor p)
        {
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand(
                "UPDATE Proveedor SET RazonSocial = @razon, Domicilio = @domicilio, Telefono = @telefono " +
                "WHERE CUIT = @cuit", conexion))
            {
                CargarParametros(comando, p);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public static void EliminarProveedor(string cuit)
        {
            using (SqlConnection conexion = new SqlConnection(ObtenerCadenaConexion()))
            using (SqlCommand comando = new SqlCommand("DELETE FROM Proveedor WHERE CUIT = @cuit", conexion))
            {
                comando.Parameters.AddWithValue("@cuit", cuit);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static void CargarParametros(SqlCommand comando, Proveedor p)
        {
            comando.Parameters.AddWithValue("@cuit", p.CUIT);
            comando.Parameters.AddWithValue("@razon", p.RazonSocial);
            comando.Parameters.AddWithValue("@domicilio", p.Domicilio);
            comando.Parameters.AddWithValue("@telefono", p.Telefono);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ClasesBase
{
    public class TrabajarClientes
    {
        public static ObservableCollection<Cliente> TraerClientes()
        {
            ObservableCollection<Cliente> lista = new ObservableCollection<Cliente>();

            string cadena = ConfigurationManager
                .ConnectionStrings["MuebleriaConnectionString"].ConnectionString;

            using (SqlConnection cnn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Cliente", cnn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow fila in dt.Rows)
                {
                    Cliente c = new Cliente();
                    c.DNI = fila["DNI"].ToString();
                    c.Apellido = fila["Apellido"].ToString();
                    c.Nombre = fila["Nombre"].ToString();
                    c.Direccion = fila["Direccion"].ToString();
                    lista.Add(c);
                }
            }
            return lista;
        }
    }
}
using System;
using System.Data;
using System.IO;
using System.Text;

namespace ClasesBase
{
    public class Utilidades
    {
        // Escapa los caracteres especiales para usarlos dentro de un RowFilter con LIKE
        public static string EscapeLike(string texto)
        {
            if (texto == null) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in texto)
            {
                if (c == '[' || c == ']' || c == '%' || c == '*') sb.Append("[" + c + "]");
                else if (c == '\'') sb.Append("''");
                else sb.Append(c);
            }
            return sb.ToString();
        }

        // Exporta lo que muestra un DataView (respeta filtros) a un archivo CSV separado por ';'
        public static void ExportarCsv(DataView datos, string ruta)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < datos.Table.Columns.Count; i++)
            {
                if (i > 0) sb.Append(";");
                sb.Append(datos.Table.Columns[i].ColumnName);
            }
            sb.AppendLine();

            foreach (DataRowView fila in datos)
            {
                for (int i = 0; i < datos.Table.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(";");
                    string valor = Convert.ToString(fila[i]);
                    valor = valor.Replace("\"", "\"\"");
                    if (valor.Contains(";") || valor.Contains("\"") || valor.Contains("\n"))
                        valor = "\"" + valor + "\"";
                    sb.Append(valor);
                }
                sb.AppendLine();
            }

            File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
        }
    }
}

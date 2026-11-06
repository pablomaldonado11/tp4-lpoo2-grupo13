using System;
using System.ComponentModel;
using System.Globalization;

namespace ClasesBase
{
    public class Producto : IDataErrorInfo
    {
        public string CodProducto { get; set; }
        public string Categoria { get; set; }
        public string Color { get; set; }
        public string Descripcion { get; set; }
        public string Precio { get; set; }

        public string Error
        {
            get
            {
                string mensaje;

                mensaje = this["CodProducto"];
                if (mensaje != "") return mensaje;

                mensaje = this["Categoria"];
                if (mensaje != "") return mensaje;

                mensaje = this["Color"];
                if (mensaje != "") return mensaje;

                mensaje = this["Descripcion"];
                if (mensaje != "") return mensaje;

                return this["Precio"];
            }
        }

        public string this[string nombrePropiedad]
        {
            get
            {
                if (nombrePropiedad == "CodProducto" &&
                    String.IsNullOrWhiteSpace(CodProducto))
                    return "El código es obligatorio.";

                if (nombrePropiedad == "Categoria" &&
                    String.IsNullOrWhiteSpace(Categoria))
                    return "La categoría es obligatoria.";

                if (nombrePropiedad == "Color" &&
                    String.IsNullOrWhiteSpace(Color))
                    return "El color es obligatorio.";

                if (nombrePropiedad == "Descripcion" &&
                    String.IsNullOrWhiteSpace(Descripcion))
                    return "La descripción es obligatoria.";

                if (nombrePropiedad == "Precio")
                {
                    if (String.IsNullOrWhiteSpace(Precio))
                        return "El precio es obligatorio.";

                    decimal precioNumerico;
                    if (!IntentarConvertirPrecio(out precioNumerico))
                        return "El precio debe ser numérico.";

                    if (precioNumerico <= 0)
                        return "El precio debe ser mayor que cero.";
                }

                return "";
            }
        }

        public decimal ObtenerPrecioNumerico()
        {
            decimal precioNumerico;
            IntentarConvertirPrecio(out precioNumerico);
            return precioNumerico;
        }

        private bool IntentarConvertirPrecio(out decimal resultado)
        {
            string texto = Precio == null ? "" : Precio.Trim().Replace(',', '.');
            return Decimal.TryParse(texto, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out resultado);
        }
    }
}

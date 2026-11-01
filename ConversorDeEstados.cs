using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vistas
{
    public class ConversorDeEstados : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            string estado = value == null ? "" : value.ToString().ToUpper();

            if (estado == "PENDIENTE") return Brushes.Red;
            if (estado == "PAGADA") return Brushes.Green;
            if (estado == "CONTABILIZADA") return Brushes.Blue;
            if (estado == "ANULADA") return Brushes.Gray;

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using ClasesBase;

namespace Vistas
{
    public partial class ComprobanteWindow : Window
    {
        public ComprobanteWindow(Venta venta, Cliente cliente,
                                 DataRowView producto, DataRowView vendedor)
        {
            InitializeComponent();
            visor.Document = CrearDocumento(venta, cliente, producto, vendedor);
        }

        private FixedDocument CrearDocumento(Venta venta, Cliente cliente,
                                             DataRowView producto, DataRowView vendedor)
        {
            CultureInfo cultura = new CultureInfo("es-AR");

            FixedPage pagina = new FixedPage();
            pagina.Width = 816;
            pagina.Height = 1056;

            // Encabezado del comercio
            Texto(pagina, "El Roble", 60, 50, 28, true);
            Texto(pagina, "-Mueblería-", 60, 88, 16, true);
            Texto(pagina, "Av. Éxodo 345     CUIT: 30-12345678-6", 60, 112, 12, false);
            Texto(pagina, "San Pedro de Jujuy     IIBB: A-999", 60, 132, 12, false);

            // Datos de la venta
            Texto(pagina, "Venta", 460, 50, 28, true);
            Texto(pagina, "Número: " + venta.NroFactura, 460, 95, 13, false);
            Texto(pagina, "Fecha y Hora: " + venta.FechaFactura.ToString("dd/MM/yyyy") +
                          " " + DateTime.Now.ToString("HH:mm"), 460, 125, 13, false);

            // Cliente
            Texto(pagina, "Cliente", 80, 230, 16, true);
            Texto(pagina, "Apellido: " + cliente.Apellido, 80, 265, 13, false);
            Texto(pagina, "Nombre: " + cliente.Nombre, 80, 292, 13, false);
            Texto(pagina, "DNI: " + cliente.DNI, 80, 319, 13, false);

            // Producto
            Texto(pagina, "Producto", 460, 230, 16, true);
            Texto(pagina, "Descripción: " + producto["Descripcion"], 460, 265, 13, false);
            Texto(pagina, "Color: " + producto["Color"], 460, 292, 13, false);
            Texto(pagina, "Precio: " + venta.Precio.ToString("C2", cultura), 460, 319, 13, false);
            Texto(pagina, "Cantidad: " + venta.Cantidad, 460, 346, 13, false);

            // Vendedor y total
            Texto(pagina, "Vendedor: " + vendedor["Nombre"] + " " + vendedor["Apellido"],
                  80, 400, 14, true);
            Texto(pagina, "Total: " + venta.Importe.ToString("C2", cultura), 460, 395, 20, true);

            PageContent contenido = new PageContent();
            ((IAddChild)contenido).AddChild(pagina);

            FixedDocument documento = new FixedDocument();
            documento.DocumentPaginator.PageSize = new Size(816, 1056);
            documento.Pages.Add(contenido);
            return documento;
        }

        private void Texto(FixedPage pagina, string texto, double x, double y,
                           double tamano, bool negrita)
        {
            TextBlock bloque = new TextBlock();
            bloque.Text = texto;
            bloque.FontSize = tamano;
            bloque.FontFamily = new FontFamily("Segoe UI");
            if (negrita)
                bloque.FontWeight = FontWeights.Bold;

            FixedPage.SetLeft(bloque, x);
            FixedPage.SetTop(bloque, y);
            pagina.Children.Add(bloque);
        }
    }
}
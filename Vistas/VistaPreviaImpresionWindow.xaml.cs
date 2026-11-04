using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClasesBase;

namespace Vistas
{
    public partial class VistaPreviaImpresionWindow : Window
    {
        public VistaPreviaImpresionWindow()
        {
            InitializeComponent();
            try
            {
                CargarDocumento(TrabajarClientes.TraerClientes());
            }
            catch
            {
                CargarDocumento(ObtenerClientesMuestra());
            }
        }

        public VistaPreviaImpresionWindow(IEnumerable<Cliente> clientes)
        {
            InitializeComponent();
            if (clientes != null)
            {
                CargarDocumento(clientes);
            }
            else
            {
                try
                {
                    CargarDocumento(TrabajarClientes.TraerClientes());
                }
                catch
                {
                    CargarDocumento(ObtenerClientesMuestra());
                }
            }
        }

        private static List<Cliente> ObtenerClientesMuestra()
        {
            return new List<Cliente>
            {
                new Cliente { DNI = "1111", Apellido = "Perez", Nombre = "Juan", Direccion = "Belgrano 761 - SSJ" },
                new Cliente { DNI = "222", Apellido = "Avila", Nombre = "Luis", Direccion = "El chañi 325 - SSJ" },
                new Cliente { DNI = "333", Apellido = "Rodriguez", Nombre = "Laura", Direccion = "Victoria 729 - SSJ" }
            };
        }

        public void CargarDocumento(IEnumerable<Cliente> clientes)
        {
            flowDoc.Blocks.Clear();

            Table tabla = new Table();
            tabla.CellSpacing = 0;
            tabla.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 160, 160));
            tabla.BorderThickness = new Thickness(1);
            tabla.FontFamily = new FontFamily("Georgia");
            tabla.FontSize = 14;

            tabla.Columns.Add(new TableColumn { Width = new GridLength(90) });
            tabla.Columns.Add(new TableColumn { Width = new GridLength(140) });
            tabla.Columns.Add(new TableColumn { Width = new GridLength(140) });
            tabla.Columns.Add(new TableColumn { Width = new GridLength(220) });

            TableRowGroup group = new TableRowGroup();

            // Encabezado de la tabla según figura del TP5
            TableRow filaHeader = new TableRow();
            filaHeader.Cells.Add(CrearCeldaHeader("DNI", true));
            filaHeader.Cells.Add(CrearCeldaHeader("Apellido", false));
            filaHeader.Cells.Add(CrearCeldaHeader("Nombre", false));
            filaHeader.Cells.Add(CrearCeldaHeader("Dirección", false));
            group.Rows.Add(filaHeader);

            // Filas de datos
            if (clientes != null)
            {
                foreach (Cliente cliente in clientes)
                {
                    TableRow filaDato = new TableRow();
                    filaDato.Cells.Add(CrearCeldaDato(cliente.DNI));
                    filaDato.Cells.Add(CrearCeldaDato(cliente.Apellido));
                    filaDato.Cells.Add(CrearCeldaDato(cliente.Nombre));
                    filaDato.Cells.Add(CrearCeldaDato(cliente.Direccion));
                    group.Rows.Add(filaDato);
                }
            }

            tabla.RowGroups.Add(group);
            flowDoc.Blocks.Add(tabla);
        }

        private TableCell CrearCeldaHeader(string texto, bool resaltado)
        {
            Paragraph p = new Paragraph(new Run(texto));
            p.Margin = new Thickness(0);
            p.FontSize = 14;

            TableCell celda = new TableCell(p);
            celda.Padding = new Thickness(8, 4, 8, 4);
            celda.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            celda.BorderThickness = new Thickness(0, 0, 1, 1);

            if (resaltado)
            {
                celda.Background = new SolidColorBrush(Color.FromRgb(204, 228, 247));
            }

            return celda;
        }

        private TableCell CrearCeldaDato(string texto)
        {
            Paragraph p = new Paragraph(new Run(texto ?? ""));
            p.Margin = new Thickness(0);
            p.FontSize = 13;

            TableCell celda = new TableCell(p);
            celda.Padding = new Thickness(8, 4, 8, 4);
            celda.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            celda.BorderThickness = new Thickness(0, 0, 0, 0);

            return celda;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Nota: Corresponde a la consigna del punto 1b.
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource paginador = flowDoc as IDocumentPaginatorSource;
                if (paginador != null)
                {
                    printDialog.PrintDocument(paginador.DocumentPaginator, "Listado de Clientes");
                }
            }
        }
    }
}

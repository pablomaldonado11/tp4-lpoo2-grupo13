using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ClasesBase; // Referencia a tu proyecto de clases

namespace Vistas
{
    public partial class ListadoClientesWindow : Window
    {
        public ListadoClientesWindow()
        {
            InitializeComponent();
            CargarYOrdenarClientes();
        }

        private void CargarYOrdenarClientes()
        {
            // 1. Obtenemos la colección de clientes (acá uso datos de prueba para que puedas ver la grilla funcionando)
            ObservableCollection<Cliente> listaClientes = ObtenerClientesDePrueba();

            // 2. Ordenamos por Apellido en forma ascendente usando LINQ
            var clientesOrdenados = listaClientes.OrderBy(c => c.Apellido).ToList();

            // 3. Asignamos la lista ya ordenada a la grilla
            dgClientes.ItemsSource = clientesOrdenados;
        }

        // Este método simula los datos hasta que tu grupo conecte la base de datos real
        private ObservableCollection<Cliente> ObtenerClientesDePrueba()
        {
            ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();

            clientes.Add(new Cliente { DNI = "33333333", Apellido = "Zárate", Nombre = "Luis", Dirección = "Calle 1" });
            clientes.Add(new Cliente { DNI = "11111111", Apellido = "Alvarez", Nombre = "María", Dirección = "Calle 2" });
            clientes.Add(new Cliente { DNI = "22222222", Apellido = "Gómez", Nombre = "Carlos", Dirección = "Calle 3" });

            return clientes;
        }
    }
}
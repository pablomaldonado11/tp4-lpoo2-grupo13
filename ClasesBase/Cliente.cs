using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClasesBase
{
    public class Cliente : INotifyPropertyChanged
    {
        private string dni;
        private string apellido;
        private string nombre;
        private string dirección;

        public event PropertyChangedEventHandler PropertyChanged;

        // Método tradicional sin dependencias de versiones nuevas de C#
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string DNI
        {
            get { return dni; }
            set { dni = value; OnPropertyChanged("DNI"); }
        }

        public string Apellido
        {
            get { return apellido; }
            set { apellido = value; OnPropertyChanged("Apellido"); }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }

        public string Dirección
        {
            get { return dirección; }
            set { dirección = value; OnPropertyChanged("Dirección"); }
        }
    }
}
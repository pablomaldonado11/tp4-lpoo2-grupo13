using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Vistas
{
    public partial class PresentacionWindow : Window
    {
        private MediaPlayer reproductor;
        private DispatcherTimer temporizador;

        public PresentacionWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                       "audio", "intro.mp3");

            if (File.Exists(ruta))
            {
                reproductor = new MediaPlayer();
                reproductor.Open(new Uri(ruta, UriKind.Absolute));
                reproductor.Play();
            }

            temporizador = new DispatcherTimer();
            temporizador.Interval = TimeSpan.FromSeconds(5);
            temporizador.Tick += temporizador_Tick;
            temporizador.Start();
        }

        private void temporizador_Tick(object sender, EventArgs e)
        {
            Continuar();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            Continuar();
        }

        private void Continuar()
        {
            temporizador.Stop();

            Login ventana = new Login();
            ventana.Show();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (reproductor != null)
            {
                reproductor.Stop();
                reproductor.Close();
            }
        }
    }
}
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Vistas
{
    /// <summary>
    /// Logica de interaccion para AcercaDeWindow.xaml
    /// </summary>
    public partial class AcercaDeWindow : Window
    {
        private readonly DispatcherTimer timer;
        private bool isUserSeeking = false;
        private bool wasPlayingBeforeSeek = false;
        private bool isUpdatingFromTimer = false;
        private bool isPlaying = false;

        public AcercaDeWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;

            Loaded += AcercaDeWindow_Loaded;
            Closing += AcercaDeWindow_Closing;
        }

        private void AcercaDeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Intentar cargar el video predeterminado del grupo
            string asmDir = null;
            try
            {
                asmDir = Path.GetDirectoryName(typeof(AcercaDeWindow).Assembly.Location);
            }
            catch
            {
                // Ignorar si no se puede obtener Location
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] candidatePaths = new string[]
            {
                !string.IsNullOrEmpty(asmDir) ? Path.Combine(asmDir, "videos", "grupo13.wmv") : null,
                !string.IsNullOrEmpty(baseDir) ? Path.Combine(baseDir, "videos", "grupo13.wmv") : null,
                !string.IsNullOrEmpty(asmDir) ? Path.Combine(asmDir, "..", "..", "videos", "grupo13.wmv") : null,
                !string.IsNullOrEmpty(baseDir) ? Path.Combine(baseDir, "..", "..", "videos", "grupo13.wmv") : null
            };

            string videoPath = null;
            foreach (string path in candidatePaths)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    videoPath = Path.GetFullPath(path);
                    break;
                }
            }

            if (videoPath != null)
            {
                CargarVideo(videoPath);
            }
            else
            {
                lblEstadoVideo.Text = "Video predeterminado no encontrado. Utilice 'Cargar video...' para seleccionar uno.";
            }
        }

        private void CargarVideo(string ruta)
        {
            try
            {
                DetenerReproduccion();

                meReproductor.Source = new Uri(ruta, UriKind.Absolute);
                lblEstadoVideo.Text = "Video cargado: " + Path.GetFileName(ruta) + " (" + ruta + ")";
                isUpdatingFromTimer = true;
                sliderProgreso.Value = 0;
                isUpdatingFromTimer = false;
                lblTiempo.Text = "00:00 / 00:00";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el video: " + ex.Message, "Error de Video",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AcercaDeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DetenerReproduccion();
            meReproductor.Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (meReproductor.NaturalDuration.HasTimeSpan && !isUserSeeking)
            {
                isUpdatingFromTimer = true;
                sliderProgreso.Value = meReproductor.Position.TotalSeconds;
                isUpdatingFromTimer = false;

                ActualizarEtiquetaTiempo(meReproductor.Position, meReproductor.NaturalDuration.TimeSpan);
            }
        }

        private void ActualizarEtiquetaTiempo(TimeSpan posicion, TimeSpan total)
        {
            string formato = total.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            lblTiempo.Text = string.Format("{0} / {1}", posicion.ToString(formato), total.ToString(formato));
        }

        private void meReproductor_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (meReproductor.NaturalDuration.HasTimeSpan)
            {
                TimeSpan total = meReproductor.NaturalDuration.TimeSpan;
                sliderProgreso.Minimum = 0;
                sliderProgreso.Maximum = total.TotalSeconds;
                isUpdatingFromTimer = true;
                sliderProgreso.Value = 0;
                isUpdatingFromTimer = false;
                ActualizarEtiquetaTiempo(TimeSpan.Zero, total);
            }
        }

        private void meReproductor_MediaEnded(object sender, RoutedEventArgs e)
        {
            DetenerReproduccion();
        }

        private void meReproductor_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            DetenerReproduccion();
            string msg = (e.ErrorException != null && !string.IsNullOrEmpty(e.ErrorException.Message))
                ? e.ErrorException.Message
                : "Formato no compatible o error en el reproductor de video.";

            MessageBox.Show("Error al reproducir el video: " + msg,
                "Error de Reproduccion", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (meReproductor.Source == null)
            {
                MessageBox.Show("Por favor seleccione un archivo de video primero.",
                    "Sin Video", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Si el video está al final (o terminó), reiniciar desde el principio
            if (meReproductor.NaturalDuration.HasTimeSpan &&
                meReproductor.Position >= meReproductor.NaturalDuration.TimeSpan)
            {
                meReproductor.Position = TimeSpan.Zero;
                isUpdatingFromTimer = true;
                sliderProgreso.Value = 0;
                isUpdatingFromTimer = false;
            }

            meReproductor.Play();
            timer.Start();
            isPlaying = true;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                meReproductor.Pause();
                timer.Stop();
                isPlaying = false;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            DetenerReproduccion();
        }

        private void DetenerReproduccion()
        {
            meReproductor.Stop();
            timer.Stop();
            isPlaying = false;
            isUpdatingFromTimer = true;
            sliderProgreso.Value = 0;
            isUpdatingFromTimer = false;

            if (meReproductor.NaturalDuration.HasTimeSpan)
            {
                ActualizarEtiquetaTiempo(TimeSpan.Zero, meReproductor.NaturalDuration.TimeSpan);
            }
            else
            {
                lblTiempo.Text = "00:00 / 00:00";
            }
        }

        private void IniciarSeeking()
        {
            if (!isUserSeeking)
            {
                isUserSeeking = true;
                wasPlayingBeforeSeek = isPlaying;
                if (isPlaying)
                {
                    meReproductor.Pause();
                    timer.Stop();
                }
            }
        }

        private void FinalizarSeeking()
        {
            if (isUserSeeking)
            {
                isUserSeeking = false;
                meReproductor.Position = TimeSpan.FromSeconds(sliderProgreso.Value);

                if (wasPlayingBeforeSeek)
                {
                    meReproductor.Play();
                    timer.Start();
                    isPlaying = true;
                }
            }
        }

        private void sliderProgreso_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IniciarSeeking();
        }

        private void sliderProgreso_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FinalizarSeeking();
        }

        private void sliderProgreso_DragStarted(object sender, DragStartedEventArgs e)
        {
            IniciarSeeking();
        }

        private void sliderProgreso_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            FinalizarSeeking();
        }

        private void sliderProgreso_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isUpdatingFromTimer)
            {
                if (!isUserSeeking)
                {
                    meReproductor.Position = TimeSpan.FromSeconds(sliderProgreso.Value);
                }

                if (meReproductor.NaturalDuration.HasTimeSpan)
                {
                    ActualizarEtiquetaTiempo(TimeSpan.FromSeconds(sliderProgreso.Value), meReproductor.NaturalDuration.TimeSpan);
                }
            }
        }

        private void sliderVolumen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (meReproductor != null)
            {
                meReproductor.Volume = sliderVolumen.Value;
            }
        }

        private void btnCargarVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Seleccionar video para reproducir";
            ofd.Filter = "Archivos de video (*.wmv;*.mp4;*.avi;*.mov;*.mkv)|*.wmv;*.mp4;*.avi;*.mov;*.mkv|Todos los archivos (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                CargarVideo(ofd.FileName);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

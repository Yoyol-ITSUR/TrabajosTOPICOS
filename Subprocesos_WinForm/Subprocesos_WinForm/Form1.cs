using System.ComponentModel;
using System.Media;

namespace Subprocesos_WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Construir la ruta completa al archivo .wav.
            // Application.StartupPath obtiene la ruta del directorio donde se inició la aplicación.
            // Path.Combine es seguro para construir rutas de archivo en diferentes sistemas operativos.
            // También aplica para la imagen a cargar
            filePathToSound = Path.Combine(Application.StartupPath, "tada.wav");
            filePathToImage = Path.Combine(Application.StartupPath, "imagen_ejemplo.jpg");

            // Informar al usuario dónde se está buscando el archivo de sonido.
            MessageBox.Show($"El programa buscará el archivo de sonido en: {filePathToSound}\n" +
                            "Asegúrate de que 'tada.wav' esté en esa ubicación.",
                            "Ruta del Archivo de Sonido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Mensaje inicial al usuario para indicar qué hará la aplicación y dónde buscará la imagen.
            MessageBox.Show($"La aplicación intentará cargar una imagen desde la ruta local:\n{filePathToImage}\n" +
                            "Asegúrate de que el archivo de imagen exista en esa ubicación.",
                            "Información de Carga", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //*** Inicio metodo de subproceso por Invoke
        // Para no causar un problema de InvalidOperationException 
        // Se necesita delegar el metodo a entrar en subproceso fuera del controlador
        private void btnInvocar_Click(object sender, EventArgs e)
        {
            var parametrosHilo = new System.Threading.ThreadStart(
                delegate
                {
                    EscribirTexto("Este texto fue escrito seguramente");
                }
                );

            var hilo = new System.Threading.Thread(parametrosHilo);
            hilo.Start();
        }

        public void EscribirTexto(string texto)
        {
            // El metodo InvokeRequired revisa si el metodo a hacer fue llamado durante
            // un subproceso
            if (txtInvocar.InvokeRequired)
            {
                // Sí si se llamo atravez de un subproceso 
                // llama a este mismo metodo pero ahora intenta asignar 'THREAD2' al texto
                Action escrituraSegura = delegate { EscribirTexto($"{texto} THREAD2"); };
                txtInvocar.Invoke(escrituraSegura);
            }
            else
            {
                txtInvocar.Text = texto;
            }
        }
        //*** Fin metodo de subproceso por Invoke



        //*** Inicio metodo de subproceso por BackgroundWorker
        private void btnWorker_Click(object sender, EventArgs e)
        {
            // Revisamos que el componente no este ocupado
            if (!bgwProccess1.IsBusy)
            {
                bgwProccess1.RunWorkerAsync();
            }
        }

        private void bgwProccess1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int contador = 0;
            int maximo = 10;

            while (contador <= maximo)
            {
                // se hara un reporte periodicamente sobre el progreso del componente
                bgwProccess1.ReportProgress(0, contador.ToString());
                // Esperaremos 1s para ir progresando en el reporte
                System.Threading.Thread.Sleep(1000);
                contador++;
            }
        }

        // Cada que se reporte un progreso, se ejecutara el evento de progress changed
        private void bgwProccess1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Cada que se haga el reporte, cambiaremos el texto de la caja de texto
            txtWorker.Text = (string)e.UserState;
        }

        private void bgwProccess1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtWorker.Text = "FIN";
        }

        //*** Fin metodo de subproceso por BackgroundWorker

        //*** Inicio metodo de subproceso por BackgrounWorker 2

        private int numberToCompute = 0;
        private int highestPercentageReached = 0;

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Verificamos que el background no este ocupado
            if (!bgwProccess2.IsBusy)
            {
                bgwProccess2.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Si podemos cancelar, cancelamos
            if (bgwProccess2.WorkerSupportsCancellation)
            {
                bgwProccess2.CancelAsync();
            }
        }


        // El subproceso que tendrá el background, se ejecuta mientras
        // este activo
        private void bgwProccess2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 0; i <= 10; i++)
            {

                // Revisamos si el subproceso tiene una cancelación pendiente
                if (worker.CancellationPending)
                {
                    // si la tiene la cancelamos
                    e.Cancel = true;
                    break;
                }

                System.Threading.Thread.Sleep(500);
                worker.ReportProgress(i * 10);
            }

        }

        // Este evento se ejecuta cada que se reporta un progreso
        private void bgwProccess2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblResult.Text = (e.ProgressPercentage.ToString() + "%");
        }

        // Este evento se ejecuta al terminar el subproceso (sin importar la manera)
        private void bgwProccess2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Si el subproceso termino por cancelacion
            if (e.Cancelled)
            {
                lblResult.Text = "Cancelado!";
            }
            else if (e.Error != null) // Revisamos si termino por un error
            {
                lblResult.Text = "Error: " + e.Error.Message;
            }
            else // si no fue nada de lo anterior, termino exitosamente
            {
                lblResult.Text = "Finalizado!";
            }
        }

        // con este evento claramente empezaremos el conteo de la serie fibonacci
        private void btnAsyncStart_Click(object sender, EventArgs e)
        {
            lblResult2.Text = "";

            // Desactivamos los controles innecesarios para evitar problemas
            // al ejecutar el subproceso
            nudLimite.Enabled = false;
            btnAsyncStart.Enabled = false;

            // Activamos los necesarios
            btnAsyncCancel.Enabled = true;

            // Tomamos los valores necesarios
            numberToCompute = (int)nudLimite.Value;
            highestPercentageReached = 0;

            // Empezamos el subproceso
            bgwProccess3.RunWorkerAsync(numberToCompute);
        }

        // Este evento es para cancelar ya comenzado el subproceso
        private void btnAsyncCancel_Click(object sender, EventArgs e)
        {
            bgwProccess3.CancelAsync();

            btnAsyncCancel.Enabled = false;
        }

        // Manejaremos el subproceso para hacer los calculos
        private void bgwProccess3_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = ComputeFibonacci((int)e.Argument, worker, e);
        }

        // Este evento se encargara nada más de dar progresos sobre el porcentaje
        // completado
        private void bgwProccess3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbrProgreso.Value = e.ProgressPercentage;
        }

        // Este evento se encargara de los resultados del subproceso
        private void bgwProccess3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // revisamos que no haya algun error
            if (e.Error != null)
            {
                MessageBox.Show("HUBO UN ERROR INESPERADO: " + e.Error.Message);
            }
            else if (e.Cancelled) // También hay que revisar que no haya sido cancelado
            {
                lblResult2.Text = "Cancelado!";
            }
            else
            {
                // Si todo termino bien, daremos resultado
                lblResult2.Text = e.Result.ToString();
            }

            nudLimite.Enabled = true;
            btnAsyncStart.Enabled = true;
            btnAsyncCancel.Enabled = false;
        }

        // En este evento nos encargaremos de hacer la calculadora del fibonacci
        private long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            long result = 0;

            // Revisamos que no exceda el limite
            // Pasar el limite superior genera un overflow
            if (n < 0 || n > 91)
            {
                throw new ArgumentException("El valor debe de encontrarse entre [1,91]", "n");
            }

            // revisamos que no haya una cancelación en progreso
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                if (n < 2)
                {
                    result = 1;
                }
                else
                {
                    result = ComputeFibonacci(n - 1, worker, e) + ComputeFibonacci(n - 2, worker, e);
                }

                int porcentaje = (int)((float)n / (float)numberToCompute * 100f);

                if (porcentaje > highestPercentageReached)
                {
                    highestPercentageReached = porcentaje;
                    worker.ReportProgress(porcentaje);
                }
            }

            return result;
        }

        //*** Fin metodo de subproceso por BackgroundWorker 2

        //*** Inicio metodo para cargar un sonido de manera asincrona

        // Declaramos 'soundPlayerInstance' como una variable de instancia de la clase Form1.
        // Esto permite que el objeto SoundPlayer persista mientras el formulario esté abierto
        // y sea accesible desde cualquier método del formulario.
        private SoundPlayer soundPlayerInstance;

        // Ruta al archivo .wav que se cargará y reproducirá.
        // Ahora se inicializa en el constructor para tomar la ruta de ejecución del programa.
        private readonly string filePathToSound;

        // Evento para cargar la instancia y ejecutara otro evento al momento de que
        // cargue el sonido
        private void btnAsyncLoad_Click(object sender, EventArgs e)
        {
            // 1. Inicializar una nueva instancia de la clase SoundPlayer si no existe.
            //    Esto asegura que siempre trabajemos con una instancia limpia o existente.
            if (soundPlayerInstance == null)
            {
                soundPlayerInstance = new SoundPlayer();
                // 2. Suscribirse al evento LoadCompleted.
                //    Este evento es CRUCIAL para operaciones asíncronas. Se dispara
                //    cuando la carga del archivo .wav ha terminado.
                soundPlayerInstance.LoadCompleted += SoundPlayer_LoadCompleted;
            }

            // 3. Asignar la ruta del archivo de sonido a la propiedad SoundLocation del SoundPlayer.
            soundPlayerInstance.SoundLocation = filePathToSound;

            try
            {
                // Deshabilitar el botón para evitar clics múltiples mientras se carga el sonido.
                this.btnAsyncLoad.Enabled = false;
                this.btnAsyncLoad.Text = "Cargando...";

                // 4. Iniciar la carga asíncrona del archivo .wav.
                //    LoadAsync() no bloquea la interfaz de usuario, permitiendo que la aplicación
                //    siga respondiendo mientras el sonido se carga en segundo plano.
                soundPlayerInstance.LoadAsync();

                MessageBox.Show("Carga asíncrona iniciada. Por favor, espera a que el sonido se cargue.", "Información de Carga", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Capturar y reportar cualquier error que ocurra ANTES de que la carga asíncrona inicie.
                // Por ejemplo, si la ruta del archivo es inválida o hay un problema con el objeto SoundPlayer.
                MessageBox.Show($"Error al configurar SoundPlayer o iniciar la carga: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Habilitar el botón de nuevo en caso de error inicial.
                this.btnAsyncLoad.Enabled = true;
                this.btnAsyncLoad.Text = "Cargar y Reproducir Sonido";
            }
        }

        // Este evento se ejecuta UNICAMENTE cuando se haya cargado el sonido
        private void SoundPlayer_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Habilitar el botón de nuevo una vez que la carga ha terminado.
            this.btnAsyncLoad.Enabled = true;
            this.btnAsyncLoad.Text = "Cargar y Reproducir Sonido";

            // Verificar si hubo un error durante la carga asíncrona.
            if (e.Error != null)
            {
                MessageBox.Show($"¡Error durante la carga asíncrona del sonido!: {e.Error.Message}", "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Verificar si la operación de carga fue cancelada.
            else if (e.Cancelled)
            {
                MessageBox.Show("La carga asíncrona del sonido fue CANCELADA.", "Carga Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Si no hubo errores ni la operación fue cancelada, significa que la carga se completó con éxito.
            else
            {
                MessageBox.Show("¡Carga asíncrona COMPLETADA con éxito! El sonido está listo para reproducirse.", "Carga Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    // Una vez que el sonido ha sido cargado completamente en memoria, es seguro reproducirlo.
                    // El método Play() reproduce el sonido una vez.
                    soundPlayerInstance.Play();
                    MessageBox.Show("Sonido reproducido.", "Reproducción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Capturar cualquier error que pueda ocurrir durante el intento de reproducción.
                    MessageBox.Show($"Error al intentar reproducir el sonido: {ex.Message}", "Error de Reproducción", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //*** Fin metodo para cargar un sonido de manera asincrona

        //*** Inicio metodo para cargar una imagen de manera asincrona

        // Ruta local de la imagen que se cargará asíncronamente.
        // Se inicializa en el constructor para tomar la ruta de ejecución del programa.
        private readonly string filePathToImage;

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Habilitar el botón de nuevo una vez que la carga ha terminado.
            btnAsyncImage.Enabled = true;
            btnAsyncImage.Text = "Cargar";

            // Verificar si hubo un error durante la carga asíncrona.
            if (e.Error != null)
            {
                MessageBox.Show($"¡Error durante la carga asíncrona de la imagen!: {e.Error.Message}", "Error de Carga de Imagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pictureBox1.Image = null; // Asegurarse de que no se muestre una imagen parcial o corrupta.
                pictureBox1.Text = "Error al cargar la imagen."; // Mostrar un mensaje de error en el PictureBox.
            }
            // Verificar si la operación de carga fue cancelada.
            else if (e.Cancelled)
            {
                MessageBox.Show("La carga asíncrona de la imagen fue CANCELADA.", "Carga Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBox1.Image = null; // Limpiar la imagen si la carga fue cancelada.
                pictureBox1.Text = "Carga de imagen cancelada."; // Mostrar mensaje de cancelación.
            }
            // Si no hubo errores ni la operación fue cancelada, significa que la carga se completó con éxito.
            else
            {
                MessageBox.Show("¡Carga asíncrona de la imagen COMPLETADA con éxito!", "Carga Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // La imagen ya está asignada al PictureBox por el método LoadAsync() internamente.
                pictureBox1.Text = ""; // Limpiar el texto del PictureBox una vez que la imagen se muestra.
            }
        }

        private void btnAsyncImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Deshabilitar el botón para evitar clics múltiples mientras la imagen se está cargando.
                btnAsyncImage.Enabled = false;
                btnAsyncImage.Text = "Cargando...";
                pictureBox1.Image = null; // Limpiar cualquier imagen anterior que se esté mostrando.
                pictureBox1.Text = "Cargando..."; // Mostrar un mensaje de carga en el PictureBox.

                // Iniciar la carga asíncrona de la imagen desde la ruta de archivo local.
                // El método LoadAsync(url) también puede tomar una ruta de archivo local.
                // No bloquea el hilo principal de la interfaz de usuario, permitiendo que la aplicación
                // siga respondiendo mientras la imagen se carga en segundo plano.
                pictureBox1.LoadAsync(filePathToImage);

                MessageBox.Show("Carga asíncrona de la imagen iniciada. Por favor, espera a que la imagen se cargue.", "Información de Carga", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Capturar y reportar cualquier error que ocurra ANTES de que la carga asíncrona inicie.
                // Por ejemplo, si la ruta del archivo es inválida o hay un problema con el objeto PictureBox.
                MessageBox.Show($"Error al iniciar la carga de la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Habilitar el botón de nuevo en caso de error inicial.
                btnAsyncImage.Enabled = true;
                btnAsyncImage.Text = "Cargar";
                pictureBox1.Text = "Error inicial."; // Mensaje de error en el PictureBox.
            }
        }

        //*** Fin metodo para cargar una imagen de manera asincrona

        // Ejemplo basico para el uso del background worker
        /*private void bgwProccess1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Tomamos referencia de nuestro BackgroundWorker
            BackgroundWorker worker = sender as BackgroundWorker;

            // Si se termina el subproceso, devolvera un resultado.
            // Para acceder al resultado se hara atravez del evento RunWorkerCompleted
            // (Resultado) = (Metodo(Argumentos, trabajador,  manejador del proceso))
            //e.Result = ComputeFibonacci((int)e.Argument, worker, e);
        }*/


    }
}

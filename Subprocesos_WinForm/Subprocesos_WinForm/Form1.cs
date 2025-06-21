using System.ComponentModel;

namespace Subprocesos_WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

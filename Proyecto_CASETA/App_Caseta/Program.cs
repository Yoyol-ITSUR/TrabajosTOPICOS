using App_Caseta.Servicios;

namespace App_Caseta
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuraci�n para el escalado de DPI de alta resoluci�n
            // Esto ayuda a que la aplicaci�n se vea consistente en diferentes pantallas.
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            string apiUrl = string.Empty;

            // Mostramos el formulario de entrada de URL primero
            using (frmUrlEntrada urlForm = new frmUrlEntrada())
            {
                // Mostramos el formulario de URL de forma modal.
                // Esto significa que el c�digo no continuar� hasta que urlForm se cierre.
                if (urlForm.ShowDialog() == DialogResult.OK)
                {
                    apiUrl = urlForm.BaseApiUrl; // Obtenemos la URL confirmada
                }
                else
                {
                    // Si el usuario cancela o cierra el formulario de URL, se usara la url default
                    MessageBox.Show("La URL de la API no fue proporcionada. La aplicaci�n usara \"https://localhost:7199\".", "Configuraci�n Requerida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    apiUrl = "https://localhost:7199";
                }
            }

            // Si se obtuvo una URL, inicializar el ApiClient y el formulario principal
            if (!string.IsNullOrEmpty(apiUrl))
            {
                ApiClient.url = apiUrl;

                Application.Run(new frmAccesoPrincipal());
            }
        }
    }
}
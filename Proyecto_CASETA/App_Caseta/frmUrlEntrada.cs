using System;
using System.Drawing;
using System.Windows.Forms;
using App_Caseta.Servicios; // Necesario para ApiClient (aunque no se instancia aquí, es parte del contexto)
using System.Net.Http; // Necesario para HttpRequestException (para la prueba de conexión)

namespace App_Caseta
{
    public partial class frmUrlEntrada : Form
    {
        // Propiedad pública para que el formulario principal pueda acceder a la URL ingresada.
        public string BaseApiUrl { get; private set; }

        private Label lblTitle;
        private Label lblInstructions;
        private TextBox txtApiUrl;
        private Button btnConfirm;

        /// <summary>
        /// Constructor del formulario de entrada de URL.
        /// Inicializa los componentes de la UI y configura el diseño.
        /// </summary>
        public frmUrlEntrada()
        {
            InitializeComponent(); // Método generado por el diseñador de Forms.
            SetupFormLayout();     // Configura el diseño y los controles del formulario.
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// Aplica un estilo consistente con la paleta de colores de GitHub Desktop.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "Configuración de API";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(500, 280); // Tamaño pequeño y fijo
            this.MinimumSize = new Size(500, 280);
            this.MaximumSize = new Size(500, 280);
            this.StartPosition = FormStartPosition.CenterScreen; // Centrar en la pantalla
            this.FormBorderStyle = (FormBorderStyle)BorderStyle.FixedSingle; // No redimensionable
            this.MaximizeBox = false; // Deshabilitar botón maximizar
            this.MinimizeBox = false; // Deshabilitar botón minimizar

            // Título del formulario
            lblTitle = new Label
            {
                Text = "Ingresa la URL de tu API",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(this.Width / 2 - 180, 30), // Centrado horizontalmente
                ForeColor = ColorTranslator.FromHtml("#e0e0e0")
            };
            lblTitle.Anchor = AnchorStyles.Top; // Anclado a la parte superior
            this.Controls.Add(lblTitle);

            // Instrucciones para el usuario
            lblInstructions = new Label
            {
                Text = "Por favor, introduce la URL base de tu Web API (ej. https://abcdef123456.ngrok-free.app):",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(50, 80),
                MaximumSize = new Size(400, 0) // Permite que el texto se ajuste en varias líneas
            };
            this.Controls.Add(lblInstructions);

            // Campo de entrada para la URL de la API
            txtApiUrl = new TextBox
            {
                Location = new Point(50, 140),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de input
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"), // Texto de input
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "https://tu-api.ngrok-free.app" // Texto de ejemplo
            };
            this.Controls.Add(txtApiUrl);

            // Botón de Confirmar
            btnConfirm = new Button
            {
                Text = "Confirmar URL",
                Location = new Point(this.Width / 2 - 75, 180), // Centrado horizontalmente
                Size = new Size(150, 40),
                BackColor = ColorTranslator.FromHtml("#238636"), // Verde
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }, // Sin borde
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnConfirm.Click += BtnConfirm_Click; // Asignar manejador de evento
            this.Controls.Add(btnConfirm);
        }

        /// <summary>
        /// Manejador del evento Click del botón Confirmar.
        /// Valida la URL ingresada y la guarda si es válida.
        /// Realiza una prueba de conexión simple a la URL.
        /// </summary>
        private async void BtnConfirm_Click(object sender, EventArgs e)
        {
            string url = txtApiUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("La URL no puede estar vacía. Por favor, ingresa una URL válida.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validación de formato de URL básica
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                MessageBox.Show("Por favor, ingresa una URL válida que comience con http:// o https://", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Eliminar la barra final si existe para asegurar consistencia en ApiClient
            if (url.EndsWith("/"))
            {
                url = url.TrimEnd('/');
            }

            // Intentar una conexión de prueba para verificar que la URL es accesible.
            // Se usa un HttpClient temporal para esta prueba.
            using (var testClient = new HttpClient())
            {
                testClient.Timeout = TimeSpan.FromSeconds(5); // Tiempo de espera corto para la prueba
                try
                {
                    // Intentar una petición GET a la URL base o un endpoint conocido y ligero.
                    // Aquí se usa el endpoint raíz, que debería responder si la API está viva.
                    var response = await testClient.GetAsync($"{url}/api/Users"); // Asumiendo que /api/Users es un endpoint accesible
                    response.EnsureSuccessStatusCode(); // Lanza excepción para códigos de error HTTP (4xx, 5xx)
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"No se pudo conectar a la URL proporcionada o la API no respondió correctamente. " +
                                    $"Por favor, verifica que la URL sea correcta y que tu API esté en funcionamiento.\n\n" +
                                    $"Detalles del error: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error inesperado al intentar conectar con la URL. " +
                                    $"Detalles: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Si la validación y la prueba de conexión son exitosas:
            this.BaseApiUrl = url;          // Guardar la URL en la propiedad pública
            this.DialogResult = DialogResult.OK; // Indicar que la operación fue exitosa
            this.Close();                   // Cerrar este formulario
        }
    }
}

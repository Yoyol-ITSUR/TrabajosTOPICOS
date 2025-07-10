using App_Caseta.Servicios;
using System;
using System.Drawing;
using System.Net.Http; // Necesario para HttpRequestException
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Caseta
{
    public partial class frmAccesoPrincipal : Form
    {
        // Instancia del cliente API para comunicarse con la Web API.
        private readonly ApiClient _apiClient;

        // URL base de tu Web API (¡REEMPLAZA ESTO CON TU URL DE NGROK!)
        private const string BaseApiUrl = "TU_URL_NGROK_AQUI";

        // Componentes de UI
        private TextBox txtQrToken;
        private Button btnScanQr;
        private Label lblInfoTitle;
        private Label lblGuestName;
        private Label lblInvitingResident;
        private Label lblStatusMessage;
        private Button btnResidentes;
        private Button btnInvitados;
        private Button btnHistorial;
        private Button btnUsuarios;

        /// <summary>
        /// Constructor del formulario principal de acceso.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmAccesoPrincipal()
        {
            InitializeComponent(); // Método generado por el diseñador de Forms.
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout(); // Configura el diseño y los controles del formulario.
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// Aplica un estilo de fondo azul oscuro y organiza los elementos.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Caseta de Vigilancia";
            this.BackColor = ColorTranslator.FromHtml("#073B4C"); // Fondo azul oscuro
            this.ForeColor = Color.White; // Texto blanco
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(800, 600); // Evitar que se haga más pequeño
            this.MaximumSize = new Size(800, 600); // Evitar que se haga más grande
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // No redimensionable
            this.MaximizeBox = false;

            // Título principal
            Label lblMainTitle = new Label
            {
                Text = "Panel de Control de Acceso",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 30),
                ForeColor = ColorTranslator.FromHtml("#06D6A0") // Verde vibrante
            };
            this.Controls.Add(lblMainTitle);

            // Campo para escanear/ingresar QR
            Label lblQrPrompt = new Label
            {
                Text = "Ingresa/Escanea Código QR:",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(50, 100)
            };
            this.Controls.Add(lblQrPrompt);

            txtQrToken = new TextBox
            {
                Location = new Point(50, 130),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Token QR"
            };
            this.Controls.Add(txtQrToken);

            btnScanQr = new Button
            {
                Text = "Procesar QR",
                Location = new Point(360, 128),
                Size = new Size(120, 30),
                BackColor = ColorTranslator.FromHtml("#118AB2"), // Azul
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnScanQr.Click += BtnScanQr_Click;
            this.Controls.Add(btnScanQr);

            // Área de visualización de información
            lblInfoTitle = new Label
            {
                Text = "Información del Acceso:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 180),
                ForeColor = ColorTranslator.FromHtml("#FFD166") // Amarillo
            };
            this.Controls.Add(lblInfoTitle);

            lblGuestName = new Label
            {
                Text = "Nombre: ",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(50, 220)
            };
            this.Controls.Add(lblGuestName);

            lblInvitingResident = new Label
            {
                Text = "Invitado por: ",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(50, 250)
            };
            this.Controls.Add(lblInvitingResident);

            // Mensajes de estado
            lblStatusMessage = new Label
            {
                Text = "Esperando escaneo...",
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(50, 300),
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblStatusMessage);

            // Botones de navegación
            int btnWidth = 150;
            int btnHeight = 40;
            int startX = 50;
            int startY = 400;
            int padding = 20;

            btnResidentes = CreateNavButton("Gestión Residentes", new Point(startX, startY));
            btnResidentes.Click += (s, e) => OpenForm(new frmResidentes());
            this.Controls.Add(btnResidentes);

            btnInvitados = CreateNavButton("Gestión Invitados", new Point(startX + btnWidth + padding, startY));
            btnInvitados.Click += (s, e) => OpenForm(new frmInvitados());
            this.Controls.Add(btnInvitados);

            btnHistorial = CreateNavButton("Historial Accesos", new Point(startX + (btnWidth + padding) * 2, startY));
            btnHistorial.Click += (s, e) => OpenForm(new frmHistorial());
            this.Controls.Add(btnHistorial);

            btnUsuarios = CreateNavButton("Gestión Usuarios", new Point(startX + (btnWidth + padding) * 3, startY));
            btnUsuarios.Click += (s, e) => OpenForm(new frmUsuarios());
            this.Controls.Add(btnUsuarios);
        }

        /// <summary>
        /// Crea un botón de navegación con estilo predefinido.
        /// </summary>
        /// <param name="text">Texto del botón.</param>
        /// <param name="location">Ubicación del botón.</param>
        /// <returns>Un objeto Button configurado.</returns>
        private Button CreateNavButton(string text, Point location)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(150, 40),
                BackColor = ColorTranslator.FromHtml("#FF6B6B"), // Rojo vibrante
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Abre un nuevo formulario y lo muestra, ocultando el formulario principal.
        /// </summary>
        /// <param name="formToOpen">El formulario a abrir.</param>
        private void OpenForm(Form formToOpen)
        {
            this.Hide(); // Oculta el formulario principal
            formToOpen.FormClosed += (s, args) => this.Show(); // Muestra el principal cuando el nuevo se cierra
            formToOpen.Show();
        }

        /// <summary>
        /// Manejador del evento Click del botón "Procesar QR".
        /// Envía el token QR a la API para registrar la entrada/salida.
        /// </summary>
        private async void BtnScanQr_Click(object sender, EventArgs e)
        {
            string qrToken = txtQrToken.Text.Trim();
            if (string.IsNullOrEmpty(qrToken))
            {
                UpdateStatus("Por favor, ingresa un token QR.", Color.OrangeRed);
                return;
            }

            btnScanQr.Enabled = false; // Deshabilitar botón durante la operación
            UpdateStatus("Procesando QR...", Color.LightBlue);

            try
            {
                var scanRequest = new ScanRequestDto { QrToken = qrToken, GuardiaId = 1 }; // GuardiaId es opcional, usar 1 como ejemplo
                // La API devuelve un string de mensaje, por eso se usa string como tipo de retorno.
                var responseMessage = await _apiClient.PostAsync<string>("BitacoraRegistro/Scan", scanRequest);

                UpdateStatus(responseMessage, Color.LightGreen); // Mensaje de éxito de la API

                // Opcional: Si la API devolviera un objeto con el invitado/residente,
                // se podría parsear y mostrar aquí. Para este ejemplo, solo mostramos el mensaje.
                lblGuestName.Text = "Nombre: (Ver mensaje de estado)";
                lblInvitingResident.Text = "Invitado por: (Ver mensaje de estado)";
            }
            catch (HttpRequestException httpEx)
            {
                // Capturar errores HTTP específicos (ej. 404 Not Found, 400 Bad Request)
                string errorMessage = $"Error de API: {httpEx.Message}";
                if (httpEx.StatusCode.HasValue)
                {
                    errorMessage = $"Error HTTP {httpEx.StatusCode.Value}: {httpEx.Message}";
                }
                UpdateStatus(errorMessage, Color.Red);
            }
            catch (Exception ex)
            {
                // Capturar otros errores inesperados
                UpdateStatus($"Error inesperado: {ex.Message}", Color.Red);
            }
            finally
            {
                btnScanQr.Enabled = true; // Habilitar botón de nuevo
                txtQrToken.Clear(); // Limpiar el campo del token
            }
        }

        /// <summary>
        /// Actualiza el mensaje de estado en la interfaz de usuario con un color específico.
        /// </summary>
        /// <param name="message">El mensaje a mostrar.</param>
        /// <param name="color">El color del texto del mensaje.</param>
        private void UpdateStatus(string message, Color color)
        {
            lblStatusMessage.Text = message;
            lblStatusMessage.ForeColor = color;
        }
    }
}

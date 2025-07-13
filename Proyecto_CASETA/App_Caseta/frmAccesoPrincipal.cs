using App_Caseta.Servicios;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Sirve como el panel de control central para los guardias,
 * permitiendo el escaneo de "c�digos" QR para registrar entradas y salidas,
 * y proporcionando botones para navegar a los otros formularios de gesti�n
*/

// Se tuvo que plantar el dise�o dentro de este form
// por que don menso la rego en algo y no supe como solucionarlo
// intentos de dise�o a "ciegas" : 39
namespace App_Caseta
{
    public partial class frmAccesoPrincipal : Form
    {
        private readonly ApiClient _apiClient;

        private readonly string BaseApiUrl = ApiClient.url;

        // Componentes de UI
        private TextBox txtQrToken;
        private Button btnScanQr;
        private Button btnResidentes;
        private Button btnInvitados;
        private Button btnHistorial;
        private Button btnUsuarios;

        // Componentes para mostrar el resultado del escaneo
        private Panel pnlScanResult;
        private Label lblResultTitle;
        private Label lblPersonName;
        private Label lblPersonType;
        private Label lblAccessType;
        private Label lblMessage; // Para el mensaje general de la API

        /// <summary>
        /// Constructor del formulario principal de acceso.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmAccesoPrincipal()
        {
            InitializeComponent();
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout(); // Metodo para la construccion del dise�o al momento de ejecuci�n
            ClearScanResultDisplay(); // Limpiar el panel de resultados al inicio
        }

        /// <summary>
        /// Configura el dise�o y los controles visuales del formulario.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Control de Acceso Residencial";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(1200, 740);
            this.MinimumSize = new Size(1200, 740);
            this.MaximumSize = new Size(1200, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // T�tulo
            Label lblTitle = new Label
            {
                Text = "Bienvenido a SCAR",
                Font = new Font("Segoe UI", 30, FontStyle.Bold), // Fuente m�s grande
                AutoSize = true,
                Location = new Point(this.Width / 2 - 250, 60), // Ajustar posici�n para centrar
                ForeColor = ColorTranslator.FromHtml("#e0e0e0")
            };
            lblTitle.Anchor = AnchorStyles.Top;
            this.Controls.Add(lblTitle);

            // Campo de entrada para el token QR
            Label lblQrToken = new Label {
                Text = "Token QR:",
                Location = new Point(this.Width / 2 - 250, 180),
                AutoSize = true,
                Font = new Font("Segoe UI", 14) 
            };

            this.Controls.Add(lblQrToken);
            txtQrToken = new TextBox
            {
                Location = new Point(520, 177),
                Size = new Size(300, 30), // Tama�o ajustado
                Font = new Font("Segoe UI", 12),
                BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de input
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"), // Texto de input
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtQrToken);

            // Bot�n de Escanear QR
            // Se usan metodos para crear casi siempre el mismo dise�o de boton.
            // Similar como en kotlin
            btnScanQr = CreateActionButton(
                "Escanear QR",
                new Point(520, 220), 
                ColorTranslator.FromHtml("#238636")
                );

            // Agregamos el evento click
            btnScanQr.Click += BtnScanQr_Click;
            this.Controls.Add(btnScanQr);

            // Panel para mostrar el resultado del escaneo
            pnlScanResult = new Panel
            {
                Location = new Point(100, 280), // Ajustar posici�n
                Size = new Size(1000, 250), // Tama�o ajustado
                BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de panel (m�s claro que el fondo general)
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                Visible = false // Oculto inicialmente
            };
            this.Controls.Add(pnlScanResult);

            lblResultTitle = new Label
            {
                Text = "Resultado del Escaneo",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15),
                ForeColor = ColorTranslator.FromHtml("#58a6ff") // Azul de acento
            };
            pnlScanResult.Controls.Add(lblResultTitle);

            lblPersonName = new Label
            {
                Text = "Nombre: ",
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                Location = new Point(15, 65),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") // Texto claro
            };
            pnlScanResult.Controls.Add(lblPersonName);

            lblPersonType = new Label
            {
                Text = "Tipo: ",
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                Location = new Point(15, 95),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") // Texto claro
            };
            pnlScanResult.Controls.Add(lblPersonType);

            lblAccessType = new Label
            {
                Text = "Acceso: ",
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                Location = new Point(15, 125),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") // Texto claro
            };
            pnlScanResult.Controls.Add(lblAccessType);

            lblMessage = new Label
            {
                Text = "Mensaje: ",
                Font = new Font("Segoe UI", 13, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(15, 175),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") // Texto claro
            };
            pnlScanResult.Controls.Add(lblMessage);


            // Botones de Navegaci�n a otros formularios
            // son los "coordenadas" clave para poder dise�ar a ciegas :C
            int btnNavX = 50;
            int btnNavY = 580; // Ajustar la posici�n Y para dejar espacio al panel
            int btnNavSpacing = 280; // Mayor espaciado

            // -- Boton de residente
            btnResidentes = CreateActionButton(
                "Gesti�n Residentes",
                new Point(btnNavX, btnNavY),
                ColorTranslator.FromHtml("#444c56")
                );

            // Se usa una funci�n lambda directa que llama a un "metodo" sin tener
            // que crear la estrutura para un evento y simplemente anidar un metodo
            // para simplificar la misma funci�n, solo cambia el formulario a abrir.
            btnResidentes.Click += (s, e) => OpenForm(new frmResidentes());
            this.Controls.Add(btnResidentes);


            // -- Boton de Invitados
            btnInvitados = CreateActionButton(
                "Gesti�n Invitados",
                new Point(btnNavX + btnNavSpacing, btnNavY),
                ColorTranslator.FromHtml("#444c56")
                ); // Bot�n secundario
            btnInvitados.Click += (s, e) => OpenForm(new frmInvitados());
            this.Controls.Add(btnInvitados);

            // -- Boton de Historial
            btnHistorial = CreateActionButton(
                "Historial de Accesos",
                new Point(btnNavX + btnNavSpacing * 2, btnNavY),
                ColorTranslator.FromHtml("#444c56")
                );
            btnHistorial.Click += (s, e) => OpenForm(new frmHistorial());
            this.Controls.Add(btnHistorial);

            // -- Boton de Usuarios
            btnUsuarios = CreateActionButton(
                "Gesti�n Usuarios",
                new Point(btnNavX + btnNavSpacing * 3, btnNavY),
                ColorTranslator.FromHtml("#444c56")
                );
            btnUsuarios.Click += (s, e) => OpenForm(new frmUsuarios());
            this.Controls.Add(btnUsuarios);
        }

        /// <summary>
        /// Crea un bot�n de acci�n con estilo.
        /// </summary>
        private Button CreateActionButton(string text, Point location, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(250, 45),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Limpia y oculta el panel de resultados del escaneo.
        /// </summary>
        private void ClearScanResultDisplay()
        {
            lblResultTitle.Text = "Resultado del Escaneo";
            lblResultTitle.ForeColor = ColorTranslator.FromHtml("#58a6ff"); // Resetear color
            lblPersonName.Text = "Nombre: ";
            lblPersonType.Text = "Tipo: ";
            lblAccessType.Text = "Acceso: ";
            lblMessage.Text = "Mensaje: ";
            pnlScanResult.Visible = false;
            pnlScanResult.BackColor = ColorTranslator.FromHtml("#3f444a"); // Resetear color de fondo
        }

        /// <summary>
        /// Muestra los resultados del escaneo en el panel.
        /// </summary>
        private void DisplayScanResult(ScanResponseDto response)
        {
            pnlScanResult.Visible = true;
            lblResultTitle.Text = response.IsSuccess ? "�Acceso Exitoso!" : "Acceso Denegado";
            lblResultTitle.ForeColor = response.IsSuccess ? ColorTranslator.FromHtml("#58a6ff") : ColorTranslator.FromHtml("#da3633"); // Azul para �xito, rojo para error
            pnlScanResult.BackColor = response.IsSuccess ? ColorTranslator.FromHtml("#3f444a") : ColorTranslator.FromHtml("#444c56"); // Fondo de panel

            lblPersonName.Text = $"Nombre: {response.PersonName}";
            lblPersonType.Text = $"Tipo: {response.PersonType}";
            lblAccessType.Text = $"Acceso: {response.AccessType}";
            lblMessage.Text = $"Mensaje: {response.Message}";
        }

        /// <summary>
        /// Manejador del evento Click del bot�n "Escanear QR".
        /// Env�a el token QR a la API para validar el acceso.
        /// </summary>
        private async void BtnScanQr_Click(object sender, EventArgs e)
        {
            ClearScanResultDisplay(); // Limpiar resultados anteriores

            string qrToken = txtQrToken.Text.Trim();
            if (string.IsNullOrEmpty(qrToken))
            {
                MessageBox.Show("Por favor, ingresa un token QR.", "Validaci�n", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int guardiaId = 1; // Asumiendo un GuardiaId fijo para la prueba

            try
            {
                var scanRequest = new ScanRequestDto { QrToken = qrToken, GuardiaId = guardiaId };
                var responseDto = await _apiClient.PostAsync<ScanResponseDto>("BitacoraRegistro/Scan", scanRequest);

                DisplayScanResult(responseDto); // Mostrar los resultados en el panel

                txtQrToken.Clear(); // Limpiar el campo despu�s del escaneo
            }
            catch (HttpRequestException httpEx)
            {
                string errorMessage = $"Error de API: {httpEx.Message}";
                if (httpEx.StatusCode.HasValue)
                {
                    errorMessage = $"Error HTTP {httpEx.StatusCode.Value}: {httpEx.Message}";
                }
                MessageBox.Show(errorMessage, "Error de Conexi�n/API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si hay un error HTTP, podemos mostrar un mensaje de error gen�rico en el panel
                DisplayScanResult(new ScanResponseDto { Message = "Error de conexi�n o API. Consulta los detalles.", IsSuccess = false });
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"Error de formato de respuesta del servidor: {jsonEx.Message}\n" +
                                "La API no devolvi� una respuesta JSON v�lida. Por favor, verifica el log de la API para m�s detalles.",
                                "Error de Datos del Servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisplayScanResult(new ScanResponseDto { Message = "Error de datos del servidor. Formato JSON inv�lido.", IsSuccess = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al escanear QR: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisplayScanResult(new ScanResponseDto { Message = "Error inesperado. Consulta el log de la aplicaci�n.", IsSuccess = false });
            }
        }

        /// <summary>
        /// Abre un nuevo formulario, ocultando el formulario actual y volvi�ndolo a mostrar al cerrar el nuevo.
        /// </summary>
        /// <param name="formToOpen">La instancia del formulario a abrir.</param>
        private void OpenForm(Form formToOpen)
        {
            this.Hide(); // Oculta el formulario principal
            formToOpen.ShowDialog(); // Muestra el nuevo formulario de forma modal (bloquea el principal)
            this.Show(); // Vuelve a mostrar el formulario principal cuando el nuevo se cierra
        }
    }
}

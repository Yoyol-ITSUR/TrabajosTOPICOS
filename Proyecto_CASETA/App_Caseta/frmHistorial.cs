using App_Caseta.Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http; // Necesario para HttpRequestException
using System.Text.Json; // Necesario para JsonException
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Formulario que muestra el registro completo de entradas y salidas de residentes e invitados.
 * Permite filtrar el historial por fechas, tipo de persona y 
 * ver el estado actual de las personas dentro del fraccionamiento.
*/

// Se tuvo que plantar el diseño dentro de este form
// por que don menso la rego en algo y no supe como solucionarlo
// intentos de diseño a "ciegas" : también perdi el conteo
namespace App_Caseta
{
    public partial class frmHistorial : Form
    {
        private readonly ApiClient _apiClient;

        private string BaseApiUrl = ApiClient.url;

        // Componentes de UI
        private DataGridView dgvHistorial;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private CheckBox chkCurrentStatus;
        private Button btnFilter;
        private Button btnClearFilter;

        /// <summary>
        /// Constructor del formulario de historial de accesos.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmHistorial()
        {
            InitializeComponent();
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout();
            LoadHistorial(); // Cargar historial al iniciar el formulario
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Historial de Accesos";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(1500, 820);
            this.MinimumSize = new Size(1500, 820);
            this.MaximumSize = new Size(1500, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            Label lblTitle = new Label
            {
                Text = "Historial de Accesos",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 20), 
                ForeColor = ColorTranslator.FromHtml("#e0e0e0")
            };
            this.Controls.Add(lblTitle);

            // Controles de filtro
            int filterX = 50;
            int filterY = 80; 
            // Espaciado horizontal ajustado para evitar superposición
            int dtpWidth = 200;
            int labelWidth = 130; // Ancho estimado de la etiqueta "Fecha Inicio:"
            int spacingBetweenDatePickers = 50; // Espacio entre el primer DTP y la segunda etiqueta
            int checkboxXOffset = 40; // Espacio adicional para el checkbox

            // Label para los dtp de filtro
            Label lblStartDate = new Label {
                Text = "Fecha Inicio:", 
                Location = new Point(filterX, filterY + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc")
            };
            this.Controls.Add(lblStartDate);
            // Fecha de inicio
            dtpStartDate = new DateTimePicker
            {
                Location = new Point(filterX + labelWidth, filterY),
                Size = new Size(dtpWidth, 30),
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                CalendarMonthBackground = ColorTranslator.FromHtml("#3f444a"),
                CalendarTitleBackColor = ColorTranslator.FromHtml("#444c56"),
                CalendarTitleForeColor = Color.White,
                CalendarTrailingForeColor = ColorTranslator.FromHtml("#6a737d")
            };
            this.Controls.Add(dtpStartDate);

            // Fecha de fin
            Label lblEndDate = new Label {
                Text = "Fecha Fin:", 
                Location = new Point(dtpStartDate.Location.X + dtpStartDate.Width + spacingBetweenDatePickers, filterY + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc")
            };
            this.Controls.Add(lblEndDate);
            dtpEndDate = new DateTimePicker
            {
                Location = new Point(lblEndDate.Location.X + lblEndDate.Width + 10, filterY),
                Size = new Size(dtpWidth, 30),
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                CalendarMonthBackground = ColorTranslator.FromHtml("#3f444a"),
                CalendarTitleBackColor = ColorTranslator.FromHtml("#444c56"),
                CalendarTitleForeColor = Color.White,
                CalendarTrailingForeColor = ColorTranslator.FromHtml("#6a737d")
            };
            this.Controls.Add(dtpEndDate);

            // Checkbox para ver las entradas activas
            chkCurrentStatus = new CheckBox
            {
                Text = "Mostrar Solo Entradas Activas",
                Location = new Point(dtpEndDate.Location.X + dtpEndDate.Width + checkboxXOffset, filterY),
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc")
            };
            chkCurrentStatus.CheckedChanged += ChkCurrentStatus_CheckedChanged;
            this.Controls.Add(chkCurrentStatus);

            //  Botones para filtrar
            int btnFilterX = chkCurrentStatus.Location.X + chkCurrentStatus.Width + 30; 
            btnFilter = CreateActionButton("Filtrar",
                new Point(btnFilterX, filterY - 5), 
                ColorTranslator.FromHtml("#238636")
                ); // Verde
            btnFilter.Click += BtnFilter_Click;
            this.Controls.Add(btnFilter);

            btnClearFilter = CreateActionButton(
                "Limpiar Filtros",
                new Point(btnFilter.Location.X + btnFilter.Width + 20, filterY - 5), 
                ColorTranslator.FromHtml("#444c56")
                ); // Gris
            btnClearFilter.Click += BtnClearFilter_Click;
            this.Controls.Add(btnClearFilter);


            // DataGridView para mostrar historial
            dgvHistorial = new DataGridView
            {
                Location = new Point(50, filterY + 50), // Posición de la tabla, más cerca de los filtros
                Size = new Size(1100, 580), // Aumentar el tamaño de la tabla para llenar el espacio
                BackgroundColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de DataGridView
                AlternatingRowsDefaultCellStyle = { BackColor = ColorTranslator.FromHtml("#2f363d") }, // Filas alternas
                ColumnHeadersDefaultCellStyle = {
                    BackColor = ColorTranslator.FromHtml("#444c56"), // Fondo de encabezado
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = {
                    BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de celda
                    ForeColor = ColorTranslator.FromHtml("#f0f6fc"), // Texto de celda
                    SelectionBackColor = ColorTranslator.FromHtml("#58a6ff"), // Color de selección
                    SelectionForeColor = Color.White
                },
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Ajustar columnas automáticamente
                BorderStyle = BorderStyle.None // Eliminar borde para mejor integración visual
            };
            this.Controls.Add(dgvHistorial);
        }

        /// <summary>
        /// Crea un botón de acción con estilo predefinido.
        /// </summary>
        private Button CreateActionButton(string text, Point location, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(150, 35), // Tamaño ajustado para filtros
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Carga el historial de accesos desde la API y lo muestra en el DataGridView.
        /// </summary>
        private async void LoadHistorial()
        {
            try
            {
                List<BitacoraRegistroDto> historial;

                if (chkCurrentStatus.Checked)
                {
                    historial = await _apiClient.GetAsync<List<BitacoraRegistroDto>>("BitacoraRegistro/CurrentStatus");
                }
                else
                {
                    // Formatear fechas para la URL
                    string startDate = dtpStartDate.Value.ToString("yyyy-MM-dd");
                    string endDate = dtpEndDate.Value.ToString("yyyy-MM-dd");
                    historial = await _apiClient.GetAsync<List<BitacoraRegistroDto>>($"BitacoraRegistro/ByDate?startDate={startDate}&endDate={endDate}");
                }

                // Procesar los datos para mostrar nombres completos y tipos de persona
                var processedHistorial = historial.Select(b => new
                {
                    b.Id,
                    b.FechaHoraEntrada,
                    b.FechaHoraSalida,
                    Persona = b.Residente != null ? $"{b.Residente.Nombre} {b.Residente.Apellido}" :
                              (b.Invitado != null ? $"{b.Invitado.Nombre} {b.Invitado.Apellido}" : "Desconocido"),
                    Tipo = b.Residente != null ? "Residente" : (b.Invitado != null ? "Invitado" : "N/A")
                }).ToList();

                dgvHistorial.DataSource = processedHistorial;

                // Ajustar encabezados de columna
                dgvHistorial.Columns["Id"].HeaderText = "ID Registro";
                dgvHistorial.Columns["FechaHoraEntrada"].HeaderText = "Fecha/Hora Entrada";
                dgvHistorial.Columns["FechaHoraSalida"].HeaderText = "Fecha/Hora Salida";
                dgvHistorial.Columns["Persona"].HeaderText = "Persona";
                dgvHistorial.Columns["Tipo"].HeaderText = "Tipo";
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al cargar historial: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar historial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento CheckedChanged del CheckBox "Mostrar Solo Entradas Activas".
        /// Deshabilita/Habilita los selectores de fecha según el estado del CheckBox.
        /// </summary>
        private void ChkCurrentStatus_CheckedChanged(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = !chkCurrentStatus.Checked;
            dtpEndDate.Enabled = !chkCurrentStatus.Checked;
            LoadHistorial(); // Recargar el historial con el nuevo filtro
        }

        /// <summary>
        /// Manejador del evento Click del botón "Filtrar".
        /// Recarga el historial aplicando los filtros de fecha.
        /// </summary>
        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadHistorial();
        }

        /// <summary>
        /// Manejador del evento Click del botón "Limpiar Filtros".
        /// Restablece los filtros y recarga todo el historial.
        /// </summary>
        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Now.AddMonths(-1); // Por defecto, último mes
            dtpEndDate.Value = DateTime.Now;
            chkCurrentStatus.Checked = false;
            LoadHistorial();
        }
    }
}

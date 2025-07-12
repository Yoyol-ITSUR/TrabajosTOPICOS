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
 * Formulario dedicado a la gestión de la información de los residentes.
 * Permite a los (guardias o administradores) ver, crear, actualizar y eliminar registros de residentes,
 * incluyendo la lógica para asociar un residente con un usuario existente o crear un nuevo usuario si es necesario.
*/

// Se tuvo que plantar el diseño dentro de este form
// por que don menso la rego en algo y no supe como solucionarlo
// intentos de diseño a "ciegas" : 55
namespace App_Caseta
{
    public partial class frmResidentes : Form
    {
        private readonly ApiClient _apiClient;

        private string BaseApiUrl = ApiClient.url;

        // Componentes de UI
        private DataGridView dgvResidentes;
        private TextBox txtSearch; // Campo de texto para la búsqueda
        private Button btnSearch; // Botón para iniciar la búsqueda
        private Button btnClearSearch; // Botón para limpiar la búsqueda

        private TextBox txtUserId;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtEmail;
        private TextBox txtTelefono;
        private TextBox txtDireccion;
        private DateTimePicker dtpFechaNacimiento;
        private DateTimePicker dtpFechaRegistro;

        private Button btnCreate;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClearFields;

        /// <summary>
        /// Constructor del formulario de gestión de residentes.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmResidentes()
        {
            InitializeComponent();
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout(); // Cargamos el diseño
            LoadResidentes(); // Cargar residentes al iniciar el formulario
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Gestión de Residentes";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(1220, 830);
            this.MinimumSize = new Size(1220, 830);
            this.MaximumSize = new Size(1220, 830);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            Label lblTitle = new Label
            {
                Text = "Gestión de Residentes",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 20),
                ForeColor = ColorTranslator.FromHtml("#e0e0e0")
            };
            this.Controls.Add(lblTitle);


            // DataGridView para mostrar residentes
            dgvResidentes = new DataGridView
            {
                Location = new Point(50, 90),// Posición de la tabla
                Size = new Size(1100, 250), // Aumenta el tamaño de la tabla para llenar el espacio
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
            dgvResidentes.CellClick += DgvResidentes_CellClick;
            this.Controls.Add(dgvResidentes);

            // Parte de búsqueda
            int searchY = 360; // Posición Y para la fila de búsqueda
            int searchLabelX = 50;
            int searchInputX = searchLabelX + 245;
            int searchInputWidth = 275;
            int searchButtonSpacing = 20;

            Label lblSearch = new Label {
                Text = "Buscar por Nombre/Dirección:",
                Location = new Point(searchLabelX, searchY),
                AutoSize = true, Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(searchInputX, searchY),
                Size = new Size(searchInputWidth, 25),
                Font = new Font("Segoe UI", 9),
                BackColor = ColorTranslator.FromHtml("#3f444a"),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtSearch);

            // Botones de para la búsqueda
            btnSearch = new Button
            {
                Text = "Buscar",
                Location = new Point(txtSearch.Location.X + txtSearch.Width + searchButtonSpacing, searchY - 2),
                Size = new Size(100, 30),
                BackColor = ColorTranslator.FromHtml("#238636"), // Verde
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            btnClearSearch = new Button
            {
                Text = "Limpiar Búsqueda",
                Location = new Point(btnSearch.Location.X + btnSearch.Width + searchButtonSpacing, searchY - 2),
                Size = new Size(150, 30),
                BackColor = ColorTranslator.FromHtml("#444c56"), // Gris
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClearSearch.Click += (s, e) => { txtSearch.Clear(); LoadResidentes(); };
            this.Controls.Add(btnClearSearch);

            // Campos de entrada para CRUD
            int labelX = 50;
            int inputX = 200; // Posición X para los inputs
            int startY = searchY + 60; // Mover los campos de entrada más abajo, debajo de la búsqueda
            int spacingY = 35; // Reducir espaciado vertical
            int inputFieldWidth = 350; // Ancho de los campos de texto
            int dtpWidth = 250; // Ancho de los DateTimePickers

            // Se creo metodos para (juntar) directamente los labels con su txt
            // Usuario ID
            AddLabelAndTextBox(
                "User ID (FK):",
                ref txtUserId,
                labelX,
                inputX,
                startY,
                inputFieldWidth
                );
            txtUserId.ReadOnly = true; // UserId es PK/FK, generalmente no se edita directamente
            txtUserId.BackColor = ColorTranslator.FromHtml("#444c56"); // Fondo de input de solo lectura

            // Nombre
            AddLabelAndTextBox(
                "Nombre:",
                ref txtNombre,
                labelX,
                inputX,
                startY + spacingY,
                inputFieldWidth
                );

            // Apellidos
            AddLabelAndTextBox(
                "Apellido:",
                ref txtApellido,
                labelX,
                inputX,
                startY + spacingY * 2,
                inputFieldWidth
                );

            // Email
            AddLabelAndTextBox(
                "Email:",
                ref txtEmail,
                labelX,
                inputX,
                startY + spacingY * 3,
                inputFieldWidth
                );

            // Telefono
            AddLabelAndTextBox(
                "Teléfono:",
                ref txtTelefono,
                labelX, 
                inputX,
                startY + spacingY * 4,
                inputFieldWidth
                );

            // Direccion
            AddLabelAndTextBox(
                "Dirección:",
                ref txtDireccion,
                labelX,
                inputX, 
                startY + spacingY * 5,
                inputFieldWidth
                );

            // Metodos para juntar el label con el data time picker
            AddLabelAndDateTimePicker(
                "Fecha Nacimiento:",
                ref dtpFechaNacimiento,
                labelX,
                inputX,
                startY + spacingY * 6,
                dtpWidth
                );

            AddLabelAndDateTimePicker("Fecha Registro:",
                ref dtpFechaRegistro,
                labelX,
                inputX,
                startY + spacingY * 7,
                dtpWidth
                );

            dtpFechaRegistro.Enabled = false; // Fecha de registro se establece en la API o al crear
            dtpFechaRegistro.CalendarForeColor = ColorTranslator.FromHtml("#f0f6fc");
            dtpFechaRegistro.CalendarMonthBackground = ColorTranslator.FromHtml("#3f444a");
            dtpFechaRegistro.CalendarTitleBackColor = ColorTranslator.FromHtml("#444c56");
            dtpFechaRegistro.CalendarTitleForeColor = Color.White;
            dtpFechaRegistro.CalendarTrailingForeColor = ColorTranslator.FromHtml("#6a737d");

            // Botones CRUD
            int btnCrudX = 50;
            int btnCrudY = startY + spacingY * 8 + 20; // Posición de los botones
            int btnCrudWidth = 200; // AJUSTADO: Ancho de los botones CRUD
            int btnCrudHeight = 40; // AJUSTADO: Alto de los botones CRUD
            int btnCrudSpacing = 10; // AJUSTADO: Espaciado entre botones

            // Metodos para crear botones más rapido
            // Boton crear
            btnCreate = CreateActionButton("Crear",
                new Point(btnCrudX, btnCrudY),
                ColorTranslator.FromHtml("#238636"),
                btnCrudWidth,
                btnCrudHeight
                );
            btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(btnCreate);

            // Boton actualizar
            btnUpdate = CreateActionButton(
                "Actualizar",
                new Point(btnCrudX + btnCrudWidth + btnCrudSpacing, btnCrudY),
                ColorTranslator.FromHtml("#58a6ff"),
                btnCrudWidth,
                btnCrudHeight
                );
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            // Boton eliminar
            btnDelete = CreateActionButton(
                "Eliminar",
                new Point(btnCrudX + (btnCrudWidth + btnCrudSpacing) * 2, btnCrudY),
                ColorTranslator.FromHtml("#da3633"),
                btnCrudWidth,
                btnCrudHeight
                );
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // Boton limpiar
            btnClearFields = CreateActionButton(
                "Limpiar Campos",
                new Point(btnCrudX + (btnCrudWidth + btnCrudSpacing) * 3, btnCrudY),
                ColorTranslator.FromHtml("#444c56"),
                btnCrudWidth,
                btnCrudHeight
                );
            btnClearFields.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClearFields);
        }

        /// <summary>
        /// Crea un botón de acción con estilo.
        /// </summary>
        private Button CreateActionButton(string text, Point location, Color backColor, int width = 150, int height = 35)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, height), // Usa los parámetros de ancho y alto
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Añade una etiqueta y un campo de texto al formulario.
        /// </summary>
        private void AddLabelAndTextBox(string labelText, ref TextBox textBox, int labelX, int inputX, int y, int inputWidth)
        {
            Label label = new Label {
                Text = labelText,
                Location = new Point(labelX, y + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 10), 
                ForeColor = ColorTranslator.FromHtml("#f0f6fc")
            };
            this.Controls.Add(label);
            textBox = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(inputWidth, 25),
                Font = new Font("Segoe UI", 9),
                BackColor = ColorTranslator.FromHtml("#3f444a"),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(textBox);
        }

        /// <summary>
        /// Añade una etiqueta y un selector de fecha al formulario.
        /// </summary>
        private void AddLabelAndDateTimePicker(string labelText, ref DateTimePicker dateTimePicker, int labelX, int inputX, int y, int dtpWidth)
        {
            Label label = new Label {
                Text = labelText,
                Location = new Point(labelX, y + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(label);
            dateTimePicker = new DateTimePicker
            {
                Location = new Point(inputX, y),
                Size = new Size(dtpWidth, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                CalendarMonthBackground = ColorTranslator.FromHtml("#3f444a"),
                CalendarTitleBackColor = ColorTranslator.FromHtml("#444c56"),
                CalendarTitleForeColor = Color.White,
                CalendarTrailingForeColor = ColorTranslator.FromHtml("#6a737d")
            };
            this.Controls.Add(dateTimePicker);
        }

        /// <summary>
        /// Carga la lista de residentes desde la API y la muestra en el DataGridView.
        /// </summary>
        private async void LoadResidentes()
        {
            try
            {
                var residentes = await _apiClient.GetAsync<List<ResidenteDto>>("Residentes");
                dgvResidentes.DataSource = residentes;
                // Ajustar columnas para mejor visualización
                dgvResidentes.Columns["UserId"].HeaderText = "ID Usuario";
                dgvResidentes.Columns["Nombre"].HeaderText = "Nombre";
                dgvResidentes.Columns["Apellido"].HeaderText = "Apellido";
                dgvResidentes.Columns["Email"].HeaderText = "Email";
                dgvResidentes.Columns["Telefono"].HeaderText = "Teléfono";
                dgvResidentes.Columns["Direccion"].HeaderText = "Dirección";
                dgvResidentes.Columns["FechaNacimiento"].HeaderText = "Fecha Nac.";
                dgvResidentes.Columns["FechaRegistro"].HeaderText = "Fecha Reg.";
                // Ocultar la columna de navegación de User, no es necesaria en la vista principal
                if (dgvResidentes.Columns.Contains("User"))
                {
                    dgvResidentes.Columns["User"].Visible = false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al cargar residentes: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar residentes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento CellClick del DataGridView.
        /// Rellena los campos de entrada con los datos del residente seleccionado.
        /// </summary>
        private void DgvResidentes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvResidentes.Rows[e.RowIndex];
                txtUserId.Text = row.Cells["UserId"].Value?.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString();
                txtApellido.Text = row.Cells["Apellido"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtTelefono.Text = row.Cells["Telefono"].Value?.ToString();
                txtDireccion.Text = row.Cells["Direccion"].Value?.ToString();
                if (row.Cells["FechaNacimiento"].Value != null)
                {
                    dtpFechaNacimiento.Value = Convert.ToDateTime(row.Cells["FechaNacimiento"].Value);
                }
                if (row.Cells["FechaRegistro"].Value != null)
                {
                    dtpFechaRegistro.Value = Convert.ToDateTime(row.Cells["FechaRegistro"].Value);
                }
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Buscar".
        /// Filtra la lista de residentes por nombre o dirección.
        /// </summary>
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadResidentes(); // Si el campo de búsqueda está vacío, recargar todos.
                return;
            }

            try
            {
                var allResidentes = await _apiClient.GetAsync<List<ResidenteDto>>("Residentes");
                var filteredResidentes = allResidentes.Where(r =>
                    (r.Nombre != null && r.Nombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Apellido != null && r.Apellido.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Direccion != null && r.Direccion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                dgvResidentes.DataSource = filteredResidentes;
                if (filteredResidentes.Count == 0)
                {
                    MessageBox.Show("No se encontraron residentes con el término de búsqueda.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al buscar residentes: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al buscar residentes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Crear".
        /// Envía los datos del nuevo residente a la API.
        /// </summary>
        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            if (!ValidateResidentFields()) return;

            // Para crear un residente, primero se debe crear un usuario asociado.
            // La API de Residentes espera un UserId existente.
            // Simplificaremos creando un nuevo User genérico antes de crear el Residente.
            int userIdForNewResidente = 0;
            try
            {
                var newUser = new UserDto { 
                    Username = $"{txtNombre.Text.Replace(" ", "")}{txtApellido.Text.Replace(" ", "")}{Guid.NewGuid().ToString().Substring(0, 4)}".ToLower(),
                    Password = "defaultpassword" };
                var createdUser = await _apiClient.PostAsync<UserDto>("Users", newUser);
                userIdForNewResidente = createdUser.Id;
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error al crear usuario para el residente: {httpEx.Message}", "Error de Creación de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al crear usuario para el residente: {ex.Message}", "Error de Creación de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ahora creamos al residente
            var newResidente = new ResidenteDto
            {
                UserId = userIdForNewResidente, // Asignar el ID del usuario recién creado
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Email = txtEmail.Text,
                Telefono = txtTelefono.Text,
                Direccion = txtDireccion.Text,
                FechaNacimiento = dtpFechaNacimiento.Value,
                FechaRegistro = DateTime.Now // La API podría sobrescribir esto, pero lo enviamos.
            };

            try
            {
                var createdResidente = await _apiClient.PostAsync<ResidenteDto>("Residentes", newResidente);
                MessageBox.Show($"Residente {createdResidente.Nombre} {createdResidente.Apellido} creado con ID de Usuario: {createdResidente.UserId}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadResidentes();
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al crear residente: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al crear residente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Actualizar".
        /// Envía los datos actualizados del residente a la API.
        /// </summary>
        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Selecciona un residente para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidateResidentFields()) return;

            int residenteId = int.Parse(txtUserId.Text);
            var updatedResidente = new ResidenteDto
            {
                UserId = residenteId,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Email = txtEmail.Text,
                Telefono = txtTelefono.Text,
                Direccion = txtDireccion.Text,
                FechaNacimiento = dtpFechaNacimiento.Value,
                FechaRegistro = dtpFechaRegistro.Value // Mantener la fecha de registro original
            };

            try
            {
                await _apiClient.PutAsync($"Residentes/{residenteId}", updatedResidente);
                MessageBox.Show($"Residente con ID {residenteId} actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadResidentes();
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al actualizar residente: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al actualizar residente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Eliminar".
        /// Envía la solicitud de eliminación del residente a la API.
        /// </summary>
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Selecciona un residente para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Estás seguro de que deseas eliminar este residente? Esto también eliminará el usuario asociado.", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int residenteId = int.Parse(txtUserId.Text);
                try
                {
                    await _apiClient.DeleteAsync($"Residentes/{residenteId}");
                    // La eliminación en cascada en la API debería eliminar el usuario asociado.
                    MessageBox.Show($"Residente con ID {residenteId} eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadResidentes();
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"Error de API al eliminar residente: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado al eliminar residente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Valida que los campos obligatorios del residente no estén vacíos.
        /// </summary>
        private bool ValidateResidentFields()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios (Nombre, Apellido, Email, Teléfono, Dirección).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Limpia todos los campos de entrada del formulario.
        /// </summary>
        private void ClearFields()
        {
            txtUserId.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            dtpFechaNacimiento.Value = DateTime.Now;
            dtpFechaRegistro.Value = DateTime.Now;
        }
    }
}

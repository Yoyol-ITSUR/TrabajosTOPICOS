using App_Caseta.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Necesario para Debug.WriteLine
using System.Drawing;
using System.Linq;
using System.Net.Http; // Necesario para HttpRequestException
using System.Text.Json; // Necesario para JsonException
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Formulario para gestionar los invitados. Permite registrar nuevos invitados, 
 * asociarlos a un residente que los invita, y ver, actualizar o eliminar sus datos. 
 * Es el proceso de generación de códigos QR de acceso.
*/

// Se tuvo que plantar el diseño dentro de este form
// por que don menso la rego en algo y no supe como solucionarlo
// intentos de diseño a "ciegas" : perdi el conteo
namespace App_Caseta
{
    public partial class frmInvitados : Form
    {
        private readonly ApiClient _apiClient;
        private string BaseApiUrl = ApiClient.url;

        // Componentes de UI
        private DataGridView dgvInvitados;
        private TextBox txtInvitadoId;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtEmail;
        private TextBox txtTelefono;
        private DateTimePicker dtpFechaVisita;
        private ComboBox cmbResidenteInvitador; // Para seleccionar el residente que invita
        private TextBox txtQrToken; // Para mostrar el token generado

        private Button btnCreate;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClearFields;

        /// <summary>
        /// Constructor del formulario de gestión de invitados.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmInvitados()
        {
            InitializeComponent();
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout();
            LoadResidentesForComboBox(); // Cargar residentes para el ComboBox
            LoadInvitados(); // Cargar invitados al iniciar el formulario
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Gestión de Invitados";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(1200, 800); // Nueva dimensión
            this.MinimumSize = new Size(1200, 800);
            this.MaximumSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            Label lblTitle = new Label
            {
                Text = "Gestión de Invitados",
                Font = new Font("Segoe UI", 30, FontStyle.Bold), // Fuente más grande
                AutoSize = true,
                Location = new Point(50, 20),
                ForeColor = ColorTranslator.FromHtml("#e0e0e0")
            };
            this.Controls.Add(lblTitle);

            // DataGridView para mostrar invitados
            dgvInvitados = new DataGridView
            {
                Location = new Point(50, 90), 
                Size = new Size(1100, 250),
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
            dgvInvitados.CellClick += DgvInvitados_CellClick;
            this.Controls.Add(dgvInvitados);

            // Campos de entrada para CRUD
            int labelX = 50;
            int inputX = 250; // Ajustar posición de inputs
            int startY = 360; // Mover los campos de entrada más arriba
            int spacingY = 35; // Espaciado

            // Caja con label para invitado
            AddLabelAndTextBox(
                "ID Invitado:",
                ref txtInvitadoId,
                labelX,
                inputX,
                startY
                );
            txtInvitadoId.ReadOnly = true;
            txtInvitadoId.BackColor = ColorTranslator.FromHtml("#444c56"); // Fondo de input de solo lectura
            txtInvitadoId.ForeColor = ColorTranslator.FromHtml("#f0f6fc");

            // Caja con label para nombre
            AddLabelAndTextBox(
                "Nombre:",
                ref txtNombre,
                labelX,
                inputX,
                startY + spacingY
                );

            // Caja con label para apellidos
            AddLabelAndTextBox("Apellido:",
                ref txtApellido,
                labelX,
                inputX,
                startY + spacingY * 2
                );

            // Caja con label para email
            AddLabelAndTextBox(
                "Email:",
                ref txtEmail,
                labelX,
                inputX,
                startY + spacingY * 3
                );

            // Caja con label para el telefono
            AddLabelAndTextBox(
                "Teléfono:",
                ref txtTelefono,
                labelX,
                inputX,
                startY + spacingY * 4
                );

            // DTP con label para indicar la fecha de visita
            AddLabelAndDateTimePicker(
                "Fecha Visita:",
                ref dtpFechaVisita,
                labelX,
                inputX,
                startY + spacingY * 5
                );

            // ComboBox para seleccionar Residente Invitador
            Label lblResidente = new Label {
                Text = "Residente Invita:",
                Location = new Point(labelX, startY + spacingY * 6 + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(lblResidente);
            cmbResidenteInvitador = new ComboBox
            {
                Location = new Point(inputX, startY + spacingY * 6),
                Size = new Size(350, 30), // Tamaño ajustado
                DropDownStyle = ComboBoxStyle.DropDownList, // Solo permite seleccionar de la lista
                Font = new Font("Segoe UI", 11),
                BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de ComboBox
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"), // Texto de ComboBox
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(cmbResidenteInvitador);

            // Campo para mostrar el Token QR generado
            Label lblQrToken = new Label {
                Text = "Token QR:",
                Location = new Point(labelX, startY + spacingY * 7 + 3), 
                AutoSize = true,
                Font = new Font("Segoe UI", 12), 
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(lblQrToken);
            txtQrToken = new TextBox
            {
                Location = new Point(inputX, startY + spacingY * 7),
                Size = new Size(350, 30), 
                Font = new Font("Segoe UI", 11),
                ReadOnly = true, // El token se genera en la API, no se edita aquí
                BackColor = ColorTranslator.FromHtml("#444c56"), // Fondo de input de solo lectura
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtQrToken);

            // Botones CRUD
            int btnCrudX = 50;
            int btnCrudY = startY + spacingY * 8 + 30; // Posición de los botones
            int btnCrudSpacing = 280; // Mayor espaciado

            // Metodo que crea un boton con un diseño predefinido
            // Boton crear
            btnCreate = CreateActionButton(
                "Crear",
                new Point(btnCrudX, btnCrudY),
                ColorTranslator.FromHtml("#238636")
                );
            btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(btnCreate);

            // Boton actualizar
            btnUpdate = CreateActionButton(
                "Actualizar",
                new Point(btnCrudX + btnCrudSpacing, btnCrudY),
                ColorTranslator.FromHtml("#58a6ff")
                ); // Azul
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            // Boton borrar
            btnDelete = CreateActionButton(
                "Eliminar",
                new Point(btnCrudX + btnCrudSpacing * 2, btnCrudY), 
                ColorTranslator.FromHtml("#da3633")
                );
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // Boton para limpiar las cajas
            btnClearFields = CreateActionButton(
                "Limpiar Campos",
                new Point(btnCrudX + btnCrudSpacing * 3, btnCrudY),
                ColorTranslator.FromHtml("#444c56")
                );
            btnClearFields.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClearFields);
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
                Size = new Size(250, 45), // Botones más grandes
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Añade una etiqueta y un campo de texto al formulario.
        /// </summary>
        private void AddLabelAndTextBox(string labelText, ref TextBox textBox, int labelX, int inputX, int y)
        {
            // Se crea el label
            Label label = new Label {
                Text = labelText,
                Location = new Point(labelX, y + 3),
                AutoSize = true, 
                Font = new Font("Segoe UI", 12), 
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(label);
            // Y la caja de texto, con las coordenada brindadas
            textBox = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = ColorTranslator.FromHtml("#3f444a"),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(textBox);
        }

        /// <summary>
        /// Añade una etiqueta y un selector de fecha al formulario.
        /// </summary>
        private void AddLabelAndDateTimePicker(string labelText, ref DateTimePicker dateTimePicker, int labelX, int inputX, int y)
        {
            // Agregramos el label
            Label label = new Label {
                Text = labelText,
                Location = new Point(labelX, y + 3),
                AutoSize = true, 
                Font = new Font("Segoe UI", 12),
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(label);
            // Y la caja de texto, con las coordenada brindadas
            dateTimePicker = new DateTimePicker
            {
                Location = new Point(inputX, y),
                Size = new Size(250, 30), // Tamaño ajustado
                Font = new Font("Segoe UI", 11),
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
        /// Carga la lista de residentes desde la API para poblar el ComboBox de invitadores.
        /// </summary>
        private async void LoadResidentesForComboBox()
        {
            try
            {
                // Cargamos de manera asincrona todos los residentes y los cargamos con la DTO correspondiente
                List<ResidenteDto> fetchedResidentes = await _apiClient.GetAsync<List<ResidenteDto>>("Residentes");

                List<ResidenteDto> comboBoxItems = new List<ResidenteDto>();

                // Aquí agregamos los items desde una "combobox" que será otra lista
                // El fin de esto es para poder asignar "nombre" con "id" para que al usuario
                // le salga el nombre del residente, pero su valor sea su id
                comboBoxItems.Add(new ResidenteDto { 
                    UserId = 0, 
                    Nombre = "-- Seleccionar",
                    Apellido = "Residente --" }
                );
                comboBoxItems.AddRange(fetchedResidentes);

                // Asignamos el data source de la lista que creamos al combo box
                cmbResidenteInvitador.DataSource = comboBoxItems;

                // Display memeber es para el texto a mostrar
                cmbResidenteInvitador.DisplayMember = "NombreCompleto";

                // Mientras que el value member es su valor real.
                cmbResidenteInvitador.ValueMember = "UserId";

                cmbResidenteInvitador.SelectedIndex = 0;

                // Anidamos un evento al combo box para debuggear
                cmbResidenteInvitador.SelectedValueChanged += (sender, e) =>
                {
                    // Meramente para comprobar que si seleccione el correcto
                    Debug.WriteLine($"ComboBox SelectedValue: {cmbResidenteInvitador.SelectedValue}");
                    Debug.WriteLine($"ComboBox SelectedItem (type): {cmbResidenteInvitador.SelectedItem?.GetType().Name}");
                    if (cmbResidenteInvitador.SelectedItem is ResidenteDto selectedResidente)
                    {
                        Debug.WriteLine($"ComboBox Selected Residente: {selectedResidente.Nombre} {selectedResidente.Apellido}, ID: {selectedResidente.UserId}");
                    }
                };
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al cargar residentes para ComboBox: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar residentes para ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga la lista de invitados desde la API y la muestra en el DataGridView.
        /// </summary>
        private async void LoadInvitados()
        {
            try
            {
                var invitados = await _apiClient.GetAsync<List<InvitadoDto>>("Invitados");
                dgvInvitados.DataSource = invitados;
                // Ajustar columnas para mejor visualización
                dgvInvitados.Columns["Id"].HeaderText = "ID Invitado";
                dgvInvitados.Columns["Nombre"].HeaderText = "Nombre";
                dgvInvitados.Columns["Apellido"].HeaderText = "Apellido";
                dgvInvitados.Columns["Email"].HeaderText = "Email";
                dgvInvitados.Columns["Telefono"].HeaderText = "Teléfono";
                dgvInvitados.Columns["FechaVisita"].HeaderText = "Fecha Visita";
                dgvInvitados.Columns["Token"].HeaderText = "Token QR";
                dgvInvitados.Columns["ResidenteId"].HeaderText = "ID Residente";
                if (dgvInvitados.Columns.Contains("Residente"))
                {
                    dgvInvitados.Columns["Residente"].Visible = false; // Ocultar objeto completo
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al cargar invitados: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar invitados: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento CellClick del DataGridView.
        /// Rellena los campos de entrada con los datos del invitado seleccionado.
        /// </summary>
        private void DgvInvitados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvInvitados.Rows[e.RowIndex];
                txtInvitadoId.Text = row.Cells["Id"].Value?.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString();
                txtApellido.Text = row.Cells["Apellido"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtTelefono.Text = row.Cells["Telefono"].Value?.ToString();
                if (row.Cells["FechaVisita"].Value != null)
                {
                    dtpFechaVisita.Value = Convert.ToDateTime(row.Cells["FechaVisita"].Value);
                }
                txtQrToken.Text = row.Cells["Token"].Value?.ToString();

                if (row.Cells["ResidenteId"].Value != null)
                {
                    int residenteId = Convert.ToInt32(row.Cells["ResidenteId"].Value);
                    var selectedResidente = (cmbResidenteInvitador.DataSource as List<ResidenteDto>)
                                            ?.FirstOrDefault(r => r.UserId == residenteId);
                    if (selectedResidente != null)
                    {
                        cmbResidenteInvitador.SelectedItem = selectedResidente;
                    }
                    else
                    {
                        cmbResidenteInvitador.SelectedIndex = 0;
                    }
                }
                else
                {
                    cmbResidenteInvitador.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Crear".
        /// Envía los datos del nuevo invitado a la API, la cual generará el token QR.
        /// </summary>
        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            if (!ValidateInvitadoFields()) return;

            var newInvitado = new InvitadoDto
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Email = txtEmail.Text,
                Telefono = txtTelefono.Text,
                FechaVisita = dtpFechaVisita.Value,
                ResidenteId = (cmbResidenteInvitador.SelectedItem as ResidenteDto)?.UserId ?? 0
            };

            Debug.WriteLine($"DEBUG: Creando Invitado. ResidenteId a enviar: {newInvitado.ResidenteId}");

            try
            {
                var createdInvitado = await _apiClient.PostAsync<InvitadoDto>("Invitados", newInvitado);
                MessageBox.Show($"Invitado {createdInvitado.Nombre} {createdInvitado.Apellido} creado con éxito. Token QR: {createdInvitado.Token}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadInvitados();
            }
            catch (HttpRequestException httpEx)
            {
                string errorMessage = $"Error de API al crear invitado: {httpEx.Message}";
                if (httpEx.StatusCode.HasValue)
                {
                    errorMessage = $"Error HTTP {httpEx.StatusCode.Value}: {httpEx.Message}";
                }
                string details = httpEx.Message.Contains("Details:") ? httpEx.Message.Substring(httpEx.Message.IndexOf("Details:")) : "";
                if (!string.IsNullOrEmpty(details))
                {
                    errorMessage += $"\n{details}";
                }
                MessageBox.Show(errorMessage, "Error de Conexión/API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al crear invitado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Actualizar".
        /// Envía los datos actualizados del invitado a la API.
        /// </summary>
        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInvitadoId.Text))
            {
                MessageBox.Show("Selecciona un invitado para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidateInvitadoFields()) return;

            int invitadoId = int.Parse(txtInvitadoId.Text);
            var updatedInvitado = new InvitadoDto
            {
                Id = invitadoId,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Email = txtEmail.Text,
                Telefono = txtTelefono.Text,
                FechaVisita = dtpFechaVisita.Value,
                ResidenteId = (cmbResidenteInvitador.SelectedItem as ResidenteDto)?.UserId ?? 0,
                Token = txtQrToken.Text // Mantener el token existente
            };

            try
            {
                await _apiClient.PutAsync($"Invitados/{invitadoId}", updatedInvitado);
                MessageBox.Show($"Invitado con ID {invitadoId} actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadInvitados();
            }
            catch (HttpRequestException httpEx)
            {
                string errorMessage = $"Error de API al actualizar invitado: {httpEx.Message}";
                if (httpEx.StatusCode.HasValue)
                {
                    errorMessage = $"Error HTTP {httpEx.StatusCode.Value}: {httpEx.Message}";
                }
                string details = httpEx.Message.Contains("Details:") ? httpEx.Message.Substring(httpEx.Message.IndexOf("Details:")) : "";
                if (!string.IsNullOrEmpty(details))
                {
                    errorMessage += $"\n{details}";
                }
                MessageBox.Show(errorMessage, "Error de Conexión/API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al actualizar invitado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Eliminar".
        /// Envía la solicitud de eliminación del invitado a la API.
        /// </summary>
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInvitadoId.Text))
            {
                MessageBox.Show("Selecciona un invitado para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Estás seguro de que deseas eliminar este invitado?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int invitadoId = int.Parse(txtInvitadoId.Text);
                try
                {
                    await _apiClient.DeleteAsync($"Invitados/{invitadoId}");
                    MessageBox.Show($"Invitado con ID {invitadoId} eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadInvitados();
                }
                catch (HttpRequestException httpEx)
                {
                    string errorMessage = $"Error de API al eliminar invitado: {httpEx.Message}";
                    if (httpEx.StatusCode.HasValue)
                    {
                        errorMessage = $"Error HTTP {httpEx.StatusCode.Value}: {httpEx.Message}";
                    }
                    string details = httpEx.Message.Contains("Details:") ? httpEx.Message.Substring(httpEx.Message.IndexOf("Details:")) : "";
                    if (!string.IsNullOrEmpty(details))
                    {
                        errorMessage += $"\n{details}";
                    }
                    MessageBox.Show(errorMessage, "Error de Conexión/API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado al eliminar invitado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Valida que los campos obligatorios del invitado no estén vacíos y que se haya seleccionado un residente válido.
        /// </summary>
        private bool ValidateInvitadoFields()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios (Nombre, Apellido, Email, Teléfono).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ResidenteDto selectedResidente = cmbResidenteInvitador.SelectedItem as ResidenteDto;
            if (selectedResidente == null || selectedResidente.UserId == 0)
            {
                MessageBox.Show("Por favor, selecciona un residente válido de la lista para asociar al invitado.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Limpia todos los campos de entrada del formulario.
        /// </summary>
        private void ClearFields()
        {
            txtInvitadoId.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            dtpFechaVisita.Value = DateTime.Now;
            cmbResidenteInvitador.SelectedIndex = 0; // Seleccionar la opción por defecto (UserId = 0)
            txtQrToken.Clear();
        }
    }
}

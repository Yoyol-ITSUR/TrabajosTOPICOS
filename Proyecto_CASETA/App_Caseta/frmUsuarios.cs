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
 * Formulario para la gestión de los usuarios del sistema (para los guardias).
 * Permite crear, actualizar y eliminar las credenciales de usuario.
*/

// Se tuvo que plantar el diseño dentro de este form
// por que don menso la rego en algo y no supe como solucionarlo
// intentos de diseño a "ciegas" : también perdi el conteo
namespace App_Caseta
{
    public partial class frmUsuarios : Form
    {
        private readonly ApiClient _apiClient;

        private string BaseApiUrl = ApiClient.url;

        // Componentes de UI
        private DataGridView dgvUsers;
        private TextBox txtUserId;
        private TextBox txtUsername;
        private TextBox txtPassword; // Campo para la contraseña (solo para entrada, no se muestra el hash)

        private Button btnCreate;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClearFields;

        /// <summary>
        /// Constructor del formulario de gestión de usuarios.
        /// Inicializa los componentes de la UI y el cliente API.
        /// </summary>
        public frmUsuarios()
        {
            InitializeComponent();
            _apiClient = new ApiClient(BaseApiUrl);
            SetupFormLayout();
            LoadUsers(); // Cargar usuarios al iniciar el formulario
        }

        /// <summary>
        /// Configura el diseño y los controles visuales del formulario.
        /// </summary>
        private void SetupFormLayout()
        {
            this.Text = "SCAR - Gestión de Usuarios";
            this.BackColor = ColorTranslator.FromHtml("#2f363d"); // Fondo oscuro 
            this.ForeColor = ColorTranslator.FromHtml("#f0f6fc"); // Texto claro
            this.Size = new Size(1200, 740); // Nueva dimensión
            this.MinimumSize = new Size(1200, 740);
            this.MaximumSize = new Size(1200, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título
            Label lblTitle = new Label
            {
                Text = "Gestión de Usuarios",
                Font = new Font("Segoe UI", 30, FontStyle.Bold), // Fuente más grande
                AutoSize = true,
                Location = new Point(50, 60),
                ForeColor = ColorTranslator.FromHtml("#e0e0e0") 
            };
            this.Controls.Add(lblTitle);

            // DataGridView para mostrar usuarios
            dgvUsers = new DataGridView
            {
                Location = new Point(50, 140), // Ajustar posición
                Size = new Size(1100, 280), // Ajustar tamaño
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
            dgvUsers.CellClick += DgvUsers_CellClick;
            this.Controls.Add(dgvUsers);

            // Campos de entrada para CRUD
            int labelX = 50;
            int inputX = 250; // Ajustar posición de inputs
            int startY = 450;
            int spacingY = 50; // Mayor espaciado

            // metodo para label y caja de texto para facilitar la union de ambas
            // Campos para el ID
            AddLabelAndTextBox(
                "ID Usuario:",
                ref txtUserId, 
                labelX, 
                inputX, 
                startY
                );
            txtUserId.ReadOnly = true; // ID es solo de lectura
            txtUserId.BackColor = ColorTranslator.FromHtml("#444c56"); // Fondo de input de solo lectura
            txtUserId.ForeColor = ColorTranslator.FromHtml("#f0f6fc");

            // Campos para el nombre
            AddLabelAndTextBox(
                "Nombre de Usuario:", 
                ref txtUsername, 
                labelX,
                inputX,
                startY + spacingY
                );

            // Campos para el apellido
            AddLabelAndTextBox(
                "Contraseña:", 
                ref txtPassword,
                labelX, 
                inputX,
                startY + spacingY * 2
                );
            txtPassword.UseSystemPasswordChar = true; // Ocultar caracteres de contraseña

            // Botones CRUD
            int btnCrudX = 50;
            int btnCrudY = startY + spacingY * 3 + 40; // Ajustar posición
            int btnCrudSpacing = 280; // Mayor espaciado

            // Metodo para boton con diseño pre establecido (solo cambiamos el color)
            // Boton crear
            btnCreate = CreateActionButton(
                "Crear", 
                new Point(btnCrudX, btnCrudY), 
                ColorTranslator.FromHtml("#238636")
                ); // Verde 
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

            // Boton actualizar
            btnDelete = CreateActionButton(
                "Eliminar", 
                new Point(btnCrudX + btnCrudSpacing * 2, btnCrudY),
                ColorTranslator.FromHtml("#da3633")
                ); // Rojo
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // Boton limpiar cajas
            btnClearFields = CreateActionButton(
                "Limpiar Campos",
                new Point(btnCrudX + btnCrudSpacing * 3, btnCrudY),
                ColorTranslator.FromHtml("#444c56")
                ); // Gris 
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
            Label label = new Label { 
                Text = labelText, 
                Location = new Point(labelX, y + 3),
                AutoSize = true, 
                Font = new Font("Segoe UI", 12), 
                ForeColor = ColorTranslator.FromHtml("#f0f6fc") 
            };
            this.Controls.Add(label);
            textBox = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(350, 30), // Tamaño ajustado
                Font = new Font("Segoe UI", 11),
                BackColor = ColorTranslator.FromHtml("#3f444a"), // Fondo de input
                ForeColor = ColorTranslator.FromHtml("#f0f6fc"), // Texto de input
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(textBox);
        }

        /// <summary>
        /// Carga la lista de usuarios desde la API y la muestra en el DataGridView.
        /// </summary>
        private async void LoadUsers()
        {
            try
            {
                var users = await _apiClient.GetAsync<List<UserDto>>("Users");
                dgvUsers.DataSource = users;
                // Ajustar columnas para mejor visualización
                dgvUsers.Columns["Id"].HeaderText = "ID";
                dgvUsers.Columns["Username"].HeaderText = "Nombre de Usuario";
                // Ocultar la columna de contraseña, ya que la API no la devuelve (y no debería).
                if (dgvUsers.Columns.Contains("Password"))
                {
                    dgvUsers.Columns["Password"].Visible = false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de API al cargar usuarios: {httpEx.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento CellClick del DataGridView.
        /// Rellena los campos de entrada con los datos del usuario seleccionado.
        /// </summary>
        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                txtUserId.Text = row.Cells["Id"].Value?.ToString();
                txtUsername.Text = row.Cells["Username"].Value?.ToString();
                txtPassword.Clear(); // Siempre limpiar la contraseña al seleccionar un usuario
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Crear".
        /// Envía los datos del nuevo usuario a la API.
        /// </summary>
        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            if (!ValidateUserFields(isUpdate: false)) return; // Validar para creación

            var newUser = new UserDto
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text // La API se encargará de hashear esto
            };

            try
            {
                var createdUser = await _apiClient.PostAsync<UserDto>("Users", newUser);
                MessageBox.Show($"Usuario '{createdUser.Username}' creado con ID: {createdUser.Id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadUsers();
            }
            catch (HttpRequestException httpEx)
            {
                string errorMessage = $"Error de API al crear usuario: {httpEx.Message}";
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
                MessageBox.Show($"Error inesperado al crear usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Actualizar".
        /// Envía los datos actualizados del usuario a la API.
        /// </summary>
        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Selecciona un usuario para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidateUserFields(isUpdate: true)) return; // Validar para actualización

            int userId = int.Parse(txtUserId.Text);
            var updatedUser = new UserDto
            {
                Id = userId,
                Username = txtUsername.Text,
                Password = txtPassword.Text // Se envía la contraseña, que es obligatoria para la API
            };

            try
            {
                await _apiClient.PutAsync($"Users/{userId}", updatedUser);
                MessageBox.Show($"Usuario con ID {userId} actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadUsers();
            }
            catch (HttpRequestException httpEx)
            {
                string errorMessage = $"Error de API al actualizar usuario: {httpEx.Message}";
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
                MessageBox.Show($"Error inesperado al actualizar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Manejador del evento Click del botón "Eliminar".
        /// Envía la solicitud de eliminación del usuario a la API.
        /// </summary>
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Selecciona un usuario para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Estás seguro de que deseas eliminar este usuario?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int userId = int.Parse(txtUserId.Text);
                try
                {
                    await _apiClient.DeleteAsync($"Users/{userId}");
                    MessageBox.Show($"Usuario con ID {userId} eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadUsers();
                }
                catch (HttpRequestException httpEx)
                {
                    string errorMessage = $"Error de API al eliminar usuario: {httpEx.Message}";
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
                    MessageBox.Show($"Error inesperado al eliminar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Valida que los campos obligatorios del usuario no estén vacíos.
        /// </summary>
        /// <param name="isUpdate">Indica si la validación es para una operación de actualización.</param>
        private bool ValidateUserFields(bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // La contraseña siempre es obligatoria para la creación y ahora también para la actualización.
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                string message = "La contraseña no puede estar vacía.";
                if (isUpdate)
                {
                    message += "\nPor favor, ingresa la contraseña actual o una nueva para actualizar el usuario.";
                }
                MessageBox.Show(message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            txtUsername.Clear();
            txtPassword.Clear();
        }
    }
}

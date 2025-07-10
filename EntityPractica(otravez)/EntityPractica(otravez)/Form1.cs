using EntityPractica_otravez_.Properties;
using Microsoft.EntityFrameworkCore;

namespace EntityPractica_otravez_
{
    public partial class Form1 : Form
    {
        // Contexto para la base de datos
        private WorldContext world;

        // Debido a que esta deshabilitado el binding navigator debido
        // al tipo de proyecto. Se tuvo que improvisar con lo siguiente:
        private BindingNavigator miNavegador;

        // ahora los botones
        private ToolStripButton agregarNuevo;

        private ToolStripButton borrarNuevo;

        private ToolStripButton moverPrimero;

        private ToolStripButton moverAnterior;

        private ToolStripButton moverSiguiente;

        private ToolStripButton moverUltimo;

        private ToolStripButton guardarButton;

        private ToolStripSeparator tsbSeparador01;

        private ToolStripSeparator tsbSeparador02;

        private ToolStripLabel tslContador;

        private ToolStripTextBox tstxtPosicion;


        public Form1()
        {
            InitializeComponent();
            var optionBuilder = new DbContextOptionsBuilder<WorldContext>();
            optionBuilder.UseMySql(Properties.Settings.Default.connStr,
                ServerVersion.AutoDetect(Properties.Settings.Default.connStr));
            world = new WorldContext(optionBuilder.Options);



            // Aquí configuraremos el BindingNavigator
            miNavegador = new BindingNavigator();
            miNavegador.BindingSource = bsCities;


            // Añadiremos la colleción de controles necesarios
            this.Controls.Add(miNavegador);
            miNavegador.Dock = DockStyle.Top;

            // Boton nuevo
            agregarNuevo = new ToolStripButton();
            agregarNuevo.Text = "Nuevo";
            miNavegador.Items.Add(agregarNuevo);
            miNavegador.AddNewItem = agregarNuevo;

            // Boton eliminar
            borrarNuevo = new ToolStripButton();
            borrarNuevo.Text = "Borrar";
            miNavegador.Items.Add(borrarNuevo);
            miNavegador.AddNewItem = borrarNuevo;

            // Separador
            tsbSeparador01 = new ToolStripSeparator();
            miNavegador.Items.Add(tsbSeparador01);

            // Boton Guardar
            guardarButton = new ToolStripButton();
            guardarButton.Text = "Guardar";
            guardarButton.Enabled = true;
            miNavegador.Items.Add(guardarButton);

            // Separador
            tsbSeparador02 = new ToolStripSeparator();
            miNavegador.Items.Add(tsbSeparador02);

            // Boton primero
            moverPrimero = new ToolStripButton();
            moverPrimero.Text = "Primero";
            miNavegador.Items.Add(moverPrimero);
            miNavegador.MoveFirstItem = moverPrimero;

            // Boton anterior
            moverAnterior = new ToolStripButton();
            moverAnterior.Text = "Anterior";
            miNavegador.Items.Add(moverAnterior);
            miNavegador.MovePreviousItem = moverAnterior;

            // Texto
            tstxtPosicion = new ToolStripTextBox();
            tstxtPosicion.AccessibleName = "Posicion";
            tstxtPosicion.AutoSize = false;
            tstxtPosicion.Width = 50;
            miNavegador.Items.Add(tstxtPosicion);
            miNavegador.PositionItem = tstxtPosicion;

            // Etiqueta de conteo
            tslContador = new ToolStripLabel();
            miNavegador.Items.Add(tslContador);
            miNavegador.CountItem = tslContador;

            // Boton siguiente
            moverSiguiente = new ToolStripButton();
            moverSiguiente.Text = "Siguiente";
            miNavegador.Items.Add(moverSiguiente);
            miNavegador.MoveNextItem = moverSiguiente;

            // Boton ultimo
            moverUltimo = new ToolStripButton();
            moverUltimo.Text = "Ultimo";
            miNavegador.Items.Add(moverUltimo);
            miNavegador.MoveLastItem = moverUltimo;

            // Finalmente agregamos el evento
            guardarButton.Click += new EventHandler(guardarCambios);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Como se esta usando .net en vez de .net framework
                // se tuvo que adaptar para cumplir con la practica
                // entonces usaremos metodos que ya conocemos y una dgv
                // para poner los resultados

                // Cargaremos los datos de la tabla ciudad

                await world.Cities.LoadAsync();
                bsCities.DataSource = world.Cities.Local.ToBindingList();
                dgvCities.DataSource = bsCities.DataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error al cargar los datos:" + ex.Message);
            }
        }

        private void guardarCambios(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                bsCities.EndEdit();

                world.SaveChanges();
                MessageBox.Show("Cambios guarddos correctamente en la base de datos");
            }
            catch (DbUpdateException ex) {
                MessageBox.Show($"Hubo un error al guardar cambios en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se genero un error inesperado: {ex.Message}");
            }
        }
    }
}

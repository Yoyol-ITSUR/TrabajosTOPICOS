namespace frmPrincipalCurp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void userControl11_GeneratedCurp(object sender, EventArgs e)
        {
            if (!ctlCURP.CURP.Equals("CURP")) {
                MessageBox.Show(
                    "Persona existente, la curp ha sido capturada.",
                    "CURP ENCONTRADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1
                    );
            }
            else {
                MessageBox.Show(
                    "Curp no encontrada, datos erroneos, o inexistentes",
                    "BUSQUEDA FALLIDA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1
                    );
            }
        }
    }
}

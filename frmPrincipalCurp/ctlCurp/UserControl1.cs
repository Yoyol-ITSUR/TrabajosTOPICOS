using System.ComponentModel;

namespace ctlCurp
{
    [DefaultEvent(nameof(GeneratedCurp))]
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            cmbGender.SelectedIndex = 0;
            cmbState.SelectedIndex = 0;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            string result =
                new clsCurp(
                    txtFirstName.Text,
                    txtMiddleName.Text,
                    txtLastName.Text,
                    dtpBirth.Value,
                    (cmbGender.SelectedIndex == 0 ? 'H' : 'M'),
                    cmbState.Text
                    ).getCURP();


            txtCurp.Text = (result != "" ? result : "CURP");
        }

        private void cmbControler_KeyDown(object sender, KeyEventArgs e) => e.SuppressKeyPress = true;

        // Evento generado (Unicamente si se genera o no curp)
        // Puede servir para validar en caso de que el resultado es
        // "CURP"
        public new event EventHandler? GeneratedCurp
        {
            add => txtCurp.TextChanged += value;
            remove => txtCurp.TextChanged -= value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string Nombres
        {
            get => txtFirstName.Text;
            set => txtFirstName.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string ApellidoP
        {
            get => txtMiddleName.Text;
            set => txtMiddleName.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string ApellidoM
        {
            get => txtLastName.Text;
            set => txtLastName.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string Estado
        {
            get => cmbState.Text;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string Genero
        {
            get => cmbGender.Text;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new DateTime FechaNacimiento
        {
            get => dtpBirth.Value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string CURP
        {
            get => txtCurp.Text;
        }
    }
}

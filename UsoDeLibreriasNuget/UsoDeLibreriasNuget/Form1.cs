using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UsoDeLibreriasNuget
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string accountToJson(Account account)
        {
            return JsonConvert.SerializeObject(account);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string json = accountToJson(new Account
            {
                name = txtNombre.Text,
                email = txtMail.Text,
                DOB = dtpFecha.Value
            });

            lblSerializable.Text = json;
        }
    }
}

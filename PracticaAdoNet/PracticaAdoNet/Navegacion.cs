using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PracticaAdoNet
{
    public partial class Navegacion : Form
    {

        private Form form;

        public Navegacion()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGoToAdd_Click(object sender, EventArgs e)
        {
            form = new NuevoCliente(this);
            form.Show();
            this.Hide();
        }

        private void GoToFillOrCancel_Click(object sender, EventArgs e)
        {
            form = new LlenarCancelar(this);
            form.Show();
            this.Hide();
        }
    }
}

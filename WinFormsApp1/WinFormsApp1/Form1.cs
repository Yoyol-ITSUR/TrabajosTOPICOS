namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Random random;
        private List<string> icons;
        Label primerElegido;
        Label segundoElegido;

        public Form1()
        {
            // Variable para la aleatoriedad
            random = new Random();

            // Iconos para el juego.
            icons = new List<string>() {
                "!", "!", "N", "N", ",", ",", "k", "k",
                "b", "b", "v", "v", "w", "w", "z", "z"
            };

            InitializeComponent();

            AssignIconsToSquares();
            primerElegido = null;
            segundoElegido = null;
        }

        private void AssignIconsToSquares()
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label icono = control as Label;

                if (icono != null)
                {
                    // Generara un numero aleatorio que equivale a un indice para el icono.
                    int numeroRandom = random.Next(icons.Count);
                    icono.Text = icons[numeroRandom];
                    icono.ForeColor = icono.BackColor;
                    icons.RemoveAt(numeroRandom);

                }

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;

            if (clickedLabel != null) {
                if (clickedLabel.ForeColor == Color.Black) {
                    return;
                }

                if (primerElegido == null) {
                    primerElegido = clickedLabel;
                    primerElegido.ForeColor = Color.Black;
                    return;
                }

                if (segundoElegido == null) {
                    segundoElegido = clickedLabel;
                    segundoElegido.ForeColor = Color.Black;
                }

                CheckForWinner();

                if (primerElegido.Text == segundoElegido.Text) {
                    primerElegido = null;
                    segundoElegido = null;
                    return;
                }

                tmrTiempo.Start();
            }
        }

        private void tmrTiempo_Tick(object sender, EventArgs e)
        {
            tmrTiempo.Stop();

            primerElegido.ForeColor = primerElegido.BackColor;
            segundoElegido.ForeColor = segundoElegido.BackColor;

            primerElegido = null;
            segundoElegido = null;
        }

        private void CheckForWinner() {
            foreach (Control controls in tableLayoutPanel1.Controls) { 
                Label icono = controls as Label;

                if (icono != null) {
                    if (icono.ForeColor == icono.BackColor) {
                        return;
                    }
                }
            }

            MessageBox.Show("Emparejaste todos los iconos!", "FELICIDADES!");
            Close();
        }
    }
}

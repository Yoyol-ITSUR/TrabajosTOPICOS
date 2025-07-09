namespace PracticaAdoNet
{
    partial class Navegacion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            btnGoToAdd = new Button();
            GoToFillOrCancel = new Button();
            btnExit = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(52, 9);
            label1.Name = "label1";
            label1.Size = new Size(139, 20);
            label1.TabIndex = 0;
            label1.Text = "¿Qué deseas hacer?";
            // 
            // btnGoToAdd
            // 
            btnGoToAdd.Location = new Point(47, 43);
            btnGoToAdd.Name = "btnGoToAdd";
            btnGoToAdd.Size = new Size(149, 32);
            btnGoToAdd.TabIndex = 1;
            btnGoToAdd.Text = "Agregar una cuenta";
            btnGoToAdd.UseVisualStyleBackColor = true;
            btnGoToAdd.Click += btnGoToAdd_Click;
            // 
            // GoToFillOrCancel
            // 
            GoToFillOrCancel.Location = new Point(12, 78);
            GoToFillOrCancel.Name = "GoToFillOrCancel";
            GoToFillOrCancel.Size = new Size(222, 32);
            GoToFillOrCancel.TabIndex = 2;
            GoToFillOrCancel.Text = "Rellenar o cancelar un pedido";
            GoToFillOrCancel.UseVisualStyleBackColor = true;
            GoToFillOrCancel.Click += GoToFillOrCancel_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(76, 147);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(94, 32);
            btnExit.TabIndex = 3;
            btnExit.Text = "Salir";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // Navegacion
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(251, 200);
            Controls.Add(btnExit);
            Controls.Add(GoToFillOrCancel);
            Controls.Add(btnGoToAdd);
            Controls.Add(label1);
            Name = "Navegacion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inicio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnGoToAdd;
        private Button GoToFillOrCancel;
        private Button btnExit;
    }
}
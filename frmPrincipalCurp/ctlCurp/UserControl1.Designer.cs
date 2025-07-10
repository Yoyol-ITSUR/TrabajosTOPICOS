namespace ctlCurp
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGenerar = new Button();
            cmbState = new ComboBox();
            cmbGender = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            dtpBirth = new DateTimePicker();
            txtFirstName = new TextBox();
            txtMiddleName = new TextBox();
            txtLastName = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            txtCurp = new TextBox();
            SuspendLayout();
            // 
            // btnGenerar
            // 
            btnGenerar.Location = new Point(24, 148);
            btnGenerar.Name = "btnGenerar";
            btnGenerar.Size = new Size(139, 29);
            btnGenerar.TabIndex = 0;
            btnGenerar.Text = "Generar Curp";
            btnGenerar.UseVisualStyleBackColor = true;
            btnGenerar.Click += btnGenerar_Click;
            // 
            // cmbState
            // 
            cmbState.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            cmbState.Font = new Font("Lucida Console", 12F);
            cmbState.FormattingEnabled = true;
            cmbState.Items.AddRange(new object[] { "AGUASCALIENTES", "BAJA CALIFORNIA", "BAJA CALIFORNIA SUR", "CAMPECHE", "CHIAPAS", "CHIHUAHUA", "COAHUILA", "COLIMA", "DURANGO", "GUANAJUATO", "GUERRERO", "HIDALGO", "JALISCO", "MEXICO", "CIUDAD DE MEXICO", "MICHOACAN", "NAYARIT", "NUEVO LEON", "OAXACA", "PUEBLA", "QUERETARO", "QUINTANA ROO", "SAN LUIS POTOSI", "SINALOA", "SONORA", "TABASCO", "TAMAULIPAS", "TLAXCALA", "VERACRUZ", "YUCATAN", "ZACATECAS", "NACIDO EN EL EXTRANJERO" });
            cmbState.Location = new Point(516, 54);
            cmbState.Name = "cmbState";
            cmbState.Size = new Size(250, 28);
            cmbState.TabIndex = 1;
            cmbState.KeyDown += cmbControler_KeyDown;
            // 
            // cmbGender
            // 
            cmbGender.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            cmbGender.Font = new Font("Lucida Console", 12F);
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "Hombre", "Mujer" });
            cmbGender.Location = new Point(516, 13);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(151, 28);
            cmbGender.TabIndex = 2;
            cmbGender.KeyDown += cmbControler_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 16);
            label1.Name = "label1";
            label1.Size = new Size(83, 20);
            label1.TabIndex = 3;
            label1.Text = "Nombre(s):";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 57);
            label2.Name = "label2";
            label2.Size = new Size(123, 20);
            label2.TabIndex = 4;
            label2.Text = "Apellido Paterno:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 99);
            label3.Name = "label3";
            label3.Size = new Size(129, 20);
            label3.TabIndex = 5;
            label3.Text = "Apellido Materno:";
            // 
            // dtpBirth
            // 
            dtpBirth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            dtpBirth.Font = new Font("Lucida Console", 12F);
            dtpBirth.Format = DateTimePickerFormat.Short;
            dtpBirth.Location = new Point(516, 97);
            dtpBirth.Name = "dtpBirth";
            dtpBirth.Size = new Size(250, 27);
            dtpBirth.TabIndex = 6;
            // 
            // txtFirstName
            // 
            txtFirstName.Font = new Font("Lucida Console", 12F);
            txtFirstName.Location = new Point(92, 13);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(233, 27);
            txtFirstName.TabIndex = 7;
            txtFirstName.TextAlign = HorizontalAlignment.Center;
            // 
            // txtMiddleName
            // 
            txtMiddleName.Font = new Font("Lucida Console", 12F);
            txtMiddleName.Location = new Point(132, 54);
            txtMiddleName.Name = "txtMiddleName";
            txtMiddleName.Size = new Size(193, 27);
            txtMiddleName.TabIndex = 8;
            txtMiddleName.TextAlign = HorizontalAlignment.Center;
            // 
            // txtLastName
            // 
            txtLastName.Font = new Font("Lucida Console", 12F);
            txtLastName.Location = new Point(138, 99);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(187, 27);
            txtLastName.TabIndex = 9;
            txtLastName.TextAlign = HorizontalAlignment.Center;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(443, 16);
            label4.Name = "label4";
            label4.Size = new Size(60, 20);
            label4.TabIndex = 10;
            label4.Text = "Genero:";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(351, 57);
            label5.Name = "label5";
            label5.Size = new Size(159, 20);
            label5.TabIndex = 11;
            label5.Text = "Estado de Nacimiento:";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(358, 102);
            label6.Name = "label6";
            label6.Size = new Size(152, 20);
            label6.TabIndex = 12;
            label6.Text = "Fecha de Nacimiento:";
            // 
            // txtCurp
            // 
            txtCurp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtCurp.Location = new Point(516, 150);
            txtCurp.Name = "txtCurp";
            txtCurp.ReadOnly = true;
            txtCurp.Size = new Size(262, 27);
            txtCurp.TabIndex = 13;
            txtCurp.Text = "CURP";
            txtCurp.TextAlign = HorizontalAlignment.Center;
            // 
            // UserControl1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtCurp);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txtLastName);
            Controls.Add(txtMiddleName);
            Controls.Add(txtFirstName);
            Controls.Add(dtpBirth);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbGender);
            Controls.Add(cmbState);
            Controls.Add(btnGenerar);
            Name = "UserControl1";
            Size = new Size(800, 192);
            Load += UserControl1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGenerar;
        private ComboBox cmbState;
        private ComboBox cmbGender;
        private Label label1;
        private Label label2;
        private Label label3;
        private DateTimePicker dtpBirth;
        private TextBox txtFirstName;
        private TextBox txtMiddleName;
        private TextBox txtLastName;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtCurp;
    }
}

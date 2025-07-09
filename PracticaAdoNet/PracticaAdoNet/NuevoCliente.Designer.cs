namespace PracticaAdoNet
{
    partial class NuevoCliente
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
            groupBox1 = new GroupBox();
            txtCustomerID = new TextBox();
            txtCustomerName = new TextBox();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            label4 = new Label();
            dtpOrderDate = new DateTimePicker();
            numOrderAmount = new NumericUpDown();
            label3 = new Label();
            btnCreateAccount = new Button();
            btnPlaceOrder = new Button();
            btnAddFinish = new Button();
            btnAddAnotherAccount = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOrderAmount).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtCustomerID);
            groupBox1.Controls.Add(txtCustomerName);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(324, 118);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Agregar cuenta";
            // 
            // txtCustomerID
            // 
            txtCustomerID.Location = new Point(179, 71);
            txtCustomerID.Name = "txtCustomerID";
            txtCustomerID.ReadOnly = true;
            txtCustomerID.Size = new Size(93, 27);
            txtCustomerID.TabIndex = 3;
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new Point(152, 29);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new Size(166, 27);
            txtCustomerName.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 74);
            label2.Name = "label2";
            label2.Size = new Size(167, 20);
            label2.TabIndex = 1;
            label2.Text = "Identificador del cliente";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 32);
            label1.Name = "label1";
            label1.Size = new Size(140, 20);
            label1.TabIndex = 0;
            label1.Text = "Nombre del cliente:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(dtpOrderDate);
            groupBox2.Controls.Add(numOrderAmount);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(12, 146);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(324, 125);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Crear pedido";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 77);
            label4.Name = "label4";
            label4.Size = new Size(127, 20);
            label4.TabIndex = 3;
            label4.Text = "Fecha del pedido:";
            // 
            // dtpOrderDate
            // 
            dtpOrderDate.Format = DateTimePickerFormat.Short;
            dtpOrderDate.Location = new Point(152, 72);
            dtpOrderDate.Name = "dtpOrderDate";
            dtpOrderDate.Size = new Size(157, 27);
            dtpOrderDate.TabIndex = 2;
            // 
            // numOrderAmount
            // 
            numOrderAmount.Location = new Point(159, 21);
            numOrderAmount.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numOrderAmount.Name = "numOrderAmount";
            numOrderAmount.Size = new Size(150, 27);
            numOrderAmount.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 23);
            label3.Name = "label3";
            label3.Size = new Size(145, 20);
            label3.TabIndex = 0;
            label3.Text = "Cantidad de pedido:";
            // 
            // btnCreateAccount
            // 
            btnCreateAccount.Location = new Point(359, 58);
            btnCreateAccount.Name = "btnCreateAccount";
            btnCreateAccount.Size = new Size(125, 29);
            btnCreateAccount.TabIndex = 2;
            btnCreateAccount.Text = "Crear cuenta";
            btnCreateAccount.UseVisualStyleBackColor = true;
            btnCreateAccount.Click += btnCreateAccount_Click;
            // 
            // btnPlaceOrder
            // 
            btnPlaceOrder.Location = new Point(359, 196);
            btnPlaceOrder.Name = "btnPlaceOrder";
            btnPlaceOrder.Size = new Size(125, 29);
            btnPlaceOrder.TabIndex = 3;
            btnPlaceOrder.Text = "Realizar pedido";
            btnPlaceOrder.UseVisualStyleBackColor = true;
            btnPlaceOrder.Click += btnPlaceOrder_Click;
            // 
            // btnAddFinish
            // 
            btnAddFinish.Location = new Point(96, 311);
            btnAddFinish.Name = "btnAddFinish";
            btnAddFinish.Size = new Size(94, 29);
            btnAddFinish.TabIndex = 4;
            btnAddFinish.Text = "Finalizar";
            btnAddFinish.UseVisualStyleBackColor = true;
            btnAddFinish.Click += btnAddFinish_Click;
            // 
            // btnAddAnotherAccount
            // 
            btnAddAnotherAccount.Location = new Point(255, 311);
            btnAddAnotherAccount.Name = "btnAddAnotherAccount";
            btnAddAnotherAccount.Size = new Size(159, 29);
            btnAddAnotherAccount.TabIndex = 5;
            btnAddAnotherAccount.Text = "Agregar otra cuenta";
            btnAddAnotherAccount.UseVisualStyleBackColor = true;
            btnAddAnotherAccount.Click += btnAddAnotherAccount_Click;
            // 
            // NuevoCliente
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 373);
            Controls.Add(btnAddAnotherAccount);
            Controls.Add(btnAddFinish);
            Controls.Add(btnPlaceOrder);
            Controls.Add(btnCreateAccount);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "NuevoCliente";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cuenta y pedidos nuevos";
            FormClosing += NuevoCliente_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOrderAmount).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox txtCustomerID;
        private TextBox txtCustomerName;
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
        private DateTimePicker dtpOrderDate;
        private NumericUpDown numOrderAmount;
        private Button btnCreateAccount;
        private Button btnPlaceOrder;
        private Button btnAddFinish;
        private Button btnAddAnotherAccount;
    }
}
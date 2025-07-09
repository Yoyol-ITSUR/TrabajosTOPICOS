namespace PracticaAdoNet
{
    partial class LlenarCancelar
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
            btnFindByOrderID = new Button();
            label1 = new Label();
            txtOrderID = new TextBox();
            label2 = new Label();
            dtpFillDate = new DateTimePicker();
            dgvCustomerOrders = new DataGridView();
            btnCancelOrder = new Button();
            btnFillOrder = new Button();
            btnFinishUpdates = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvCustomerOrders).BeginInit();
            SuspendLayout();
            // 
            // btnFindByOrderID
            // 
            btnFindByOrderID.Location = new Point(340, 5);
            btnFindByOrderID.Name = "btnFindByOrderID";
            btnFindByOrderID.Size = new Size(145, 29);
            btnFindByOrderID.TabIndex = 0;
            btnFindByOrderID.Text = "Buscar pedido";
            btnFindByOrderID.UseVisualStyleBackColor = true;
            btnFindByOrderID.Click += btnFindByOrderID_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(170, 20);
            label1.TabIndex = 1;
            label1.Text = "Identificador de pedido:";
            // 
            // txtOrderID
            // 
            txtOrderID.Location = new Point(188, 6);
            txtOrderID.Name = "txtOrderID";
            txtOrderID.Size = new Size(125, 27);
            txtOrderID.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 68);
            label2.MaximumSize = new Size(290, 0);
            label2.Name = "label2";
            label2.Size = new Size(286, 40);
            label2.TabIndex = 3;
            label2.Text = "Si esta rellenando un pedido, especifique la fecha rellenada";
            // 
            // dtpFillDate
            // 
            dtpFillDate.Format = DateTimePickerFormat.Short;
            dtpFillDate.Location = new Point(316, 76);
            dtpFillDate.Name = "dtpFillDate";
            dtpFillDate.Size = new Size(169, 27);
            dtpFillDate.TabIndex = 4;
            // 
            // dgvCustomerOrders
            // 
            dgvCustomerOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCustomerOrders.Location = new Point(13, 128);
            dgvCustomerOrders.Name = "dgvCustomerOrders";
            dgvCustomerOrders.ReadOnly = true;
            dgvCustomerOrders.RowHeadersVisible = false;
            dgvCustomerOrders.RowHeadersWidth = 51;
            dgvCustomerOrders.Size = new Size(472, 188);
            dgvCustomerOrders.TabIndex = 5;
            // 
            // btnCancelOrder
            // 
            btnCancelOrder.Location = new Point(13, 360);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new Size(129, 29);
            btnCancelOrder.TabIndex = 6;
            btnCancelOrder.Text = "Cancelar pedido";
            btnCancelOrder.UseVisualStyleBackColor = true;
            btnCancelOrder.Click += btnCancelOrder_Click;
            // 
            // btnFillOrder
            // 
            btnFillOrder.Location = new Point(184, 360);
            btnFillOrder.Name = "btnFillOrder";
            btnFillOrder.Size = new Size(124, 29);
            btnFillOrder.TabIndex = 7;
            btnFillOrder.Text = "Rellenar pedido";
            btnFillOrder.UseVisualStyleBackColor = true;
            btnFillOrder.Click += btnFillOrder_Click;
            // 
            // btnFinishUpdates
            // 
            btnFinishUpdates.Location = new Point(391, 360);
            btnFinishUpdates.Name = "btnFinishUpdates";
            btnFinishUpdates.Size = new Size(94, 29);
            btnFinishUpdates.TabIndex = 8;
            btnFinishUpdates.Text = "Finalizar";
            btnFinishUpdates.UseVisualStyleBackColor = true;
            btnFinishUpdates.Click += btnFinishUpdates_Click;
            // 
            // LlenarCancelar
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(503, 415);
            Controls.Add(btnFinishUpdates);
            Controls.Add(btnFillOrder);
            Controls.Add(btnCancelOrder);
            Controls.Add(dgvCustomerOrders);
            Controls.Add(dtpFillDate);
            Controls.Add(label2);
            Controls.Add(txtOrderID);
            Controls.Add(label1);
            Controls.Add(btnFindByOrderID);
            Name = "LlenarCancelar";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rellenar o cancelar un pedido";
            FormClosing += LlenarCancelar_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dgvCustomerOrders).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnFindByOrderID;
        private Label label1;
        private TextBox txtOrderID;
        private Label label2;
        private DateTimePicker dtpFillDate;
        private DataGridView dgvCustomerOrders;
        private Button btnCancelOrder;
        private Button btnFillOrder;
        private Button btnFinishUpdates;
    }
}
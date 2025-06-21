namespace Subprocesos_WinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            bgwProccess1 = new System.ComponentModel.BackgroundWorker();
            groupBox1 = new GroupBox();
            txtInvocar = new TextBox();
            btnInvocar = new Button();
            groupBox2 = new GroupBox();
            txtWorker = new TextBox();
            btnWorker = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // bgwProccess1
            // 
            bgwProccess1.WorkerReportsProgress = true;
            bgwProccess1.DoWork += bgwProccess1_DoWork;
            bgwProccess1.ProgressChanged += bgwProccess1_ProgressChanged;
            bgwProccess1.RunWorkerCompleted += bgwProccess1_RunWorkerCompleted;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtInvocar);
            groupBox1.Controls.Add(btnInvocar);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(333, 95);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ejemplo, Invoke";
            // 
            // txtInvocar
            // 
            txtInvocar.Location = new Point(6, 39);
            txtInvocar.Name = "txtInvocar";
            txtInvocar.ReadOnly = true;
            txtInvocar.Size = new Size(232, 23);
            txtInvocar.TabIndex = 1;
            // 
            // btnInvocar
            // 
            btnInvocar.Location = new Point(244, 39);
            btnInvocar.Name = "btnInvocar";
            btnInvocar.Size = new Size(83, 23);
            btnInvocar.TabIndex = 0;
            btnInvocar.Text = "Subproceso";
            btnInvocar.UseVisualStyleBackColor = true;
            btnInvocar.Click += btnInvocar_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtWorker);
            groupBox2.Controls.Add(btnWorker);
            groupBox2.Location = new Point(351, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(363, 95);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Ejemplo, BackgroundWorker";
            // 
            // txtWorker
            // 
            txtWorker.Location = new Point(6, 40);
            txtWorker.Name = "txtWorker";
            txtWorker.ReadOnly = true;
            txtWorker.Size = new Size(261, 23);
            txtWorker.TabIndex = 1;
            // 
            // btnWorker
            // 
            btnWorker.Location = new Point(273, 40);
            btnWorker.Name = "btnWorker";
            btnWorker.Size = new Size(84, 23);
            btnWorker.TabIndex = 0;
            btnWorker.Text = "Subproceso";
            btnWorker.UseVisualStyleBackColor = true;
            btnWorker.Click += btnWorker_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(727, 450);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker bgwProccess1;
        private GroupBox groupBox1;
        private TextBox txtInvocar;
        private Button btnInvocar;
        private GroupBox groupBox2;
        private TextBox txtWorker;
        private Button btnWorker;
    }
}

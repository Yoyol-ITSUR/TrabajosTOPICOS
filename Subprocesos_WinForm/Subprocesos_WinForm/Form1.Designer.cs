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
            groupBox3 = new GroupBox();
            btnCancel = new Button();
            btnStart = new Button();
            lblResult = new Label();
            bgwProccess2 = new System.ComponentModel.BackgroundWorker();
            groupBox4 = new GroupBox();
            lblResult2 = new Label();
            pbrProgreso = new ProgressBar();
            btnAsyncCancel = new Button();
            btnAsyncStart = new Button();
            nudLimite = new NumericUpDown();
            bgwProccess3 = new System.ComponentModel.BackgroundWorker();
            groupBox5 = new GroupBox();
            btnAsyncLoad = new Button();
            groupBox6 = new GroupBox();
            btnAsyncImage = new Button();
            pictureBox1 = new PictureBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudLimite).BeginInit();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            groupBox1.Location = new Point(14, 16);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(381, 127);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ejemplo, Invoke";
            // 
            // txtInvocar
            // 
            txtInvocar.Location = new Point(7, 52);
            txtInvocar.Margin = new Padding(3, 4, 3, 4);
            txtInvocar.Name = "txtInvocar";
            txtInvocar.ReadOnly = true;
            txtInvocar.Size = new Size(265, 27);
            txtInvocar.TabIndex = 1;
            // 
            // btnInvocar
            // 
            btnInvocar.Location = new Point(279, 52);
            btnInvocar.Margin = new Padding(3, 4, 3, 4);
            btnInvocar.Name = "btnInvocar";
            btnInvocar.Size = new Size(95, 31);
            btnInvocar.TabIndex = 0;
            btnInvocar.Text = "Subproceso";
            btnInvocar.UseVisualStyleBackColor = true;
            btnInvocar.Click += btnInvocar_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtWorker);
            groupBox2.Controls.Add(btnWorker);
            groupBox2.Location = new Point(401, 16);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(415, 127);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Ejemplo, BackgroundWorker";
            // 
            // txtWorker
            // 
            txtWorker.Location = new Point(7, 53);
            txtWorker.Margin = new Padding(3, 4, 3, 4);
            txtWorker.Name = "txtWorker";
            txtWorker.ReadOnly = true;
            txtWorker.Size = new Size(298, 27);
            txtWorker.TabIndex = 1;
            // 
            // btnWorker
            // 
            btnWorker.Location = new Point(312, 53);
            btnWorker.Margin = new Padding(3, 4, 3, 4);
            btnWorker.Name = "btnWorker";
            btnWorker.Size = new Size(96, 31);
            btnWorker.TabIndex = 0;
            btnWorker.Text = "Subproceso";
            btnWorker.UseVisualStyleBackColor = true;
            btnWorker.Click += btnWorker_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnCancel);
            groupBox3.Controls.Add(btnStart);
            groupBox3.Controls.Add(lblResult);
            groupBox3.Location = new Point(14, 150);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(381, 167);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Background 2";
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(279, 107);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(14, 107);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 1;
            btnStart.Text = "Comenzar";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.Location = new Point(154, 66);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(78, 20);
            lblResult.TabIndex = 0;
            lblResult.Text = "Resultado:";
            // 
            // bgwProccess2
            // 
            bgwProccess2.WorkerReportsProgress = true;
            bgwProccess2.WorkerSupportsCancellation = true;
            bgwProccess2.DoWork += bgwProccess2_DoWork;
            bgwProccess2.ProgressChanged += bgwProccess2_ProgressChanged;
            bgwProccess2.RunWorkerCompleted += bgwProccess2_RunWorkerCompleted;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(lblResult2);
            groupBox4.Controls.Add(pbrProgreso);
            groupBox4.Controls.Add(btnAsyncCancel);
            groupBox4.Controls.Add(btnAsyncStart);
            groupBox4.Controls.Add(nudLimite);
            groupBox4.Location = new Point(401, 150);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(418, 167);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "Calculadora Fibonacci";
            // 
            // lblResult2
            // 
            lblResult2.AutoSize = true;
            lblResult2.Location = new Point(106, 28);
            lblResult2.Name = "lblResult2";
            lblResult2.Size = new Size(78, 20);
            lblResult2.TabIndex = 7;
            lblResult2.Text = "Resultado:";
            // 
            // pbrProgreso
            // 
            pbrProgreso.Location = new Point(35, 66);
            pbrProgreso.Name = "pbrProgreso";
            pbrProgreso.Size = new Size(345, 29);
            pbrProgreso.Step = 2;
            pbrProgreso.TabIndex = 6;
            // 
            // btnAsyncCancel
            // 
            btnAsyncCancel.Enabled = false;
            btnAsyncCancel.Location = new Point(286, 107);
            btnAsyncCancel.Name = "btnAsyncCancel";
            btnAsyncCancel.Size = new Size(94, 29);
            btnAsyncCancel.TabIndex = 4;
            btnAsyncCancel.Text = "Cancelar";
            btnAsyncCancel.UseVisualStyleBackColor = true;
            btnAsyncCancel.Click += btnAsyncCancel_Click;
            // 
            // btnAsyncStart
            // 
            btnAsyncStart.Location = new Point(35, 107);
            btnAsyncStart.Name = "btnAsyncStart";
            btnAsyncStart.Size = new Size(94, 29);
            btnAsyncStart.TabIndex = 5;
            btnAsyncStart.Text = "Iniciar";
            btnAsyncStart.UseVisualStyleBackColor = true;
            btnAsyncStart.Click += btnAsyncStart_Click;
            // 
            // nudLimite
            // 
            nudLimite.Location = new Point(35, 26);
            nudLimite.Maximum = new decimal(new int[] { 91, 0, 0, 0 });
            nudLimite.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLimite.Name = "nudLimite";
            nudLimite.Size = new Size(65, 27);
            nudLimite.TabIndex = 4;
            nudLimite.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // bgwProccess3
            // 
            bgwProccess3.WorkerReportsProgress = true;
            bgwProccess3.WorkerSupportsCancellation = true;
            bgwProccess3.DoWork += bgwProccess3_DoWork;
            bgwProccess3.ProgressChanged += bgwProccess3_ProgressChanged;
            bgwProccess3.RunWorkerCompleted += bgwProccess3_RunWorkerCompleted;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btnAsyncLoad);
            groupBox5.Location = new Point(14, 323);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(250, 125);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "Sonido";
            // 
            // btnAsyncLoad
            // 
            btnAsyncLoad.Location = new Point(20, 59);
            btnAsyncLoad.Name = "btnAsyncLoad";
            btnAsyncLoad.Size = new Size(212, 29);
            btnAsyncLoad.TabIndex = 0;
            btnAsyncLoad.Text = "Cargar y Reproducir sonido";
            btnAsyncLoad.UseVisualStyleBackColor = true;
            btnAsyncLoad.Click += btnAsyncLoad_Click;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(btnAsyncImage);
            groupBox6.Controls.Add(pictureBox1);
            groupBox6.Location = new Point(270, 323);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(549, 265);
            groupBox6.TabIndex = 5;
            groupBox6.TabStop = false;
            groupBox6.Text = "Imagen";
            // 
            // btnAsyncImage
            // 
            btnAsyncImage.Location = new Point(443, 126);
            btnAsyncImage.Name = "btnAsyncImage";
            btnAsyncImage.Size = new Size(94, 29);
            btnAsyncImage.TabIndex = 1;
            btnAsyncImage.Text = "Cargar";
            btnAsyncImage.UseVisualStyleBackColor = true;
            btnAsyncImage.Click += btnAsyncImage_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(6, 26);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(420, 233);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.LoadCompleted += pictureBox1_LoadCompleted;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(831, 600);
            Controls.Add(groupBox6);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudLimite).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private GroupBox groupBox3;
        private Button btnCancel;
        private Button btnStart;
        private Label lblResult;
        private System.ComponentModel.BackgroundWorker bgwProccess2;
        private GroupBox groupBox4;
        private ProgressBar pbrProgreso;
        private Button btnAsyncCancel;
        private Button btnAsyncStart;
        private NumericUpDown nudLimite;
        private Label lblResult2;
        private System.ComponentModel.BackgroundWorker bgwProccess3;
        private GroupBox groupBox5;
        private Button btnAsyncLoad;
        private GroupBox groupBox6;
        private Button btnAsyncImage;
        private PictureBox pictureBox1;
    }
}

namespace EntityPractica_otravez_
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
            components = new System.ComponentModel.Container();
            dgvCities = new DataGridView();
            bsCities = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dgvCities).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsCities).BeginInit();
            SuspendLayout();
            // 
            // dgvCities
            // 
            dgvCities.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCities.Location = new Point(12, 90);
            dgvCities.Name = "dgvCities";
            dgvCities.RowHeadersWidth = 51;
            dgvCities.Size = new Size(776, 335);
            dgvCities.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvCities);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCities).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsCities).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvCities;
        private BindingSource bsCities;
    }
}

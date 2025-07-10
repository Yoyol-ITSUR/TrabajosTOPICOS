namespace frmPrincipalCurp
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
            ctlCURP = new ctlCurp.UserControl1();
            SuspendLayout();
            // 
            // ctlCURP
            // 
            ctlCURP.ApellidoM = "";
            ctlCURP.ApellidoP = "";
            ctlCURP.Location = new Point(0, 0);
            ctlCURP.Name = "ctlCURP";
            ctlCURP.Nombres = "";
            ctlCURP.Size = new Size(790, 200);
            ctlCURP.TabIndex = 0;
            ctlCURP.GeneratedCurp += userControl11_GeneratedCurp;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(772, 203);
            Controls.Add(ctlCURP);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Consultar CURP";
            ResumeLayout(false);
        }

        #endregion

        private ctlCurp.UserControl1 ctlCURP;
    }
}

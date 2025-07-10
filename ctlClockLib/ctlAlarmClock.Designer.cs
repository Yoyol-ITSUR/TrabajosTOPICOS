namespace ctlClockLib
{
    partial class ctlAlarmClock
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            lblAlarm = new Label();
            btnAlarma = new Button();
            SuspendLayout();
            // 
            // lblAlarm
            // 
            lblAlarm.AutoSize = true;
            lblAlarm.Location = new Point(3, 32);
            lblAlarm.Name = "lblAlarm";
            lblAlarm.Size = new Size(65, 20);
            lblAlarm.TabIndex = 1;
            lblAlarm.Text = "¡Alarma!";
            lblAlarm.TextAlign = ContentAlignment.MiddleCenter;
            lblAlarm.Visible = false;
            // 
            // btnAlarma
            // 
            btnAlarma.Location = new Point(74, 35);
            btnAlarma.Name = "btnAlarma";
            btnAlarma.Size = new Size(94, 29);
            btnAlarma.TabIndex = 2;
            btnAlarma.Text = "Deshabilitar";
            btnAlarma.UseVisualStyleBackColor = true;
            btnAlarma.Click += btnAlarma_Click;
            // 
            // ctlAlarmClock
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnAlarma);
            Controls.Add(lblAlarm);
            Name = "ctlAlarmClock";
            Controls.SetChildIndex(lblAlarm, 0);
            Controls.SetChildIndex(btnAlarma, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAlarm;
        private Button btnAlarma;
    }
}

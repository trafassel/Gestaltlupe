namespace Fractrace.Gui
{
    partial class SettingsControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbDeleteCacheAutomatically = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbDeleteCacheAutomatically);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.panel1.Size = new System.Drawing.Size(389, 32);
            this.panel1.TabIndex = 0;
            // 
            // cbDeleteCacheAutomatically
            // 
            this.cbDeleteCacheAutomatically.AutoSize = true;
            this.cbDeleteCacheAutomatically.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDeleteCacheAutomatically.Location = new System.Drawing.Point(0, 5);
            this.cbDeleteCacheAutomatically.Name = "cbDeleteCacheAutomatically";
            this.cbDeleteCacheAutomatically.Size = new System.Drawing.Size(384, 22);
            this.cbDeleteCacheAutomatically.TabIndex = 1;
            this.cbDeleteCacheAutomatically.Text = "Delete Cache Automatically";
            this.cbDeleteCacheAutomatically.UseVisualStyleBackColor = true;
            this.cbDeleteCacheAutomatically.CheckedChanged += new System.EventHandler(this.cbDeleteCacheAutomatically_CheckedChanged);
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(389, 150);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbDeleteCacheAutomatically;
    }
}

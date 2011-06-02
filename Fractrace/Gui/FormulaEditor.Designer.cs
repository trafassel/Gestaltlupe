namespace Fractrace {
    partial class FormulaEditor {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
          this.panel1 = new System.Windows.Forms.Panel();
          this.tbErrors = new System.Windows.Forms.TextBox();
          this.panel2 = new System.Windows.Forms.Panel();
          this.btnShrink = new System.Windows.Forms.Button();
          this.tbSource = new System.Windows.Forms.RichTextBox();
          this.panel1.SuspendLayout();
          this.panel2.SuspendLayout();
          this.SuspendLayout();
          // 
          // panel1
          // 
          this.panel1.Controls.Add(this.tbErrors);
          this.panel1.Controls.Add(this.panel2);
          this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel1.Location = new System.Drawing.Point(0, 209);
          this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.panel1.Name = "panel1";
          this.panel1.Size = new System.Drawing.Size(338, 178);
          this.panel1.TabIndex = 0;
          // 
          // tbErrors
          // 
          this.tbErrors.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tbErrors.ForeColor = System.Drawing.Color.Red;
          this.tbErrors.Location = new System.Drawing.Point(0, 0);
          this.tbErrors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.tbErrors.Multiline = true;
          this.tbErrors.Name = "tbErrors";
          this.tbErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
          this.tbErrors.Size = new System.Drawing.Size(338, 151);
          this.tbErrors.TabIndex = 1;
          // 
          // panel2
          // 
          this.panel2.Controls.Add(this.btnShrink);
          this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel2.Location = new System.Drawing.Point(0, 151);
          this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.panel2.Name = "panel2";
          this.panel2.Size = new System.Drawing.Size(338, 27);
          this.panel2.TabIndex = 0;
          // 
          // btnShrink
          // 
          this.btnShrink.Dock = System.Windows.Forms.DockStyle.Left;
          this.btnShrink.Location = new System.Drawing.Point(0, 0);
          this.btnShrink.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.btnShrink.Name = "btnShrink";
          this.btnShrink.Size = new System.Drawing.Size(28, 27);
          this.btnShrink.TabIndex = 0;
          this.btnShrink.Text = "^";
          this.btnShrink.UseVisualStyleBackColor = true;
          this.btnShrink.Click += new System.EventHandler(this.btnShrink_Click);
          // 
          // tbSource
          // 
          this.tbSource.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tbSource.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.tbSource.Location = new System.Drawing.Point(0, 0);
          this.tbSource.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.tbSource.Multiline = true;
          this.tbSource.Name = "tbSource";
          this.tbSource.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
          this.tbSource.Size = new System.Drawing.Size(338, 209);
          this.tbSource.TabIndex = 1;
          this.tbSource.TextChanged += new System.EventHandler(this.tbSource_TextChanged);
          // 
          // FormulaEditor
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.tbSource);
          this.Controls.Add(this.panel1);
          this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
          this.Name = "FormulaEditor";
          this.Size = new System.Drawing.Size(338, 387);
          this.panel1.ResumeLayout(false);
          this.panel1.PerformLayout();
          this.panel2.ResumeLayout(false);
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
       // private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.RichTextBox tbSource;
        private System.Windows.Forms.TextBox tbErrors;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnShrink;
    }
}

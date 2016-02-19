namespace Fractrace.Gui
{
    partial class ExportResultDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblExportText = new System.Windows.Forms.Label();
            this.cbOpenInBrowser = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 33);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(184, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(164, 33);
            this.panel4.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cbOpenInBrowser);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(184, 33);
            this.panel3.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblExportText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(348, 42);
            this.panel2.TabIndex = 1;
            // 
            // lblExportText
            // 
            this.lblExportText.AutoSize = true;
            this.lblExportText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExportText.Location = new System.Drawing.Point(10, 10);
            this.lblExportText.Name = "lblExportText";
            this.lblExportText.Size = new System.Drawing.Size(0, 13);
            this.lblExportText.TabIndex = 0;
            // 
            // cbOpenInBrowser
            // 
            this.cbOpenInBrowser.AutoSize = true;
            this.cbOpenInBrowser.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbOpenInBrowser.Location = new System.Drawing.Point(5, 5);
            this.cbOpenInBrowser.Name = "cbOpenInBrowser";
            this.cbOpenInBrowser.Size = new System.Drawing.Size(105, 23);
            this.cbOpenInBrowser.TabIndex = 0;
            this.cbOpenInBrowser.Text = "Open In Browser";
            this.cbOpenInBrowser.UseVisualStyleBackColor = true;
            this.cbOpenInBrowser.CheckedChanged += new System.EventHandler(this.cbOpenInBrowser_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(86, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ExportResultDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 75);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ExportResultDialog";
            this.Text = "Surface is exported";
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbOpenInBrowser;
        private System.Windows.Forms.Label lblExportText;
    }
}
namespace Fractrace.Basic {
  partial class DataViewElement {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.panel1 = new System.Windows.Forms.Panel();
      this.lblName = new System.Windows.Forms.Label();
      this.pnlEdit = new System.Windows.Forms.Panel();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.lblName);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new System.Windows.Forms.Padding(5);
      this.panel1.Size = new System.Drawing.Size(251, 97);
      this.panel1.TabIndex = 0;
      // 
      // lblName
      // 
      this.lblName.AutoSize = true;
      this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblName.Location = new System.Drawing.Point(5, 5);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(46, 17);
      this.lblName.TabIndex = 0;
      this.lblName.Text = "label1";
      // 
      // pnlEdit
      // 
      this.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlEdit.Location = new System.Drawing.Point(251, 0);
      this.pnlEdit.Name = "pnlEdit";
      this.pnlEdit.Size = new System.Drawing.Size(95, 97);
      this.pnlEdit.TabIndex = 1;
      // 
      // DataViewElement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pnlEdit);
      this.Controls.Add(this.panel1);
      this.Name = "DataViewElement";
      this.Size = new System.Drawing.Size(346, 97);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    protected System.Windows.Forms.Panel pnlEdit;
    private System.Windows.Forms.Label lblName;
  }
}

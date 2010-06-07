namespace Fractrace.Animation {
    partial class AnimationControl {
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
          this.splitContainer1 = new System.Windows.Forms.SplitContainer();
          this.pnlSteps = new System.Windows.Forms.Panel();
          this.splitContainer2 = new System.Windows.Forms.SplitContainer();
          this.pnlAnimationEntries = new System.Windows.Forms.Panel();
          this.tbAnimationDescription = new System.Windows.Forms.TextBox();
          this.panel2 = new System.Windows.Forms.Panel();
          this.btnDelete = new System.Windows.Forms.Button();
          this.btnAddRow = new System.Windows.Forms.Button();
          this.pnlPreview = new System.Windows.Forms.Panel();
          this.panel4 = new System.Windows.Forms.Panel();
          this.panel7 = new System.Windows.Forms.Panel();
          this.panel11 = new System.Windows.Forms.Panel();
          this.panel10 = new System.Windows.Forms.Panel();
          this.tbSize = new System.Windows.Forms.TextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.panel6 = new System.Windows.Forms.Panel();
          this.panel9 = new System.Windows.Forms.Panel();
          this.lblAnimationProgress = new System.Windows.Forms.Label();
          this.panel8 = new System.Windows.Forms.Panel();
          this.btnPreview = new System.Windows.Forms.Button();
          this.btnStop = new System.Windows.Forms.Button();
          this.btnStart = new System.Windows.Forms.Button();
          this.panel1 = new System.Windows.Forms.Panel();
          ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
          this.splitContainer1.Panel2.SuspendLayout();
          this.splitContainer1.SuspendLayout();
          this.pnlSteps.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
          this.splitContainer2.Panel1.SuspendLayout();
          this.splitContainer2.Panel2.SuspendLayout();
          this.splitContainer2.SuspendLayout();
          this.pnlAnimationEntries.SuspendLayout();
          this.panel2.SuspendLayout();
          this.panel4.SuspendLayout();
          this.panel7.SuspendLayout();
          this.panel10.SuspendLayout();
          this.panel6.SuspendLayout();
          this.panel9.SuspendLayout();
          this.panel8.SuspendLayout();
          this.SuspendLayout();
          // 
          // splitContainer1
          // 
          this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.splitContainer1.Location = new System.Drawing.Point(0, 0);
          this.splitContainer1.Name = "splitContainer1";
          // 
          // splitContainer1.Panel2
          // 
          this.splitContainer1.Panel2.Controls.Add(this.pnlSteps);
          this.splitContainer1.Panel2.Controls.Add(this.panel1);
          this.splitContainer1.Size = new System.Drawing.Size(598, 300);
          this.splitContainer1.SplitterDistance = 25;
          this.splitContainer1.TabIndex = 0;
          // 
          // pnlSteps
          // 
          this.pnlSteps.Controls.Add(this.splitContainer2);
          this.pnlSteps.Dock = System.Windows.Forms.DockStyle.Fill;
          this.pnlSteps.Location = new System.Drawing.Point(0, 0);
          this.pnlSteps.Name = "pnlSteps";
          this.pnlSteps.Size = new System.Drawing.Size(569, 288);
          this.pnlSteps.TabIndex = 1;
          // 
          // splitContainer2
          // 
          this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
          this.splitContainer2.Location = new System.Drawing.Point(0, 0);
          this.splitContainer2.Name = "splitContainer2";
          // 
          // splitContainer2.Panel1
          // 
          this.splitContainer2.Panel1.Controls.Add(this.pnlAnimationEntries);
          this.splitContainer2.Panel1.Controls.Add(this.panel2);
          // 
          // splitContainer2.Panel2
          // 
          this.splitContainer2.Panel2.Controls.Add(this.pnlPreview);
          this.splitContainer2.Panel2.Controls.Add(this.panel4);
          this.splitContainer2.Size = new System.Drawing.Size(569, 288);
          this.splitContainer2.SplitterDistance = 230;
          this.splitContainer2.TabIndex = 0;
          // 
          // pnlAnimationEntries
          // 
          this.pnlAnimationEntries.Controls.Add(this.tbAnimationDescription);
          this.pnlAnimationEntries.Dock = System.Windows.Forms.DockStyle.Fill;
          this.pnlAnimationEntries.Location = new System.Drawing.Point(0, 0);
          this.pnlAnimationEntries.Name = "pnlAnimationEntries";
          this.pnlAnimationEntries.Size = new System.Drawing.Size(230, 266);
          this.pnlAnimationEntries.TabIndex = 2;
          // 
          // tbAnimationDescription
          // 
          this.tbAnimationDescription.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tbAnimationDescription.Location = new System.Drawing.Point(0, 0);
          this.tbAnimationDescription.Multiline = true;
          this.tbAnimationDescription.Name = "tbAnimationDescription";
          this.tbAnimationDescription.Size = new System.Drawing.Size(230, 266);
          this.tbAnimationDescription.TabIndex = 0;
          // 
          // panel2
          // 
          this.panel2.Controls.Add(this.btnDelete);
          this.panel2.Controls.Add(this.btnAddRow);
          this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel2.Location = new System.Drawing.Point(0, 266);
          this.panel2.Name = "panel2";
          this.panel2.Size = new System.Drawing.Size(230, 22);
          this.panel2.TabIndex = 0;
          // 
          // btnDelete
          // 
          this.btnDelete.Location = new System.Drawing.Point(0, 0);
          this.btnDelete.Name = "btnDelete";
          this.btnDelete.Size = new System.Drawing.Size(75, 23);
          this.btnDelete.TabIndex = 0;
          this.btnDelete.Text = "+";
          this.btnDelete.Click += new System.EventHandler(this.btnAddRow_Click);
          // 
          // btnAddRow
          // 
          this.btnAddRow.Dock = System.Windows.Forms.DockStyle.Left;
          this.btnAddRow.Location = new System.Drawing.Point(0, 0);
          this.btnAddRow.Name = "btnAddRow";
          this.btnAddRow.Size = new System.Drawing.Size(32, 22);
          this.btnAddRow.TabIndex = 0;
          this.btnAddRow.Text = "+";
          this.btnAddRow.UseVisualStyleBackColor = true;
          this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
          // 
          // pnlPreview
          // 
          this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
          this.pnlPreview.Location = new System.Drawing.Point(0, 0);
          this.pnlPreview.Name = "pnlPreview";
          this.pnlPreview.Size = new System.Drawing.Size(335, 185);
          this.pnlPreview.TabIndex = 2;
          // 
          // panel4
          // 
          this.panel4.Controls.Add(this.panel7);
          this.panel4.Controls.Add(this.panel6);
          this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel4.Location = new System.Drawing.Point(0, 185);
          this.panel4.Name = "panel4";
          this.panel4.Size = new System.Drawing.Size(335, 103);
          this.panel4.TabIndex = 1;
          // 
          // panel7
          // 
          this.panel7.Controls.Add(this.panel11);
          this.panel7.Controls.Add(this.panel10);
          this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel7.Location = new System.Drawing.Point(0, 35);
          this.panel7.Name = "panel7";
          this.panel7.Size = new System.Drawing.Size(335, 29);
          this.panel7.TabIndex = 1;
          // 
          // panel11
          // 
          this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
          this.panel11.Location = new System.Drawing.Point(0, 0);
          this.panel11.Name = "panel11";
          this.panel11.Size = new System.Drawing.Size(158, 29);
          this.panel11.TabIndex = 1;
          // 
          // panel10
          // 
          this.panel10.Controls.Add(this.tbSize);
          this.panel10.Controls.Add(this.label1);
          this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
          this.panel10.Location = new System.Drawing.Point(158, 0);
          this.panel10.Name = "panel10";
          this.panel10.Padding = new System.Windows.Forms.Padding(5);
          this.panel10.Size = new System.Drawing.Size(177, 29);
          this.panel10.TabIndex = 0;
          // 
          // tbSize
          // 
          this.tbSize.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tbSize.Location = new System.Drawing.Point(40, 5);
          this.tbSize.Name = "tbSize";
          this.tbSize.Size = new System.Drawing.Size(132, 22);
          this.tbSize.TabIndex = 1;
          this.tbSize.Text = "1";
          this.tbSize.TextChanged += new System.EventHandler(this.tbSize_TextChanged);
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Dock = System.Windows.Forms.DockStyle.Left;
          this.label1.Location = new System.Drawing.Point(5, 5);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(35, 17);
          this.label1.TabIndex = 0;
          this.label1.Text = "Size";
          // 
          // panel6
          // 
          this.panel6.Controls.Add(this.panel9);
          this.panel6.Controls.Add(this.panel8);
          this.panel6.Controls.Add(this.btnStop);
          this.panel6.Controls.Add(this.btnStart);
          this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel6.Location = new System.Drawing.Point(0, 64);
          this.panel6.Name = "panel6";
          this.panel6.Padding = new System.Windows.Forms.Padding(5);
          this.panel6.Size = new System.Drawing.Size(335, 39);
          this.panel6.TabIndex = 0;
          // 
          // panel9
          // 
          this.panel9.Controls.Add(this.lblAnimationProgress);
          this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
          this.panel9.Location = new System.Drawing.Point(5, 5);
          this.panel9.Name = "panel9";
          this.panel9.Size = new System.Drawing.Size(106, 29);
          this.panel9.TabIndex = 4;
          // 
          // lblAnimationProgress
          // 
          this.lblAnimationProgress.AutoSize = true;
          this.lblAnimationProgress.Dock = System.Windows.Forms.DockStyle.Fill;
          this.lblAnimationProgress.Location = new System.Drawing.Point(0, 0);
          this.lblAnimationProgress.Name = "lblAnimationProgress";
          this.lblAnimationProgress.Size = new System.Drawing.Size(0, 17);
          this.lblAnimationProgress.TabIndex = 3;
          // 
          // panel8
          // 
          this.panel8.Controls.Add(this.btnPreview);
          this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
          this.panel8.Location = new System.Drawing.Point(111, 5);
          this.panel8.Name = "panel8";
          this.panel8.Size = new System.Drawing.Size(81, 29);
          this.panel8.TabIndex = 3;
          // 
          // btnPreview
          // 
          this.btnPreview.Dock = System.Windows.Forms.DockStyle.Right;
          this.btnPreview.Location = new System.Drawing.Point(6, 0);
          this.btnPreview.Name = "btnPreview";
          this.btnPreview.Size = new System.Drawing.Size(75, 29);
          this.btnPreview.TabIndex = 0;
          this.btnPreview.Text = "Preview";
          this.btnPreview.UseVisualStyleBackColor = true;
          this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
          // 
          // btnStop
          // 
          this.btnStop.Dock = System.Windows.Forms.DockStyle.Right;
          this.btnStop.Location = new System.Drawing.Point(192, 5);
          this.btnStop.Name = "btnStop";
          this.btnStop.Size = new System.Drawing.Size(68, 29);
          this.btnStop.TabIndex = 1;
          this.btnStop.Text = "Stop";
          this.btnStop.UseVisualStyleBackColor = true;
          this.btnStop.Visible = false;
          this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
          // 
          // btnStart
          // 
          this.btnStart.Dock = System.Windows.Forms.DockStyle.Right;
          this.btnStart.Location = new System.Drawing.Point(260, 5);
          this.btnStart.Name = "btnStart";
          this.btnStart.Size = new System.Drawing.Size(70, 29);
          this.btnStart.TabIndex = 0;
          this.btnStart.Text = "Start";
          this.btnStart.UseVisualStyleBackColor = true;
          this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
          // 
          // panel1
          // 
          this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel1.Location = new System.Drawing.Point(0, 288);
          this.panel1.Name = "panel1";
          this.panel1.Size = new System.Drawing.Size(569, 12);
          this.panel1.TabIndex = 0;
          // 
          // AnimationControl
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.splitContainer1);
          this.Name = "AnimationControl";
          this.Size = new System.Drawing.Size(598, 300);
          this.splitContainer1.Panel2.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
          this.splitContainer1.ResumeLayout(false);
          this.pnlSteps.ResumeLayout(false);
          this.splitContainer2.Panel1.ResumeLayout(false);
          this.splitContainer2.Panel2.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
          this.splitContainer2.ResumeLayout(false);
          this.pnlAnimationEntries.ResumeLayout(false);
          this.pnlAnimationEntries.PerformLayout();
          this.panel2.ResumeLayout(false);
          this.panel4.ResumeLayout(false);
          this.panel7.ResumeLayout(false);
          this.panel10.ResumeLayout(false);
          this.panel10.PerformLayout();
          this.panel6.ResumeLayout(false);
          this.panel9.ResumeLayout(false);
          this.panel9.PerformLayout();
          this.panel8.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlSteps;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.TextBox tbSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label lblAnimationProgress;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.Panel pnlAnimationEntries;
        private System.Windows.Forms.TextBox tbAnimationDescription;
    }
}

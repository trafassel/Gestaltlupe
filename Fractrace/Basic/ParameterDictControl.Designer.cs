﻿namespace Fractrace.Basic {
    partial class ParameterDictControl {
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
          this.splitContainer1 = new System.Windows.Forms.SplitContainer();
          this.treeView1 = new System.Windows.Forms.TreeView();
          this.dataGridView1 = new System.Windows.Forms.DataGridView();
          this.panel1 = new System.Windows.Forms.Panel();
          this.splitContainer1.Panel1.SuspendLayout();
          this.splitContainer1.Panel2.SuspendLayout();
          this.splitContainer1.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
          this.panel1.SuspendLayout();
          this.SuspendLayout();
          // 
          // splitContainer1
          // 
          this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.splitContainer1.Location = new System.Drawing.Point(0, 0);
          this.splitContainer1.Name = "splitContainer1";
          // 
          // splitContainer1.Panel1
          // 
          this.splitContainer1.Panel1.Controls.Add(this.treeView1);
          // 
          // splitContainer1.Panel2
          // 
          this.splitContainer1.Panel2.Controls.Add(this.panel1);
          this.splitContainer1.Size = new System.Drawing.Size(343, 311);
          this.splitContainer1.SplitterDistance = 113;
          this.splitContainer1.TabIndex = 0;
          // 
          // treeView1
          // 
          this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.treeView1.Location = new System.Drawing.Point(0, 0);
          this.treeView1.Name = "treeView1";
          this.treeView1.Size = new System.Drawing.Size(113, 311);
          this.treeView1.TabIndex = 0;
          this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
          // 
          // dataGridView1
          // 
          this.dataGridView1.AllowUserToAddRows = false;
          this.dataGridView1.AllowUserToDeleteRows = false;
          this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
          this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
          this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.dataGridView1.Location = new System.Drawing.Point(0, 0);
          this.dataGridView1.MultiSelect = false;
          this.dataGridView1.Name = "dataGridView1";
          this.dataGridView1.RowHeadersVisible = false;
          this.dataGridView1.RowHeadersWidth = 300;
          this.dataGridView1.RowTemplate.Height = 24;
          this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
          this.dataGridView1.Size = new System.Drawing.Size(226, 311);
          this.dataGridView1.TabIndex = 0;
          this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
          // 
          // panel1
          // 
          this.panel1.Controls.Add(this.dataGridView1);
          this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.panel1.Location = new System.Drawing.Point(0, 0);
          this.panel1.Name = "panel1";
          this.panel1.Size = new System.Drawing.Size(226, 311);
          this.panel1.TabIndex = 1;
          // 
          // ParameterDictControl
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.splitContainer1);
          this.Name = "ParameterDictControl";
          this.Size = new System.Drawing.Size(343, 311);
          this.splitContainer1.Panel1.ResumeLayout(false);
          this.splitContainer1.Panel2.ResumeLayout(false);
          this.splitContainer1.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
          this.panel1.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
    }
}

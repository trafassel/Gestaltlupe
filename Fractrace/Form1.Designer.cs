namespace Fractrace {
    partial class Form1 {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
          this.panel1 = new System.Windows.Forms.Panel();
          this.progressBar1 = new System.Windows.Forms.ProgressBar();
          this.btnRepaint = new System.Windows.Forms.Button();
          this.button4 = new System.Windows.Forms.Button();
          this.button3 = new System.Windows.Forms.Button();
          this.panel2 = new System.Windows.Forms.Panel();
          this.panel3 = new System.Windows.Forms.Panel();
          this.pictureBox1 = new System.Windows.Forms.PictureBox();
          this.panel1.SuspendLayout();
          this.panel2.SuspendLayout();
          this.panel3.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
          this.SuspendLayout();
          // 
          // panel1
          // 
          this.panel1.Controls.Add(this.progressBar1);
          this.panel1.Controls.Add(this.btnRepaint);
          this.panel1.Controls.Add(this.button4);
          this.panel1.Controls.Add(this.button3);
          this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.panel1.Location = new System.Drawing.Point(0, 460);
          this.panel1.Name = "panel1";
          this.panel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
          this.panel1.Size = new System.Drawing.Size(687, 42);
          this.panel1.TabIndex = 0;
          // 
          // progressBar1
          // 
          this.progressBar1.Location = new System.Drawing.Point(176, 15);
          this.progressBar1.Name = "progressBar1";
          this.progressBar1.Size = new System.Drawing.Size(432, 17);
          this.progressBar1.TabIndex = 6;
          // 
          // btnRepaint
          // 
          this.btnRepaint.Location = new System.Drawing.Point(626, 13);
          this.btnRepaint.Name = "btnRepaint";
          this.btnRepaint.Size = new System.Drawing.Size(54, 21);
          this.btnRepaint.TabIndex = 5;
          this.btnRepaint.Text = "Repaint";
          this.btnRepaint.UseVisualStyleBackColor = true;
          this.btnRepaint.Click += new System.EventHandler(this.btnRepaint_Click);
          // 
          // button4
          // 
          this.button4.Location = new System.Drawing.Point(98, 13);
          this.button4.Name = "button4";
          this.button4.Size = new System.Drawing.Size(72, 25);
          this.button4.TabIndex = 3;
          this.button4.Text = "Zoom";
          this.button4.UseVisualStyleBackColor = true;
          this.button4.Click += new System.EventHandler(this.button4_Click);
          // 
          // button3
          // 
          this.button3.Location = new System.Drawing.Point(8, 13);
          this.button3.Name = "button3";
          this.button3.Size = new System.Drawing.Size(84, 24);
          this.button3.TabIndex = 2;
          this.button3.Text = "export";
          this.button3.UseVisualStyleBackColor = true;
          this.button3.Click += new System.EventHandler(this.button3_Click);
          // 
          // panel2
          // 
          this.panel2.Controls.Add(this.panel3);
          this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
          this.panel2.Location = new System.Drawing.Point(0, 0);
          this.panel2.Name = "panel2";
          this.panel2.Size = new System.Drawing.Size(687, 460);
          this.panel2.TabIndex = 1;
          // 
          // panel3
          // 
          this.panel3.AutoScroll = true;
          this.panel3.Controls.Add(this.pictureBox1);
          this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
          this.panel3.Location = new System.Drawing.Point(0, 0);
          this.panel3.Name = "panel3";
          this.panel3.Size = new System.Drawing.Size(687, 460);
          this.panel3.TabIndex = 1;
          // 
          // pictureBox1
          // 
          this.pictureBox1.Location = new System.Drawing.Point(0, 0);
          this.pictureBox1.Name = "pictureBox1";
          this.pictureBox1.Size = new System.Drawing.Size(2500, 2500);
          this.pictureBox1.TabIndex = 1;
          this.pictureBox1.TabStop = false;
          this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
          this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
          this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
          // 
          // Form1
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(687, 502);
          this.Controls.Add(this.panel2);
          this.Controls.Add(this.panel1);
          this.Name = "Form1";
          this.Text = "Form1";
          this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
          this.panel1.ResumeLayout(false);
          this.panel2.ResumeLayout(false);
          this.panel3.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnRepaint;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}


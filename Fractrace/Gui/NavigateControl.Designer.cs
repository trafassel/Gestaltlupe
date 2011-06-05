namespace Fractrace {
    partial class NavigateControl {
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
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnTop = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBackwards = new System.Windows.Forms.Button();
            this.btnZoomX = new System.Windows.Forms.Button();
            this.btnZoomY = new System.Windows.Forms.Button();
            this.btnZoomZ = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnZoomYout = new System.Windows.Forms.Button();
            this.btnZoomZout = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbZoomFactor = new System.Windows.Forms.TextBox();
            this.btnRotX = new System.Windows.Forms.Button();
            this.btnRotY = new System.Windows.Forms.Button();
            this.btnRotZ = new System.Windows.Forms.Button();
            this.btnRotZneg = new System.Windows.Forms.Button();
            this.btnRotYneg = new System.Windows.Forms.Button();
            this.btnRotXneg = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAngle = new System.Windows.Forms.TextBox();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnAspect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(17, 77);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(59, 34);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Text = "<-";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(185, 77);
            this.btnRight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(59, 34);
            this.btnRight.TabIndex = 1;
            this.btnRight.Text = "->";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnTop
            // 
            this.btnTop.Location = new System.Drawing.Point(111, 28);
            this.btnTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(34, 59);
            this.btnTop.TabIndex = 2;
            this.btnTop.Text = "/\\";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(111, 100);
            this.btnDown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(34, 59);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "\\/";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(16, 28);
            this.btnForward.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(59, 34);
            this.btnForward.TabIndex = 4;
            this.btnForward.Text = ">  <";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBackwards
            // 
            this.btnBackwards.Location = new System.Drawing.Point(185, 28);
            this.btnBackwards.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBackwards.Name = "btnBackwards";
            this.btnBackwards.Size = new System.Drawing.Size(59, 34);
            this.btnBackwards.TabIndex = 5;
            this.btnBackwards.Text = "< >";
            this.btnBackwards.UseVisualStyleBackColor = true;
            this.btnBackwards.Click += new System.EventHandler(this.btnBackwards_Click);
            // 
            // btnZoomX
            // 
            this.btnZoomX.Location = new System.Drawing.Point(16, 299);
            this.btnZoomX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZoomX.Name = "btnZoomX";
            this.btnZoomX.Size = new System.Drawing.Size(89, 34);
            this.btnZoomX.TabIndex = 6;
            this.btnZoomX.Text = "ZoomX";
            this.btnZoomX.UseVisualStyleBackColor = true;
            this.btnZoomX.Click += new System.EventHandler(this.btnZoomX_Click);
            // 
            // btnZoomY
            // 
            this.btnZoomY.Location = new System.Drawing.Point(111, 299);
            this.btnZoomY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZoomY.Name = "btnZoomY";
            this.btnZoomY.Size = new System.Drawing.Size(89, 34);
            this.btnZoomY.TabIndex = 7;
            this.btnZoomY.Text = "ZoomY";
            this.btnZoomY.UseVisualStyleBackColor = true;
            this.btnZoomY.Click += new System.EventHandler(this.btnZoomY_Click);
            // 
            // btnZoomZ
            // 
            this.btnZoomZ.Location = new System.Drawing.Point(206, 299);
            this.btnZoomZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZoomZ.Name = "btnZoomZ";
            this.btnZoomZ.Size = new System.Drawing.Size(89, 34);
            this.btnZoomZ.TabIndex = 8;
            this.btnZoomZ.Text = "ZoomZ";
            this.btnZoomZ.UseVisualStyleBackColor = true;
            this.btnZoomZ.Click += new System.EventHandler(this.btnZoomZ_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 337);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 34);
            this.button1.TabIndex = 9;
            this.button1.Text = "ZoomXOut";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnZoomYout
            // 
            this.btnZoomYout.Location = new System.Drawing.Point(111, 337);
            this.btnZoomYout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZoomYout.Name = "btnZoomYout";
            this.btnZoomYout.Size = new System.Drawing.Size(89, 34);
            this.btnZoomYout.TabIndex = 10;
            this.btnZoomYout.Text = "ZoomYOut";
            this.btnZoomYout.UseVisualStyleBackColor = true;
            this.btnZoomYout.Click += new System.EventHandler(this.btnZoomYout_Click);
            // 
            // btnZoomZout
            // 
            this.btnZoomZout.Location = new System.Drawing.Point(206, 337);
            this.btnZoomZout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZoomZout.Name = "btnZoomZout";
            this.btnZoomZout.Size = new System.Drawing.Size(89, 34);
            this.btnZoomZout.TabIndex = 11;
            this.btnZoomZout.Text = "ZoomZout";
            this.btnZoomZout.UseVisualStyleBackColor = true;
            this.btnZoomZout.Click += new System.EventHandler(this.btnZoomZout_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(324, 86);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(72, 22);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "6";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(263, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Move";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "Zoom";
            // 
            // tbZoomFactor
            // 
            this.tbZoomFactor.Location = new System.Drawing.Point(324, 131);
            this.tbZoomFactor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbZoomFactor.Name = "tbZoomFactor";
            this.tbZoomFactor.Size = new System.Drawing.Size(72, 22);
            this.tbZoomFactor.TabIndex = 15;
            this.tbZoomFactor.Text = "6";
            this.tbZoomFactor.TextChanged += new System.EventHandler(this.tbZoomFactor_TextChanged);
            // 
            // btnRotX
            // 
            this.btnRotX.Location = new System.Drawing.Point(17, 190);
            this.btnRotX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotX.Name = "btnRotX";
            this.btnRotX.Size = new System.Drawing.Size(59, 34);
            this.btnRotX.TabIndex = 16;
            this.btnRotX.Text = "RotX+";
            this.btnRotX.UseVisualStyleBackColor = true;
            this.btnRotX.Click += new System.EventHandler(this.btnRotX_Click);
            // 
            // btnRotY
            // 
            this.btnRotY.Location = new System.Drawing.Point(101, 190);
            this.btnRotY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotY.Name = "btnRotY";
            this.btnRotY.Size = new System.Drawing.Size(59, 34);
            this.btnRotY.TabIndex = 17;
            this.btnRotY.Text = "RotY+";
            this.btnRotY.UseVisualStyleBackColor = true;
            this.btnRotY.Click += new System.EventHandler(this.btnRotY_Click);
            // 
            // btnRotZ
            // 
            this.btnRotZ.Location = new System.Drawing.Point(181, 190);
            this.btnRotZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotZ.Name = "btnRotZ";
            this.btnRotZ.Size = new System.Drawing.Size(59, 34);
            this.btnRotZ.TabIndex = 18;
            this.btnRotZ.Text = "RotZ+";
            this.btnRotZ.UseVisualStyleBackColor = true;
            this.btnRotZ.Click += new System.EventHandler(this.btnRotZ_Click);
            // 
            // btnRotZneg
            // 
            this.btnRotZneg.Location = new System.Drawing.Point(181, 228);
            this.btnRotZneg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotZneg.Name = "btnRotZneg";
            this.btnRotZneg.Size = new System.Drawing.Size(59, 34);
            this.btnRotZneg.TabIndex = 21;
            this.btnRotZneg.Text = "RotZ-";
            this.btnRotZneg.UseVisualStyleBackColor = true;
            this.btnRotZneg.Click += new System.EventHandler(this.btnRotZneg_Click);
            // 
            // btnRotYneg
            // 
            this.btnRotYneg.Location = new System.Drawing.Point(101, 228);
            this.btnRotYneg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotYneg.Name = "btnRotYneg";
            this.btnRotYneg.Size = new System.Drawing.Size(59, 34);
            this.btnRotYneg.TabIndex = 20;
            this.btnRotYneg.Text = "RotY-";
            this.btnRotYneg.UseVisualStyleBackColor = true;
            this.btnRotYneg.Click += new System.EventHandler(this.btnRotYneg_Click);
            // 
            // btnRotXneg
            // 
            this.btnRotXneg.Location = new System.Drawing.Point(16, 228);
            this.btnRotXneg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRotXneg.Name = "btnRotXneg";
            this.btnRotXneg.Size = new System.Drawing.Size(59, 34);
            this.btnRotXneg.TabIndex = 19;
            this.btnRotXneg.Text = "RotX-";
            this.btnRotXneg.UseVisualStyleBackColor = true;
            this.btnRotXneg.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(261, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 22;
            this.label3.Text = "Angle";
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(324, 219);
            this.tbAngle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(72, 22);
            this.tbAngle.TabIndex = 23;
            this.tbAngle.Text = "10";
            this.tbAngle.TextChanged += new System.EventHandler(this.tbAngle_TextChanged);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(17, 125);
            this.btnZoomIn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(59, 34);
            this.btnZoomIn.TabIndex = 25;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(185, 125);
            this.btnZoomOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(59, 34);
            this.btnZoomOut.TabIndex = 26;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnAspect
            // 
            this.btnAspect.Location = new System.Drawing.Point(307, 301);
            this.btnAspect.Name = "btnAspect";
            this.btnAspect.Size = new System.Drawing.Size(88, 69);
            this.btnAspect.TabIndex = 27;
            this.btnAspect.Text = "Automatic";
            this.btnAspect.UseVisualStyleBackColor = true;
            this.btnAspect.Click += new System.EventHandler(this.btn_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(16, 175);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(378, 2);
            this.panel1.TabIndex = 28;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(17, 280);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(378, 2);
            this.panel2.TabIndex = 29;
            // 
            // NavigateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAspect);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.tbAngle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRotZneg);
            this.Controls.Add(this.btnRotYneg);
            this.Controls.Add(this.btnRotXneg);
            this.Controls.Add(this.btnRotZ);
            this.Controls.Add(this.btnRotY);
            this.Controls.Add(this.btnRotX);
            this.Controls.Add(this.tbZoomFactor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnZoomZout);
            this.Controls.Add(this.btnZoomYout);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnZoomZ);
            this.Controls.Add(this.btnZoomY);
            this.Controls.Add(this.btnZoomX);
            this.Controls.Add(this.btnBackwards);
            this.Controls.Add(this.btnForward);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnTop);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "NavigateControl";
            this.Size = new System.Drawing.Size(435, 404);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnTop;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBackwards;
        private System.Windows.Forms.Button btnZoomX;
        private System.Windows.Forms.Button btnZoomY;
        private System.Windows.Forms.Button btnZoomZ;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnZoomYout;
        private System.Windows.Forms.Button btnZoomZout;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbZoomFactor;
        private System.Windows.Forms.Button btnRotX;
        private System.Windows.Forms.Button btnRotY;
        private System.Windows.Forms.Button btnRotZ;
        private System.Windows.Forms.Button btnRotZneg;
        private System.Windows.Forms.Button btnRotYneg;
        private System.Windows.Forms.Button btnRotXneg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbAngle;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnAspect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}

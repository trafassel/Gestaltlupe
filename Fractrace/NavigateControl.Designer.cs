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
            this.btnAllAngles0 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(77, 94);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(37, 31);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Text = "<-";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(151, 95);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(33, 29);
            this.btnRight.TabIndex = 1;
            this.btnRight.Text = "->";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnTop
            // 
            this.btnTop.Location = new System.Drawing.Point(118, 43);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(31, 45);
            this.btnTop.TabIndex = 2;
            this.btnTop.Text = "/\\  |";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(118, 126);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(31, 55);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "|   \\/";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(23, 42);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(59, 34);
            this.btnForward.TabIndex = 4;
            this.btnForward.Text = ">  <";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBackwards
            // 
            this.btnBackwards.Location = new System.Drawing.Point(191, 46);
            this.btnBackwards.Name = "btnBackwards";
            this.btnBackwards.Size = new System.Drawing.Size(59, 29);
            this.btnBackwards.TabIndex = 5;
            this.btnBackwards.Text = "< >";
            this.btnBackwards.UseVisualStyleBackColor = true;
            this.btnBackwards.Click += new System.EventHandler(this.btnBackwards_Click);
            // 
            // btnZoomX
            // 
            this.btnZoomX.Location = new System.Drawing.Point(12, 193);
            this.btnZoomX.Name = "btnZoomX";
            this.btnZoomX.Size = new System.Drawing.Size(70, 28);
            this.btnZoomX.TabIndex = 6;
            this.btnZoomX.Text = "ZoomX";
            this.btnZoomX.UseVisualStyleBackColor = true;
            this.btnZoomX.Click += new System.EventHandler(this.btnZoomX_Click);
            // 
            // btnZoomY
            // 
            this.btnZoomY.Location = new System.Drawing.Point(97, 193);
            this.btnZoomY.Name = "btnZoomY";
            this.btnZoomY.Size = new System.Drawing.Size(70, 28);
            this.btnZoomY.TabIndex = 7;
            this.btnZoomY.Text = "ZoomY";
            this.btnZoomY.UseVisualStyleBackColor = true;
            this.btnZoomY.Click += new System.EventHandler(this.btnZoomY_Click);
            // 
            // btnZoomZ
            // 
            this.btnZoomZ.Location = new System.Drawing.Point(181, 193);
            this.btnZoomZ.Name = "btnZoomZ";
            this.btnZoomZ.Size = new System.Drawing.Size(69, 27);
            this.btnZoomZ.TabIndex = 8;
            this.btnZoomZ.Text = "ZoomZ";
            this.btnZoomZ.UseVisualStyleBackColor = true;
            this.btnZoomZ.Click += new System.EventHandler(this.btnZoomZ_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 237);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "ZoomXOut";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnZoomYout
            // 
            this.btnZoomYout.Location = new System.Drawing.Point(120, 237);
            this.btnZoomYout.Name = "btnZoomYout";
            this.btnZoomYout.Size = new System.Drawing.Size(102, 23);
            this.btnZoomYout.TabIndex = 10;
            this.btnZoomYout.Text = "ZoomYOut";
            this.btnZoomYout.UseVisualStyleBackColor = true;
            this.btnZoomYout.Click += new System.EventHandler(this.btnZoomYout_Click);
            // 
            // btnZoomZout
            // 
            this.btnZoomZout.Location = new System.Drawing.Point(236, 237);
            this.btnZoomZout.Name = "btnZoomZout";
            this.btnZoomZout.Size = new System.Drawing.Size(91, 22);
            this.btnZoomZout.TabIndex = 11;
            this.btnZoomZout.Text = "ZoomZout";
            this.btnZoomZout.UseVisualStyleBackColor = true;
            this.btnZoomZout.Click += new System.EventHandler(this.btnZoomZout_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(324, 95);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(72, 22);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "6";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(263, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "factor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(308, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "factor";
            // 
            // tbZoomFactor
            // 
            this.tbZoomFactor.Location = new System.Drawing.Point(358, 198);
            this.tbZoomFactor.Name = "tbZoomFactor";
            this.tbZoomFactor.Size = new System.Drawing.Size(72, 22);
            this.tbZoomFactor.TabIndex = 15;
            this.tbZoomFactor.Text = "6";
            this.tbZoomFactor.TextChanged += new System.EventHandler(this.tbZoomFactor_TextChanged);
            // 
            // btnRotX
            // 
            this.btnRotX.Location = new System.Drawing.Point(17, 274);
            this.btnRotX.Name = "btnRotX";
            this.btnRotX.Size = new System.Drawing.Size(65, 23);
            this.btnRotX.TabIndex = 16;
            this.btnRotX.Text = "RotX+";
            this.btnRotX.UseVisualStyleBackColor = true;
            this.btnRotX.Click += new System.EventHandler(this.btnRotX_Click);
            // 
            // btnRotY
            // 
            this.btnRotY.Location = new System.Drawing.Point(88, 275);
            this.btnRotY.Name = "btnRotY";
            this.btnRotY.Size = new System.Drawing.Size(65, 23);
            this.btnRotY.TabIndex = 17;
            this.btnRotY.Text = "RotY+";
            this.btnRotY.UseVisualStyleBackColor = true;
            this.btnRotY.Click += new System.EventHandler(this.btnRotY_Click);
            // 
            // btnRotZ
            // 
            this.btnRotZ.Location = new System.Drawing.Point(159, 274);
            this.btnRotZ.Name = "btnRotZ";
            this.btnRotZ.Size = new System.Drawing.Size(65, 23);
            this.btnRotZ.TabIndex = 18;
            this.btnRotZ.Text = "RotZ+";
            this.btnRotZ.UseVisualStyleBackColor = true;
            this.btnRotZ.Click += new System.EventHandler(this.btnRotZ_Click);
            // 
            // btnRotZneg
            // 
            this.btnRotZneg.Location = new System.Drawing.Point(159, 303);
            this.btnRotZneg.Name = "btnRotZneg";
            this.btnRotZneg.Size = new System.Drawing.Size(65, 23);
            this.btnRotZneg.TabIndex = 21;
            this.btnRotZneg.Text = "RotZ-";
            this.btnRotZneg.UseVisualStyleBackColor = true;
            this.btnRotZneg.Click += new System.EventHandler(this.btnRotZneg_Click);
            // 
            // btnRotYneg
            // 
            this.btnRotYneg.Location = new System.Drawing.Point(88, 304);
            this.btnRotYneg.Name = "btnRotYneg";
            this.btnRotYneg.Size = new System.Drawing.Size(65, 23);
            this.btnRotYneg.TabIndex = 20;
            this.btnRotYneg.Text = "RotY-";
            this.btnRotYneg.UseVisualStyleBackColor = true;
            this.btnRotYneg.Click += new System.EventHandler(this.btnRotYneg_Click);
            // 
            // btnRotXneg
            // 
            this.btnRotXneg.Location = new System.Drawing.Point(17, 304);
            this.btnRotXneg.Name = "btnRotXneg";
            this.btnRotXneg.Size = new System.Drawing.Size(65, 23);
            this.btnRotXneg.TabIndex = 19;
            this.btnRotXneg.Text = "RotX-";
            this.btnRotXneg.UseVisualStyleBackColor = true;
            this.btnRotXneg.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(308, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 22;
            this.label3.Text = "Angle";
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(358, 287);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(72, 22);
            this.tbAngle.TabIndex = 23;
            this.tbAngle.Text = "1";
            this.tbAngle.TextChanged += new System.EventHandler(this.tbAngle_TextChanged);
            // 
            // btnAllAngles0
            // 
            this.btnAllAngles0.Location = new System.Drawing.Point(239, 277);
            this.btnAllAngles0.Name = "btnAllAngles0";
            this.btnAllAngles0.Size = new System.Drawing.Size(71, 43);
            this.btnAllAngles0.TabIndex = 24;
            this.btnAllAngles0.Text = "All Angles 0";
            this.btnAllAngles0.UseVisualStyleBackColor = true;
            this.btnAllAngles0.Click += new System.EventHandler(this.btnAllAngles0_Click);
            // 
            // NavigateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAllAngles0);
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
            this.Name = "NavigateControl";
            this.Size = new System.Drawing.Size(435, 330);
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
        private System.Windows.Forms.Button btnAllAngles0;
    }
}

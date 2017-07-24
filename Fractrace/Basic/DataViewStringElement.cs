using System;

namespace Fractrace.Basic
{
    class DataViewStringElement : DataViewElement
    {

        System.Windows.Forms.TextBox _tbValue = new System.Windows.Forms.TextBox();

        // Used to scale the parameter changes if pus or minus button is pressed.
        private double _amount = 1;

        protected override void PreInit()
        {
            _tbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEdit.Controls.Add(_tbValue);
            _tbValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbValue.Text = _value;
            _oldValue = _value;
            this._tbValue.TextChanged += new EventHandler(tbValue_TextChanged);
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// </summary>
        public override void UpdateElements()
        {
            lock (_updateMutex)
            {
                string newValue = ParameterDict.Current[_name];
                if (_oldValue != newValue)
                {
                    _value = newValue;
                    _dontRaiseElementChangedEvent = true;
                    this._tbValue.Text = _value;
                    _dontRaiseElementChangedEvent = false;
                    _oldValue = newValue;
                }
            }
        }


        /// <summary>
        /// Text change by user.
        /// </summary>
        void tbValue_TextChanged(object sender, EventArgs e)
        {
            ParameterDict.Current.Entries[_name] = _tbValue.Text;
            _oldValue= _tbValue.Text;
            CallElementChanged(_name, _tbValue.Text);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DataViewStringElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DataViewStringElement";
            this.ResumeLayout(false);

        }

        public void AddFixedValueButton(string text)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Text = text;
            button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.DarkGray;
            button.FlatAppearance.BorderSize = 0;
            button.Dock = System.Windows.Forms.DockStyle.Right;
            if (text.Length < 3 || (text.Length == 3 && text.Contains(".")))
            {
                button.Width = 30;
                this.pnlButtons.Width += 30;
            }
            else if (text.Length < 4)
            {
                button.Width = 35;
                this.pnlButtons.Width += 35;
            }
            else if (text.Length < 5)
            {
                button.Width = 40;
                this.pnlButtons.Width += 40;
            }
            else
            {
                button.Width = 60;
                this.pnlButtons.Width += 60;
            }
            _additionalButtonsWidth += button.Width;
            _additionalButtonsWidth += tmpBtnSize;
            button.Click += Button_Click;
            this.pnlButtons.Controls.Add(button);    
        }

        int tmpBtnSize = 0;

        public void AddPlusButton(string value)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Text = "+";
            button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.DarkGray;
            button.FlatAppearance.BorderSize = 0;
            button.Dock = System.Windows.Forms.DockStyle.Right;
            button.Tag = value;
            button.Click += PlusButton_Click;
            button.Width = 30;
            _additionalButtonsWidth += button.Width;
            _additionalButtonsWidth += tmpBtnSize;
            this.pnlButtons.Width += 30;
            this.pnlButtons.Controls.Add(button);
        }

        public void AddAdjustButtons()
        {
            {
                System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                button.Text = ">";
                button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button.ForeColor = System.Drawing.Color.DarkGray;
                button.FlatAppearance.BorderSize = 0;
                button.Dock = System.Windows.Forms.DockStyle.Right;
                button.Click += IncreaseAmount;
                button.Width = 30;
                _additionalButtonsWidth += button.Width;
                _additionalButtonsWidth += tmpBtnSize;
                this.pnlButtons.Width += 30;
                this.pnlButtons.Controls.Add(button);
            }
            {
                System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                button.Text = "<";
                button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button.ForeColor = System.Drawing.Color.DarkGray;
                button.FlatAppearance.BorderSize = 0;
                button.Dock = System.Windows.Forms.DockStyle.Right;
                button.Click += DecreaseAmount;
                button.Width = 30;
                _additionalButtonsWidth += button.Width;
                _additionalButtonsWidth += tmpBtnSize;
                this.pnlButtons.Width += 30;
                this.pnlButtons.Controls.Add(button);
            }
        }

        private void IncreaseAmount(object sender, EventArgs e)
        {
            _amount *= 10.0;
        }

        private void DecreaseAmount(object sender, EventArgs e)
        {
            _amount /= 10.0;
        }

        System.Windows.Forms.Button fillRightButton = null;

        int _additionalButtonsWidth = 0;
        public void AddFillRightButton()
        {
            fillRightButton = new System.Windows.Forms.Button();
            fillRightButton.Text = "";
            fillRightButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            fillRightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            fillRightButton.ForeColor = System.Drawing.Color.DarkGray;
            fillRightButton.FlatAppearance.BorderSize = 0;
            fillRightButton.Dock = System.Windows.Forms.DockStyle.Right;
            fillRightButton.Width = 130;
            this.pnlButtons.Width += 130;
            this.pnlButtons.Controls.Add(fillRightButton);
        }

        void UpdateFillRightSize()
        {
            if(fillRightButton!=null)
            {
                int oldWidth = fillRightButton.Width;
                int newWidth= this.Width - 420- _additionalButtonsWidth+200;
                if (newWidth < 0)
                    newWidth = 0;
                fillRightButton.Width = newWidth;
                this.pnlButtons.Width += fillRightButton.Width-oldWidth;
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            double currentValue = Double.Parse(this._tbValue.Text, ParameterDict.Culture);
            double valueToAdd= Double.Parse(button.Tag.ToString(), ParameterDict.Culture);
            currentValue += _amount*valueToAdd;
            this._tbValue.Text = currentValue.ToString(ParameterDict.Culture);
        }


        public void AddMinusButton(string value)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Text = "-";
            button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.DarkGray;
            button.FlatAppearance.BorderSize = 0;
            button.Dock = System.Windows.Forms.DockStyle.Right;
            button.Tag = value;
            button.Click += MinusButton_Click;
            button.Width = 30;
            _additionalButtonsWidth += button.Width;
            _additionalButtonsWidth += tmpBtnSize;
            this.pnlButtons.Width += 30;
            this.pnlButtons.Controls.Add(button);
        }

        private void MinusButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            double currentValue = Double.Parse(this._tbValue.Text, ParameterDict.Culture);
            double valueToSubtract = Double.Parse(button.Tag.ToString(), ParameterDict.Culture);
            currentValue -= _amount*valueToSubtract;
            this._tbValue.Text = currentValue.ToString(ParameterDict.Culture);
        }


        private void Button_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            this._tbValue.Text = button.Text;
          }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateFillRightSize();
        }
    }
}

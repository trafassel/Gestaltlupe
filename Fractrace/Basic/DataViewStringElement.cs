using System;

namespace Fractrace.Basic
{
    class DataViewStringElement : DataViewElement
    {

        System.Windows.Forms.TextBox _tbValue = new System.Windows.Forms.TextBox();

        protected override void PreInit()
        {
            _tbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEdit.Controls.Add(_tbValue);
            _tbValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbValue.Text = _value;
            this._tbValue.TextChanged += new EventHandler(tbValue_TextChanged);
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// </summary>
        public override void UpdateElements()
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


        /// <summary>
        /// Text change by user.
        /// </summary>
        void tbValue_TextChanged(object sender, EventArgs e)
        {
            ParameterDict.Current.Entries[_name] = _tbValue.Text;
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
            button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.DarkGray;
            button.FlatAppearance.BorderSize = 0;
            button.Dock = System.Windows.Forms.DockStyle.Right;
            if (text.Length < 3 || (text.Length==3 && text.Contains(".")))
            {
                button.Width = 30;
                this.pnlButtons.Width += 32;
            }
            else
            {
                button.Width = 60;
                this.pnlButtons.Width += 64;
            }

            button.Click += Button_Click;
            this.pnlButtons.Controls.Add(button);

            
        }

        private void Button_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            this._tbValue.Text = button.Text;
          }
    }
}

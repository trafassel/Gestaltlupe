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
                this._tbValue.Text = _value;
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
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(260, 0);
            // 
            // DataViewStringElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "DataViewStringElement";
            this.ResumeLayout(false);

        }
    }
}

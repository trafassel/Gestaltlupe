using System;


namespace Fractrace.Basic
{
    class DataViewBoolElement : DataViewElement
    {

        System.Windows.Forms.CheckBox _cbValue = new System.Windows.Forms.CheckBox();

        protected override void PreInit()
        {
            _cbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEdit.Controls.Add(_cbValue);
            this._cbValue.Checked = ParameterDict.Current.GetBool(_name);
            this._cbValue.CheckedChanged += cbValue_CheckedChanged;
        }


        /// <summary>
        /// Value changed by user.
        /// </summary>
        void cbValue_CheckedChanged(object sender, EventArgs e)
        {
            ParameterDict.Current.SetBool(_name, this._cbValue.Checked);
            CallElementChanged(_name, this._cbValue.Checked.ToString());
            _oldValue = this._cbValue.Checked.ToString();
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// </summary>
        public override void UpdateElements()
        {
            string newValue = ParameterDict.Current.GetBool(_name).ToString();
            if (_oldValue != newValue)
            {
                _value = newValue;
                _dontRaiseElementChangedEvent = true;
                this._cbValue.Checked = ParameterDict.Current.GetBool(_name);
                _dontRaiseElementChangedEvent = false;
                _oldValue = newValue;
            }
        }


    }
}

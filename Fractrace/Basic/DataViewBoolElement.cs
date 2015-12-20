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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbValue_CheckedChanged(object sender, EventArgs e)
        {
            ParameterDict.Current.SetBool(_name, this._cbValue.Checked);
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// </summary>
        public override void UpdateElements()
        {
            this._cbValue.Checked = ParameterDict.Current.GetBool(_name);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic
{
    class DataViewBoolElement : DataViewElement
    {

        System.Windows.Forms.CheckBox cbValue = new System.Windows.Forms.CheckBox();

        protected override void PreInit()
        {
            cbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEdit.Controls.Add(cbValue);
            this.cbValue.Checked = Fractrace.Basic.ParameterDict.Exemplar.GetBool(mName);
            this.cbValue.CheckedChanged += cbValue_CheckedChanged;
        }


        /// <summary>
        /// Value changed by user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbValue_CheckedChanged(object sender, EventArgs e)
        {
            Fractrace.Basic.ParameterDict.Exemplar.SetBool(mName, this.cbValue.Checked);
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// </summary>
        public override void UpdateElements()
        {
            this.cbValue.Checked = Fractrace.Basic.ParameterDict.Exemplar.GetBool(mName);
        }


    }
}

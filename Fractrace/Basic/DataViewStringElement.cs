using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic {
  class DataViewStringElement : DataViewElement {


    System.Windows.Forms.TextBox tbValue = new System.Windows.Forms.TextBox();

    protected override void PreInit() {
      tbValue.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlEdit.Controls.Add(tbValue);
      this.tbValue.Text = mValue;
      this.tbValue.TextChanged += new EventHandler(tbValue_TextChanged);
    }


    /// <summary>
    /// Corresponding string value is set from ParameterDict.Exemplar.
    /// </summary>
    public override void UpdateElements() {
      string newValue = ParameterDict.Exemplar[mName];
      if (oldValue != newValue) {
        mValue = newValue;
        this.tbValue.Text = mValue;
        oldValue = newValue;
      }
    }


    /// <summary>
    /// Text change by user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tbValue_TextChanged(object sender, EventArgs e) {
      ParameterDict.Exemplar.Entries[mName] = tbValue.Text;
      CallElementChanged(mName, tbValue.Text);
    }
  }
}

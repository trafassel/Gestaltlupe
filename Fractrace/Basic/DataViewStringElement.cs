using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Text wurde vom Nutzer verändert.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tbValue_TextChanged(object sender, EventArgs e) {
      ParameterDict.Exemplar.Entries[mName] = tbValue.Text;
      CallElementChanged(mName, tbValue.Text);
      //throw new NotImplementedException();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.Basic {
  class DataViewElementFactory {

    public static DataViewElement Create(string name, string value, string type, string description,bool shortenName) {
      DataViewElement retVal = null;


      // Standardmäßig wird als Datentyp string angenommen.

      if (retVal == null) {
        retVal = new DataViewStringElement();
        retVal.Dock = System.Windows.Forms.DockStyle.Top;
        retVal.Height = mDefaultHeight;
        retVal.Init(name, value, type, description, shortenName);
      }

      return retVal;

    }


    public static int DefaultHeight {
      get {
        return mDefaultHeight;
      }
    }
    protected static int mDefaultHeight = 24;


  }
}

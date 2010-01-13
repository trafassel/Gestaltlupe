using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.DataTypes {
    public class XYZTupel {
        public double x=0;
        public double y=0;
        public double z=0;
        public double zz=0;
        public void print() { }


      /// <summary>
      /// Erstellt eine echte Kopie des Inhaltes dieser Klasse.
      /// </summary>
      /// <returns></returns>
        public XYZTupel Clone() {
          XYZTupel retVal = new XYZTupel();
          retVal = (XYZTupel)this.MemberwiseClone();
          return retVal;
        }

    }

}

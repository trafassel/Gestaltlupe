using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.TomoGeometry
{
    public class AdditionalPointInfo
    {

      public AdditionalPointInfo(AdditionalPointInfo other) {
        this.red = other.red;
        this.blue = other.blue;
        this.green = other.green;
      }


      public AdditionalPointInfo() {
      }

        public double red = 0;

        public double green = 0;

        public double blue = 0;
    }
}

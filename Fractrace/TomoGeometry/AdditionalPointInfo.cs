using System;
using System.Collections.Generic;
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


        /// <summary>
        /// Set all color components to 0.
        /// </summary>
        public void Clear()
        {
            red = 0;
            green = 0;
            blue = 0;
        }

        public AdditionalPointInfo Clone()
        {
            AdditionalPointInfo retVal = new AdditionalPointInfo();
            retVal.red = red;
            retVal.green = green;
            retVal.blue = blue;
            return retVal;
        }


    }
}

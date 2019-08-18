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
        this.red2 = other.red2;
        this.blue2 = other.blue2;
        this.green2 = other.green2;
      }


        public AdditionalPointInfo() {
      }

        public double red = 0;
        public double green = 0;
        public double blue = 0;

        // Additional color, used in picture art to save the color of result image. 
        public double red2 = 0;
        public double green2 = 0;
        public double blue2 = 0;



        /// <summary>
        /// Set all color components to 0.
        /// </summary>
        public void Clear()
        {
            red = 0;
            green = 0;
            blue = 0;
            red2 = 0;
            green2 = 0;
            blue2 = 0;
        }

        public AdditionalPointInfo Clone()
        {
            AdditionalPointInfo retVal = new AdditionalPointInfo();
            retVal.red = red;
            retVal.green = green;
            retVal.blue = blue;
            return retVal;
        }

        public bool isEmpty()
        {
            return red == 0 && green == 0 && blue == 0;

        }


    }
}

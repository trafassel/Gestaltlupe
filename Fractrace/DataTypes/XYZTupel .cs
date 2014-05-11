using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes
{
    public class XYZTupel
    {
        public double x = 0;
        public double y = 0;
        public double z = 0;
        public double zz = 0;
        public void print() { }


        /// <summary>
        /// Return deep copy.
        /// </summary>
        /// <returns></returns>
        public XYZTupel Clone()
        {
            XYZTupel retVal = new XYZTupel();
            retVal = (XYZTupel)this.MemberwiseClone();
            return retVal;
        }

    }

}

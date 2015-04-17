using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic
{
    class DataViewElementFactory
    {

        public static DataViewElement Create(string name, string value, string type, string description, bool shortenName)
        {
            DataViewElement retVal = null;

            // Use string as default datatype
            if (retVal == null)
            {
                retVal = new DataViewStringElement();
                retVal.Dock = System.Windows.Forms.DockStyle.Top;
                retVal.Height = mDefaultHeight;
                retVal.Init(name, value, type, description, shortenName);
            }
            return retVal;
        }


        /// <summary>
        /// The height of this element.
        /// </summary>
        public static int DefaultHeight
        {
            get
            {
                return mDefaultHeight;
            }
        }

        protected static int mDefaultHeight = 24;


    }
}

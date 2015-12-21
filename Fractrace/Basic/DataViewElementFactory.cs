
namespace Fractrace.Basic
{
    class DataViewElementFactory
    {

        /// <summary>
        /// The height of all created DataViewElements.
        /// </summary>
        public static int DefaultHeight { get { return _defaultHeight; } }
        protected static int _defaultHeight = 24;


        /// <summary>
        /// Return new DataViewStringElement, DataViewBoolElement or DataViewHeadlineElement, depending of given type.
        /// The Dock property is always set to DockStyle.Top.
        /// </summary>
        public static DataViewElement Create(string name, string value, string type, string description, bool shortenName)
        {
            DataViewElement retVal = null;

            switch (type)
            {
                case "Bool":
                  retVal = new DataViewBoolElement();
                  break;

                case "Headline":
                  retVal = new DataViewHeadlineElement();
                  break;
            }

            // Use string as default datatype
            if (retVal == null)
            {
                retVal = new DataViewStringElement();
                if(ParameterDict.Current[name +".PARAMETERINFO.VIEW.FixedButtons"]!="")
                {
                    string buttonValues = ParameterDict.Current[name + ".PARAMETERINFO.VIEW.FixedButtons"];
                    foreach(string buttonText in buttonValues.Split(' '))
                    {
                        DataViewStringElement stringElement = (DataViewStringElement)retVal;
                        stringElement.AddFixedValueButton(buttonText.Trim());
                    }

                }
            }


            retVal.Dock = System.Windows.Forms.DockStyle.Top;
            retVal.Height = _defaultHeight;
            retVal.Init(name, value, type, description, shortenName);

            return retVal;
        }


    }
}

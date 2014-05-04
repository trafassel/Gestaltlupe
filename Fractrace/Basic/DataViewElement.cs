using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic
{


    public delegate void ElementChangedDelegate(string name, string value);

    public partial class DataViewElement : UserControl
    {

        public DataViewElement()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public void Init(string name, string value, string type, string description, bool shortenName)
        {
            mName = name;
            mValue = value;
            mType = type;
            mDescription = description;
            if (shortenName)
            {
                string[] strings = mName.Split('.');
                if (strings.Length > 0)
                    lblName.Text = strings[strings.Length - 1];
            }
            else
            {
                string sName = name;
                while (sName.Length > 27)
                {
                    int ppos = sName.IndexOf(".");
                    if (ppos <= 1)
                    {
                        break;
                    }
                    sName = sName.Substring(ppos + 1);
                }
                lblName.Text = sName;
            }
            PreInit();
        }


        /// <summary>
        /// Is used in subclasses to raise ElementChanged on user input.
        /// </summary>
        protected string oldValue = "";


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// ElementChanged is not raised.
        /// </summary>
        public virtual void UpdateElements()
        {
            string newValue = ParameterDict.Exemplar[mName];
            if (oldValue != newValue)
            {
                mValue = newValue;
                oldValue = newValue;
            }
        }


        /// <summary>
        /// Add subclass specific control elements (has to be implemented in subclasses).
        /// </summary>
        protected virtual void PreInit()
        {
        }


        /// <summary>
        /// Name of this element (should be equal to the name of the corresponding ParameterDict entry).
        /// </summary>
        protected string mName = "";


        /// <summary>
        /// Value as string.
        /// </summary>
        protected string mValue = "";


        /// <summary>
        /// Datatype of the value of this entry.
        /// </summary>
        protected string mType = "";


        /// <summary>
        /// Short english documentation of this entry. 
        /// </summary>
        protected string mDescription = "";


        /// <summary>
        /// Is raised, if the value of the corresponding entry is changed by user input.
        /// </summary>
        public event ElementChangedDelegate ElementChanged;


        /// <summary>
        /// Has to be called in subclasses to raise ElementChanged event.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void CallElementChanged(string key, string value)
        {
            ElementChanged(key, value);
        }

    }
}

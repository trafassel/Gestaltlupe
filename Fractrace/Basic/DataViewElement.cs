using System.Windows.Forms;

namespace Fractrace.Basic
{


    public delegate void ElementChangedDelegate(string name, string value);

    /// <summary>
    /// Subcontrol, used in DataView form.
    /// </summary>
    public partial class DataViewElement : UserControl
    {

        public DataViewElement()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Return name of associated parameter.
        /// </summary>
        public string ParameterName { get { return _name; } }
        /// <summary>Name of this element (should be equal to the name of the corresponding ParameterDict entry).</summary>
        protected string _name = "";

        /// <summary>
        /// Value as string.
        /// </summary>
        protected string _value = "";

        /// <summary>
        /// Datatype of the value of this entry.
        /// </summary>
        protected string _type = "";

        /// <summary>
        /// Short english documentation of this entry. 
        /// </summary>
        protected string _description = "";

        /// <summary>
        /// Is used in subclasses to raise ElementChanged on user input.
        /// </summary>
        protected string _oldValue = "";

        /// <summary>
        /// Is raised, if the value of the corresponding entry is changed by user input.
        /// </summary>
        public event ElementChangedDelegate ElementChanged;


        /// <summary>
        /// Initialisation.
        /// </summary>
        public void Init(string name, string value, string type, string description, bool shortenName)
        {
            _name = name;
            _value = value;
            _type = type;
            _description = description;
            if (shortenName)
            {
                string[] strings = _name.Split('.');
                if (strings.Length > 0)
                    lblName.Text = strings[strings.Length - 1];
                if (lblName.Text.Length == 1)
                    panel1.Width = 30;
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
            if (description != string.Empty)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(lblName, description);
                
            }     
            PreInit();
        }


        /// <summary>
        /// Corresponding string value is set from ParameterDict.Exemplar.
        /// ElementChanged is not raised.
        /// </summary>
        public virtual void UpdateElements()
        {
            string newValue = ParameterDict.Current[_name];
            if (_oldValue != newValue)
            {
                _value = newValue;
                _oldValue = newValue;
            }
        }


        /// <summary>
        /// Add subclass specific control elements (has to be implemented in subclasses).
        /// </summary>
        protected virtual void PreInit()
        {
        }


        /// <summary>
        /// If true, no ElementChanged will be raised on update.
        /// </summary>
        protected bool _dontRaiseElementChangedEvent = false;

        /// <summary>
        /// Has to be called in subclasses to raise ElementChanged event.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void CallElementChanged(string key, string value)
        {
            if(!_dontRaiseElementChangedEvent)
              ElementChanged(key, value);
        }


    }
}

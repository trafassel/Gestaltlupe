using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic
{


    /// <summary>
    /// One page of the DataViewControl  
    /// </summary>
    public partial class DataViewControlPage : UserControl
    {
        

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewControlPage"/> class.
        /// </summary>
        public DataViewControlPage(DataViewControl parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        /// <summary>
        /// Parent Control.
        /// </summary>
        protected DataViewControl _parent = null;

        /// <summary>
        /// Gets the unique string of the names of all entries.
        /// This property can be used to test, if this page contain all elements of the category.
        /// </summary>
        /// <value>The node hash.</value>
        public string NodeHash { get { return _nodeHash; } }
        /// <summary>
        /// Intern access to NodeHash
        /// </summary>
        protected string _nodeHash = "";

        /// <summary>
        /// Gets the unique string of the names and values of all entries.
        /// This property can be used to test if all values are up to date.
        /// </summary>
        /// <value>The node value hash.</value>
        public string NodeValueHash { get { return _nodeValueHash; } }
        /// <summary>Intern access to NodeValueHash</summary>
        protected string _nodeValueHash = "";

        /// <summary>
        /// Gets the parameter category which corresponds to the subentries.
        /// </summary>
        /// <value>The category.</value>
        public string Category { get { return _category; } }
        /// <summary>The corresponding parameter category.</summary>
        protected string _category = "";

        /// <summary>
        /// Gets the expected height of this control.
        /// </summary>
        /// <value>The height of the computed.</value>
        public int ComputedHeight { get { return _computedHeight; } }
        protected int _computedHeight = 1200;

        /// <summary>
        /// Control ist filled with all entries which corresponds to the given category.
        /// </summary>
        /// <param name="category">The category.</param>
        public void Create(string category)
        {
            this.Dock = DockStyle.Fill;

            _category = category;
            _nodeHash = ParameterDict.Current.GetHashOfName(category);
            _nodeValueHash = ParameterDict.Current.GetHash(category);

            // Contain the edit entries before adding to the control
            List<DataViewElement> oldElements = new List<DataViewElement>();

            this.SuspendLayout();
            _computedHeight = 0;
            bool elementAdded = false;
            foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
            {
                string parameterName = entry.Key;
                if (parameterName.StartsWith(category+".") && ParameterDict.HasControl(parameterName))
                {
                    if (parameterName.Length > category.Length)
                    {
                        string tempName = parameterName.Substring(category.Length + 1);
                        if (!tempName.Contains("."))
                        {
                            DataViewElement dElement = DataViewElementFactory.Create(parameterName, entry.Value, ParameterDict.Current.GetDatatype(parameterName),
                                ParameterDict.Current.GetDescription(parameterName), true);
                            dElement.ElementChanged += new ElementChangedDelegate(_parent.dElement_ElementChanged);
                            oldElements.Add(dElement);
                            dElement.TabIndex = oldElements.Count;
                            _computedHeight += DataViewElementFactory.DefaultHeight;
                            elementAdded = true;
                        }
                    }
                }
            }

            // Wenn kein direktes Unterelement existiert, werden alle Unterelemente eingefügt.
            string currentCategory = "";
            string oldCategory = "";
            if (!elementAdded)
            {
                foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
                {
                    string parameterName = entry.Key;
                    if (parameterName.StartsWith(category) && ParameterDict.HasControl(parameterName) && !ParameterDict.IsAdditionalInfo(parameterName))
                    {
                        string[] paraSplit = parameterName.Split('.');
                        if (paraSplit.Length > 1)
                        {
                            currentCategory = paraSplit[paraSplit.Length - 2];
                            if (currentCategory != oldCategory)
                            {
                                DataViewElement helement = DataViewElementFactory.Create(currentCategory, "", "Headline", "", false);
                                oldElements.Add(helement);
                                oldCategory = currentCategory;
                                _computedHeight += DataViewElementFactory.DefaultHeight;
                            }
                        }
                        DataViewElement dElement = DataViewElementFactory.Create(parameterName, entry.Value, ParameterDict.Current.GetDatatype(parameterName), ParameterDict.Current.GetDescription(parameterName), true);
                        dElement.ElementChanged += new ElementChangedDelegate(_parent.dElement_ElementChanged);
                        oldElements.Add(dElement);
                        dElement.TabIndex = oldElements.Count;
                        _computedHeight += DataViewElementFactory.DefaultHeight;
                    }
                }
            }
            for (int i = oldElements.Count - 1; i >= 0; i--)
            {
                DataViewElement dElement = oldElements[i];
                Controls.Add(dElement);
            }
            this.ResumeLayout(true);
        }


        public void UpdateElements()
        {
            foreach (UserControl subControl in Controls)
            {
                if (subControl is DataViewElement)
                {
                    DataViewElement dataView = (DataViewElement)subControl;
                    dataView.UpdateElements();
                }
            }

        }


        /// <summary>
        /// Subentries are updated. Returns true, if at least one entry is added or removed.
        /// </summary>
        /// <param name="category">The category.</param>
        public bool IterateElements()
        {
            string newNodeHash = ParameterDict.Current.GetHashOfName(_category);
            string newNodeValueHash = ParameterDict.Current.GetHash(_category);
            if (newNodeHash != _nodeHash)
            { // At least one entry is added or deleted, so everything must new created 
                Create(_category);
                return true;
            }
            if (newNodeValueHash != _nodeValueHash)
            { // At least one value has changed, so each subcontrol has to update itself 
                foreach (UserControl subControl in Controls)
                {
                    if (subControl is DataViewElement)
                    {
                        DataViewElement dataView = (DataViewElement)subControl;
                        dataView.UpdateElements();
                    }
                }
                _nodeValueHash = ParameterDict.Current.GetHash(_category);
                return false;
            }
            // If nothing has changed, nothing has to update.
            return false;
        }


    }
}

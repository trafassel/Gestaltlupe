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
        /// Parent Control.
        /// </summary>
        protected DataViewControl mParent = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewControlPage"/> class.
        /// </summary>
        public DataViewControlPage(DataViewControl parent)
        {
            InitializeComponent();
            mParent = parent;
        }


        /// <summary>
        /// Gets the unique string of the names of all entries.
        ///  This property can be used to test, if this page contain all elements of the category.
        /// </summary>
        /// <value>The node hash.</value>
        public string NodeHash
        {
            get
            {
                return mNodeHash;
            }
        }


        /// <summary>
        /// Intern access to NodeHash
        /// </summary>
        protected string mNodeHash = "";


        /// <summary>
        /// Gets the unique string of the names and values of all entries.
        /// This property can be used to test if all values are up to date.
        /// </summary>
        /// <value>The node value hash.</value>
        public string NodeValueHash
        {
            get
            {
                return mNodeValueHash;
            }
        }


        /// <summary>
        /// Intern access to NodeValueHash
        /// </summary>
        protected string mNodeValueHash = "";


        /// <summary>
        /// The corresponding parameter category.
        /// </summary>
        protected string mCategory = "";


        /// <summary>
        /// Gets the parameter category which corresponds to the subentries.
        /// </summary>
        /// <value>The category.</value>
        public string Category
        {
            get
            {
                return mCategory;
            }
        }


        /// <summary>
        /// Control ist filled with all entries which corresponds to the given category.
        /// </summary>
        /// <param name="category">The category.</param>
        public void Create(string category)
        {
            this.Dock = DockStyle.Fill;

            mCategory = category;
            mNodeHash = ParameterDict.Exemplar.GetHashOfName(category);
            mNodeValueHash = ParameterDict.Exemplar.GetHash(category);
            // this.BackColor = Color.Green;

            // Contain the edit entries before adding to the control
            List<DataViewElement> oldElements = new List<DataViewElement>();
            /*
            if (category == oldCategory) {
              Update(category);
              return;
            }*/

            this.SuspendLayout();

            mComputedHeight = 0;

            bool elementAdded = false;
            foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries)
            {
                if (entry.Key.StartsWith(category))
                {
                    if (entry.Key.Length > category.Length)
                    {
                        string tempName = entry.Key.Substring(category.Length + 1);
                        if (!tempName.Contains("."))
                        {
                            DataViewElement dElement = DataViewElementFactory.Create(entry.Key, entry.Value, "", "", true);
                            dElement.ElementChanged += new ElementChangedDelegate(mParent.dElement_ElementChanged);
                            oldElements.Add(dElement);
                            dElement.TabIndex = oldElements.Count;
                            mComputedHeight += DataViewElementFactory.DefaultHeight;
                            elementAdded = true;
                        }
                    }
                }
            }

            // Wenn kein direktes Unterelement existiert, werden alle Unterelemente eingefügt.
            if (!elementAdded)
            {
                foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries)
                {
                    if (entry.Key.StartsWith(category))
                    {
                        DataViewElement dElement = DataViewElementFactory.Create(entry.Key, entry.Value, "", "", false);
                        dElement.ElementChanged += new ElementChangedDelegate(mParent.dElement_ElementChanged);
                        oldElements.Add(dElement);
                        dElement.TabIndex = oldElements.Count;
                        mComputedHeight += DataViewElementFactory.DefaultHeight;
                        elementAdded = true;
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


        protected int mComputedHeight = 1200;


        /// <summary>
        /// Gets the expected height of this control.
        /// </summary>
        /// <value>The height of the computed.</value>
        public int ComputedHeight
        {
            get
            {
                return mComputedHeight;
            }
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
            string newNodeHash = ParameterDict.Exemplar.GetHashOfName(mCategory);
            string newNodeValueHash = ParameterDict.Exemplar.GetHash(mCategory);
            if (newNodeHash != mNodeHash)
            { // At least one entry is added or deleted, so everything must new created 
                Create(mCategory);
                return true;
            }
            if (newNodeValueHash != mNodeValueHash)
            { // At least one value has changed, so each subcontrol has to update itself 
                foreach (UserControl subControl in Controls)
                {
                    if (subControl is DataViewElement)
                    {
                        DataViewElement dataView = (DataViewElement)subControl;
                        dataView.UpdateElements();
                    }
                }
                mNodeValueHash = ParameterDict.Exemplar.GetHash(mCategory);
                return false;
            }

            return false;
            // If nothing has changed, nothing has to update.

        }

    }
}

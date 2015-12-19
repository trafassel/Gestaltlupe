using System.Collections.Generic;
using System.Windows.Forms;

namespace Fractrace.Basic
{


    /// <summary>
    /// Display the Entries of the global object ParameterDict.
    /// </summary>
    public partial class DataViewControl : UserControl
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewControl"/> class.
        /// </summary>
        public DataViewControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Some created pages for fast display on node selection. Key is the category.
        /// </summary>
        protected Dictionary<string, DataViewControlPage> _pages = new Dictionary<string, DataViewControlPage>();


        /// <summary>
        /// Current choosen category (of selected node).
        /// </summary>
        protected string _oldCategory = "";


        /// <summary>
        /// A new category (as node in the tree view is selected). This control 
        /// has to display all corresponding entries.
        /// </summary>
        /// <param name="category">The category.</param>
        public void Select(string category)
        {
            if (category == "")
                return;
            this.SuspendLayout();
            DataViewControlPage newPage = null;
            if (_pages.ContainsKey(category))
            {
                newPage = _pages[category];
                if (newPage.NodeValueHash == ParameterDict.Exemplar.GetHashOfName(category))
                {
                    newPage.UpdateElements();
                    newPage.Update();
                }
                else
                {
                    newPage = new DataViewControlPage(this);
                    newPage.Create(category);
                    _pages[category] = newPage;
                }
            }
            else
            {
                newPage = new DataViewControlPage(this);
                newPage.Create(category);
                _pages[category] = newPage;
            }
            if (_oldCategory != category)
            {
                pnlMain.Controls.Clear();
                pnlMain.Controls.Add(newPage);
                this.Height = newPage.ComputedHeight;
            }
            this.ResumeLayout(true);
            _oldCategory = category;
        }


        /// <summary>
        /// SubElement has changed.
        /// </summary>
        public void dElement_ElementChanged(string name, string value)
        {
            if (ElementChanged != null)
                ElementChanged(name, value);
        }


        /// <summary>
        /// Called if a subentry value changed.
        /// </summary>
        public event ElementChangedDelegate ElementChanged;


    }
}

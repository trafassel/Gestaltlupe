using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic {


  /// <summary>
  /// Display the Entries of the global object ParameterDict.
  /// </summary>
  public partial class DataViewControl : UserControl {


    /// <summary>
    /// Initializes a new instance of the <see cref="DataViewControl"/> class.
    /// </summary>
    public DataViewControl() {
      InitializeComponent();
    }


    /// <summary>
    /// Some created pages for fast display on node selection. Key is the category.
    /// </summary>
    protected Dictionary<string, DataViewControlPage> mPages = new Dictionary<string, DataViewControlPage>();


    /// <summary>
    /// Current choosen category (of selected node).
    /// </summary>
    protected string oldCategory = "";


    /// <summary>
    /// A new category (as node in the tree view is selected). This control 
    /// has to display all corresponding entries.
    /// </summary>
    /// <param name="category">The category.</param>
    public void Select(string category) {
        if (category == "")
            return;
      this.SuspendLayout();
      DataViewControlPage newPage=null;
      if (mPages.ContainsKey(category)) {
        newPage = mPages[category];
        newPage.UpdateElements();
        newPage.Update();
      } else {
        newPage = new DataViewControlPage(this);
        newPage.Create(category);
        mPages[category] = newPage;
      }
      if (oldCategory != category) {
        pnlMain.Controls.Clear();
        pnlMain.Controls.Add(newPage);
        this.Height = newPage.ComputedHeight;
      }
      this.ResumeLayout(true);
      oldCategory = category;
    }



    /// <summary>
    /// SubElement has changed.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void dElement_ElementChanged(string name, string value) {
      if(ElementChanged!=null)
        ElementChanged(name,value);
    }


    /// <summary>
    /// Subentry change its value.
    /// </summary>
    public event ElementChangedDelegate ElementChanged;

  }
}

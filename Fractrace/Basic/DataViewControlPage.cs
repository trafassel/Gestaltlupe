using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic {


  /// <summary>
  /// One page of the DataViewControl  
  /// </summary>
  public partial class DataViewControlPage : UserControl {


    /// <summary>
    /// Parent Control.
    /// </summary>
    protected DataViewControl mParent = null;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="DataViewControlPage"/> class.
    /// </summary>
    public DataViewControlPage(DataViewControl parent) {
      InitializeComponent();
      mParent = parent;
    }


    /// <summary>
    /// Gets the unique string of the names of all entries.
    ///  This property can be used to test, if this page contain all elements of the category.
    /// </summary>
    /// <value>The node hash.</value>
    public string NodeHash {
      get {
        return mNodeHash;
      }
    }


    /// <summary>
    /// Intern access to NodeHash
    /// </summary>
    protected string mNodeHash = "";




    /// <summary>
    /// Gets the unique string of the names and values of all entries.
    /// This property can be used to test, if all values are up to date.
    /// </summary>
    /// <value>The node value hash.</value>
    public string NodeValueHash {
      get {
        return mNodeHash;
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
    public string Category {
      get {
        return mCategory;
      }
    }

    public void Update(string category) {


    }

  }
}

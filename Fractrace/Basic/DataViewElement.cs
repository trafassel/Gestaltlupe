using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic {


  public delegate void ElementChangedDelegate(string name, string value);

  public partial class DataViewElement : UserControl {

    
    public DataViewElement() {
      InitializeComponent();
    }


    /// <summary>
    /// Initialisierung.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="description"></param>
    public void Init(string name, string value, string type, string description,bool shortenName) {
      mName = name;
      mValue = value;
      mType = type;
      mDescription = description;
      if (shortenName) {
        string[] strings = mName.Split('.');
        if (strings.Length > 0)
          lblName.Text = strings[strings.Length - 1];
      } else {
        string sName = name;
        while (sName.Length > 27) {
           int ppos = sName.IndexOf(".");
           if (ppos <= 1) {
             break;
           }
           sName = sName.Substring(ppos + 1);
        }
        lblName.Text =sName;
      }
      PreInit();
    }


    /// <summary>
    /// Hier werden die weiteren Steuerelemente aufgebaut. 
    /// </summary>
    protected virtual void PreInit() {
    }



    protected string mName = "";

    protected string mValue = "";

    protected string mType = "";

    protected string mDescription = "";

    public event ElementChangedDelegate ElementChanged;

    protected void CallElementChanged(string key, string value) {
      ElementChanged(key, value);
    }

  }
}

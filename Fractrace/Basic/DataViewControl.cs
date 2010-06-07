using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic {
  public partial class DataViewControl : UserControl {
    public DataViewControl() {
      InitializeComponent();
    }


    protected List<DataViewElement> oldElements = new List<DataViewElement>();

    /// <summary>
    /// Inhalt des Steuerelementes wird mit den Einträgen befüllt, die category entsprechen.
    /// </summary>
    /// <param name="category"></param>
    public void Select(string category) {
      this.SuspendLayout();

      // alte events abkoppeln:
      foreach (DataViewElement element in oldElements) {
        if(element!=null)
          element.ElementChanged += new ElementChangedDelegate(dElement_ElementChanged);
      }
      oldElements.Clear();

      pnlMain.Controls.Clear();
      int height = 0;

      bool elementAdded = false;
      foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries) {
        if (entry.Key.StartsWith(category)) {
          if (entry.Key.Length > category.Length) {
            string tempName = entry.Key.Substring(category.Length + 1);
            if (!tempName.Contains(".")) {
              DataViewElement dElement = DataViewElementFactory.Create(entry.Key, entry.Value, "", "", true);
              dElement.ElementChanged += new ElementChangedDelegate(dElement_ElementChanged);
              oldElements.Add(dElement);
              dElement.TabIndex = oldElements.Count;
              height += DataViewElementFactory.DefaultHeight;
              elementAdded = true;
            }
          }
        }
      }

      // Wenn kein dirketes Unterelement existiert, werden alle Unterelemente eingefügt.
      if (!elementAdded) {
        foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries) {
          if (entry.Key.StartsWith(category)) {
              DataViewElement dElement = DataViewElementFactory.Create(entry.Key, entry.Value, "", "",false);
             // pnlMain.Controls.Add(dElement);
              dElement.ElementChanged += new ElementChangedDelegate(dElement_ElementChanged);
              oldElements.Add(dElement);
              dElement.TabIndex = oldElements.Count;
              height += DataViewElementFactory.DefaultHeight;
              elementAdded = true;
          }
        }


      }

      for (int i = oldElements.Count - 1; i >= 0; i--) {
        DataViewElement dElement = oldElements[i];
        pnlMain.Controls.Add(dElement);
      }
             


      this.Height = height;
      this.ResumeLayout(true);

    }

    /// <summary>
    /// Ein Unterelement hat sich geändert.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void dElement_ElementChanged(string name, string value) {
      if(ElementChanged!=null)
        ElementChanged(name,value);
      //throw new NotImplementedException();
    }


    /// <summary>
    /// Wird aufgerufen, wenn der Nutzer eine Änderung eingegeben hat.
    /// </summary>
    public event ElementChangedDelegate ElementChanged;

  }
}

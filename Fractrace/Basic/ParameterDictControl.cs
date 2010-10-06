using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic {
    public partial class ParameterDictControl : UserControl {


        public ParameterDictControl() {
            InitializeComponent();
            Init();
        }


        /// <summary>
        /// Der Baum wird aus den Einträgen aus dem ParameterDict aufgebaut.
        /// </summary>
        protected void Init() {
            foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries) {
                string cat = GetCategory(entry.Key);
                string parentCat = GetCategory(cat);
                if (!Nodes.ContainsKey(cat)) {
                    TreeNode tNode = new TreeNode(GetName(cat));
                    tNode.Tag = cat;
                    if (parentCat == ".") {
                        treeView1.Nodes.Add(tNode);
                    } else
                        if (parentCat != "") {
                            if (!Nodes.ContainsKey(parentCat)) {
                               
                                Nodes[parentCat] = new TreeNode(parentCat);
                                Nodes[parentCat].Tag = parentCat;
                                // TODO: eigentlich müsste hier rekursiv die Hirarchie aufgebaut werden
                                string testParentParent = GetCategory(parentCat);
                                if (testParentParent!="." && Nodes.ContainsKey(parentCat)) {
                                  Nodes[testParentParent].Nodes.Add(Nodes[parentCat]);
                                } else 
                                treeView1.Nodes.Add(Nodes[parentCat]);
                            }
                            Nodes[parentCat].Nodes.Add(tNode);
                        }
                    Nodes[cat] = tNode;
                }
            }
        }


        protected Dictionary<string, TreeNode> Nodes = new Dictionary<string, TreeNode>();

        /// <summary>
        /// Die Hirarchie wird in den Einträgen durch . abgetrennt. Hier wird der String 
        /// bis zum letzten . zurückgeliefert.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string GetCategory(string input) {
            int pos = input.LastIndexOf('.');
            if (pos != -1) {
                return input.Substring(0, pos);
            }
            return ".";
        }


        /// <summary>
        /// Liefert den Namen des Baumknotens der angegebenen Kategorie
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        protected string GetName(string category) {
            int pos = category.LastIndexOf('.');
            if (pos != -1) {
                return category.Substring(pos+1);
            }
            return category;
        }


        protected string mChoosenHirarchy = "";


        /// <summary>
        /// Die Daten werden neu gezeichnet.
        /// </summary>
        public void UpdateFromData() {
          this.dataViewControl1.Select(mChoosenHirarchy);


          if (inDataGridView1_CellValueChanged)
            return;
            arr = new System.Collections.Generic.List<OptionMember>();
            foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries) {
                if (entry.Key.StartsWith(mChoosenHirarchy)) {
                    OptionMember newEntry = new OptionMember(entry.Key, entry.Value);
                    try {
                      arr.Add(newEntry);
                    } catch (System.Exception ex) {
                      System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
            try {
              dataGridView1.DataSource = arr;
            } catch (Exception ex) {
              System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
          SelectNode(e.Node.Tag.ToString());
        }


      /// <summary>
      /// Ein Knoten mit einer bestimmten Hirarchie wurde ausgewählt.
      /// </summary>
      /// <param name="hirarchy"></param>
        public void SelectNode(string hirarchy) {
          mChoosenHirarchy = hirarchy;
          UpdateFromData();
        }

        /// <summary>
        /// Die Einträge zum ausgewählen Knoten.
        /// </summary>
        System.Collections.Generic.List<OptionMember> arr = null;


        private bool inDataGridView1_CellValueChanged = false;

        /// <summary>
        /// Ein Eintrag wurde verändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
          inDataGridView1_CellValueChanged = true;
           OptionMember optionEntry=arr[e.RowIndex];
           ParameterDict.Exemplar[optionEntry.Name] = optionEntry.Value;
           Changed = true;
           inDataGridView1_CellValueChanged = false;
        }


        /// <summary>
        /// Zugriff auf das aktualisierte Datagrid
        /// </summary>
        public DataGridView InternDataGridView {
            get {
                return dataGridView1;
            }

        }

        public bool Changed = false;

    }
}

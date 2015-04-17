using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fractrace.Basic
{
    public partial class ParameterDictControl : UserControl
    {


        public ParameterDictControl()
        {
            InitializeComponent();
            Init();
        }


        /// <summary>
        /// Build hierarchy from ParameterDict entries. 
        /// </summary>
        protected void Init()
        {
            foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.SortedEntries)
            {
                string cat = GetCategory(entry.Key);
                string parentCat = GetCategory(cat);
                if (!Nodes.ContainsKey(cat))
                {
                    TreeNode tNode = new TreeNode(GetName(cat));
                    tNode.Tag = cat;
                    if (parentCat == ".")
                    {
                        treeView1.Nodes.Add(tNode);
                    }
                    else
                        if (parentCat != "")
                        {
                            if (!Nodes.ContainsKey(parentCat))
                            {
                                Nodes[parentCat] = new TreeNode(parentCat);
                                Nodes[parentCat].Tag = parentCat;
                                // TODO: eigentlich müsste hier rekursiv die Hirarchie aufgebaut werden
                                string testParentParent = GetCategory(parentCat);
                                if (testParentParent != "." && Nodes.ContainsKey(parentCat))
                                {
                                    Nodes[testParentParent].Nodes.Add(Nodes[parentCat]);
                                }
                                else
                                    treeView1.Nodes.Add(Nodes[parentCat]);
                            }
                            Nodes[parentCat].Nodes.Add(tNode);
                        }
                    Nodes[cat] = tNode;
                }
            }
        }


        /// <summary>
        /// Nodes of this three with unique name of the entry as key.
        /// </summary>
        protected Dictionary<string, TreeNode> Nodes = new Dictionary<string, TreeNode>();


        /// <summary>
        /// Die Hirarchie wird in den Einträgen durch . abgetrennt. Hier wird der String 
        /// bis zum letzten . zurückgeliefert.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string GetCategory(string input)
        {
            int pos = input.LastIndexOf('.');
            if (pos != -1)
            {
                return input.Substring(0, pos);
            }
            return ".";
        }


        /// <summary>
        /// Liefert den Namen des Baumknotens der angegebenen Kategorie
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        protected string GetName(string category)
        {
            int pos = category.LastIndexOf('.');
            if (pos != -1)
            {
                return category.Substring(pos + 1);
            }
            return category;
        }


        protected string mChoosenHirarchy = "";


        /// <summary>
        /// Die Daten werden neu gezeichnet.
        /// </summary>
        public void UpdateFromData()
        {
            this.dataViewControl1.Select(mChoosenHirarchy);
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectNode(e.Node.Tag.ToString());
        }


        /// <summary>
        /// Ein Knoten mit einer bestimmten Hirarchie wurde ausgewählt.
        /// </summary>
        /// <param name="hirarchy"></param>
        public void SelectNode(string hirarchy)
        {
            mChoosenHirarchy = hirarchy;
            UpdateFromData();
        }



        public bool Changed = false;

    }
}

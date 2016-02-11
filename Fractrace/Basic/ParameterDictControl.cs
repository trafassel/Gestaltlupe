using System.Collections.Generic;
using System.Windows.Forms;

namespace Fractrace.Basic
{

    /// <summary>
    /// Control to view and edit all public ParameterDict Entries.
    /// </summary>
    public partial class ParameterDictControl : UserControl
    {

        public delegate void VoidDelegate();

        public ParameterDictControl()
        {
            InitializeComponent();
            Init();
            dataViewControl1.ElementChanged += DataViewControl1_ElementChanged;
        }


        /// <summary>
        /// Nodes of this three with unique name of the entry as key.
        /// </summary>
        protected Dictionary<string, TreeNode> _nodes = new Dictionary<string, TreeNode>();

        /// <summary>
        /// Id of currently selected hirarchy.
        /// </summary>
        protected string _choosenHirarchy = "";

        public DataViewControl MainDataViewControl { get { return dataViewControl1; } }

        /// <summary>
        /// Called if a subentry value changed.
        /// </summary>
        public event ElementChangedDelegate ElementChanged;


        /// <summary>
        /// Build hierarchy from ParameterDict entries. 
        /// </summary>
        protected void Init()
        {
            foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
            {
                string parameterName=entry.Key;
                if (   !ParameterDict.IsAdditionalInfo(parameterName) && HasEntryControl(parameterName))
                {
                    string cat = GetCategory(parameterName);
                    string parentCat = GetCategory(cat);
                    if (!_nodes.ContainsKey(cat))
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
                                if (!_nodes.ContainsKey(parentCat))
                                {
                                    _nodes[parentCat] = new TreeNode(parentCat);
                                    _nodes[parentCat].Tag = parentCat;
                                    // TODO: eigentlich müsste hier rekursiv die Hirarchie aufgebaut werden
                                    string testParentParent = GetCategory(parentCat);
                                    if (testParentParent != "." && _nodes.ContainsKey(parentCat))
                                    {
                                        _nodes[testParentParent].Nodes.Add(_nodes[parentCat]);
                                    }
                                    else
                                        treeView1.Nodes.Add(_nodes[parentCat]);
                                }
                                // Test, if Parent Node page contains all elements of this node
                                // in this case, the node as subtree is not nececcary.
                                if (NeedSubNodes(parentCat))
                                    _nodes[parentCat].Nodes.Add(tNode);
                            }
                        _nodes[cat] = tNode;
                    }
                }
            }
            treeView1.ExpandAll();
        }


        private void DataViewControl1_ElementChanged(string name, string value)
        {
            ElementChanged(name, value);
        }


        /// <summary>
        /// Return true, if node corresponding to given category need subnodes to edit subentries.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        protected bool NeedSubNodes(string category)
        {
            foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
            {
                string parameterName = entry.Key;
                if (parameterName.StartsWith(category) && ParameterDict.HasControl(parameterName) && parameterName!=category)
                {
                    string tempName = parameterName.Substring(category.Length + 1);
                    if (!tempName.Contains("."))
                    {
                        return true;
                    }
                    System.Diagnostics.Debug.WriteLine(parameterName);
                }
            }
            return false;
        }


        /// <summary>
        /// Die Hirarchie wird in den Einträgen durch . abgetrennt. Hier wird der String 
        /// bis zum letzten . zurückgeliefert.
        /// </summary>
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
        protected string GetName(string category)
        {
            int pos = category.LastIndexOf('.');
            if (pos != -1)
            {
                return category.Substring(pos + 1);
            }
            return category;
        }

        
        /// <summary>
        /// Return true, if corresponding parameter entry has an user control in ParameterDictControl.
        /// </summary>
        /// <returns></returns>
        bool HasEntryControl(string parameterName)
        {
            if (parameterName.StartsWith("Intern."))
                return false;
            if (ParameterDict.Current.GetBool(parameterName + ".PARAMETERINFO.VIEW.Invisible"))
                return false;
            return true;
        }


        /// <summary>
        /// Redraw Data Page.
        /// </summary>
        public void UpdateFromData()
        {
            this.Invoke(new VoidDelegate(UpdateFromDataIntern));
        }


        /// <summary>
        /// Redraw Data Page.
        /// </summary>
        private void UpdateFromDataIntern()
        {
            lock (_updateFromDataMutex)
            {
                if (_inUpdateFromData)
                    return;
                _inUpdateFromData = true;
            }
            this.dataViewControl1.Select(_choosenHirarchy);         
            pnlEdit.Dock = DockStyle.Fill;
        
            lock (_updateFromDataMutex)
            {
                _inUpdateFromData = false;
            }
        }


        object _updateFromDataMutex=new object();
        bool _inUpdateFromData = false;


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectNode(e.Node.Tag.ToString());
        }


        public void SelectTreeNode(string hirarchy)
        {
            _nodes[hirarchy].TreeView.SelectedNode = _nodes[hirarchy];
        }


        /// <summary>
        /// Ein Knoten mit einer bestimmten Hirarchie wurde ausgewählt.
        /// </summary>
        /// <param name="hirarchy"></param>
        public void SelectNode(string hirarchy)
        {
            _choosenHirarchy = hirarchy;
            UpdateFromDataIntern();
        }


    }
}

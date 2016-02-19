using Fractrace.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractrace.Gui
{
    public partial class ExportResultDialog : Form
    {
        public ExportResultDialog(string fileName)
        {
            InitializeComponent();
            lblExportText.Text = "Export to " + fileName;
            cbOpenInBrowser.Checked = Settings.Default.ShowExportInBrowser;
        }


        public bool OpenInBrowser { get { return cbOpenInBrowser.Checked; } }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbOpenInBrowser_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ShowExportInBrowser = cbOpenInBrowser.Checked;
            Settings.Default.Save();
        }
    }
}

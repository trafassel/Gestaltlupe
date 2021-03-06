﻿using System;

using System.Windows.Forms;

namespace Fractrace.Gui
{

    
    public partial class ExportResultDialog : Form
    {
        static bool _openInBrowser = false;

        public ExportResultDialog(string fileName)
        {
            InitializeComponent();
            lblExportText.Text = "Export to " + fileName;
            cbOpenInBrowser.Checked = _openInBrowser;
        }


        public bool OpenInBrowser { get { return cbOpenInBrowser.Checked; } }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbOpenInBrowser_CheckedChanged(object sender, EventArgs e)
        {
            _openInBrowser = cbOpenInBrowser.Checked;
        }
    }
}

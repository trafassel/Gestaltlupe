using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fractrace.Properties;

namespace Fractrace.Gui
{


    /// <summary>
    /// User input for:
    /// Settings.Default.DeleteCacheAutomatically
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            
        }

        private void cbDeleteCacheAutomatically_CheckedChanged(object sender, EventArgs e)
        {
           
        }


    }
}

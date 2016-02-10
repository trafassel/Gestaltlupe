using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Fractrace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            double t = Math.Atan2(1, 0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ResultImageView());
        }


    }
}

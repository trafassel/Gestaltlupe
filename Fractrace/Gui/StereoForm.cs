using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fractrace
{
    public partial class StereoForm : Form
    {
        public StereoForm()
        {
            InitializeComponent();
            imageRenderer.IsRightView = true;
        }


        /// <summary>
        /// Zugriff auf das Preview-Control
        /// </summary>
        public RenderImage ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }


        /// <summary>
        /// Berechnung wird abgebrochen.
        /// </summary>
        public void Abort()
        {
            if (imageRenderer != null)
            {
                imageRenderer.Abort();
            }
        }

    }
}

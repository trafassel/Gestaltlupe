using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractrace.Animation
{
    public partial class AnimationStepPreview : UserControl
    {
        public AnimationStepPreview()
        {
            InitializeComponent();
            Bitmap image = new Bitmap(200, 100);
            grSteps = Graphics.FromImage(image);
            pictureBox1.Image = image;
            this.ContextMenuStrip = contextMenuStrip1;
        }


        protected Graphics grSteps = null;

        int time = 0;

        int steps = 0;


        /// <summary>
        /// Initialize with Animation time and number of steps used in Animation.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="steps"></param>
        public void Init(int time,int steps)
        {
            this.time = time;
            this.steps = steps;
            label1.Text = time.ToString();

            Pen p = new Pen(System.Drawing.Color.Black);
            double width = this.Width;
            double dsteps = width / ((double)steps);
            for (double i = 0; i < steps; ++i)
            {
                double pos = i *dsteps;
                int ipos = (int)pos;
                grSteps.DrawLine(p, ipos, 3, ipos, 6);

            }
        }



        public void UpdateComputedStep(int currentStep)
        {

            Pen p = new Pen(System.Drawing.Color.Green);
            double width = this.Width;
            double dsteps = width / ((double)steps);

            double pos = (currentStep+1) * dsteps;
            int ipos = (int)pos;

            for (int i = 0; i < pos; ++i)
            {
                grSteps.DrawLine(p, i, 3, i, 6);
            }
            this.Refresh();
        }


        /// <summary>
        /// Remove corresponding entry in Animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pen p = new Pen(System.Drawing.Color.Red);
            double width = this.Width;
            double dsteps = width / ((double)steps);
            for (double i = 0; i < steps; ++i)
            {
                double pos = i * dsteps;
                int ipos = (int)pos;
                grSteps.DrawLine(p, ipos, 3, ipos, 6);

            }

            Fractrace.Animation.AnimationControl.MainAnimationControl.RemoveStep(time);
            this.Refresh();
        }

    }
}

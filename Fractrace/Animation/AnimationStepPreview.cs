using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fractrace.Animation
{

    /// <summary>
    /// Control, which is used in the animation form to show a small picture of some animation steps.
    /// </summary>
    public partial class AnimationStepPreview : UserControl
    {
        public AnimationStepPreview()
        {
            InitializeComponent();
            Bitmap image = new Bitmap(200, 100);
            _graphicsUsedInSteps = Graphics.FromImage(image);
            pictureBox1.Image = image;
            this.ContextMenuStrip = contextMenuStrip1;
        }


        /// <summary>
        /// Graphics, used to draw the rendered imag.
        /// </summary>
        protected Graphics _graphicsUsedInSteps = null;

        /// <summary>
        /// Corresponding global history time.
        /// </summary>
        int _time = 0;


        /// <summary>
        /// Number of substeps until next animation step. 
        /// </summary>
        int _steps = 0;
    

        /// <summary>
        /// Initialize with Animation time and number of steps used in Animation.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="steps"></param>
        public void Init(int time,int steps)
        {
            this._time = time;
            this._steps = steps;
            label1.Text = time.ToString();

            Pen p = new Pen(System.Drawing.Color.Black);
            double width = this.Width;
            double dsteps = width / ((double)steps);
            for (double i = 0; i < steps; ++i)
            {
                double pos = i *dsteps;
                int ipos = (int)pos;
                _graphicsUsedInSteps.DrawLine(p, ipos, 3, ipos, 6);

            }
        }


        /// <summary>
        /// Indicate computed frame (with green rectangle).
        /// </summary>
        public void UpdateComputedStep(int currentStep)
        {
            if (_steps < 1)
                return;
            Pen p = new Pen(System.Drawing.Color.Green);
            double width = this.Width;
            double dsteps = width / ((double)_steps);
            double pos = (currentStep+1) * dsteps;
            for (int i = 0; i < pos; ++i)
            {
                _graphicsUsedInSteps.DrawLine(p, i, 3, i, 6);
            }
            this.Refresh();
        }


        /// <summary>
        /// Remove corresponding entry from animation scriptand mark deletion in preview with red.
        /// </summary>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pen p = new Pen(System.Drawing.Color.Red);
            double width = this.Width;
            double dsteps = width / ((double)_steps);
            for (double i = 0; i < _steps; ++i)
            {
                double pos = i * dsteps;
                int ipos = (int)pos;
                _graphicsUsedInSteps.DrawLine(p, ipos, 3, ipos, 6);
            }
            AnimationControl.MainAnimationControl.RemoveStep(_time);
            this.Refresh();
        }


    }
}

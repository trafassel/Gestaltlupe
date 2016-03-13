using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.PictureArt {


    public class Renderer {

        public delegate void PaintEndsDelegate();


        //public event PaintEndsDelegate PaintEnds;

        public PictureData PictureData { get { return pData; } }
        protected PictureData pData = null;

        protected Formulas formula = null;

        protected int _width = 0;
        
        protected int _height = 0;

        /// <summary>
        /// Is set to true, if painting ends.
        /// </summary>
        protected bool paintHasEnded = false;


        /// <summary>
        /// Called, if the paint algorithm ends.
        /// </summary>
        protected void CallPaintEnds()
        {
            paintHasEnded = true;
            //if (PaintEnds != null)
            //    PaintEnds();
        }


        /// <summary>
        /// Initialisation with formula is needed for sharp rendering and computing original coordinates.
        /// </summary>
        public virtual void Init(Formulas formula) {
            this.formula = formula;
        }


        /// <summary>
        /// Initialisation
        /// </summary>
        public Renderer(PictureData pData) {
            this.pData = pData;
        }



         /// <summary>
        /// Paint generated rgb values to bitmap.
        /// </summary>
        public virtual void Paint(Graphics grLabel) {
        }


        protected bool _stopRequest = false;


        /// <summary>
        /// Render process should be stopped.
        /// </summary>
        public void Stop()
        {
            _stopRequest = true;
        }


        /// <summary>
        /// Return true if image is successfully created.
        /// </summary>
        public bool Valid { get { return !_stopRequest && paintHasEnded; }  }


        /// <summary>
        /// Stop Render process and wait for its ending.
        /// </summary>
        public void StopAndWait()
        {
            Stop();
            while (!paintHasEnded)
                System.Threading.Thread.Sleep(10);
        }


        /// <summary>
        /// Wait for ending of Rendering.
        /// </summary>
        public void WaitUntilEnd()
        {
            while (!paintHasEnded)
                System.Threading.Thread.Sleep(10);
        }



    }
}

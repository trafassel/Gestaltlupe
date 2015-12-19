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


        public event PaintEndsDelegate PaintEnds;

        protected PictureData pData = null;

        protected Formulas formula = null;

        protected int width = 0;
        
        protected int height = 0;

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
            if (PaintEnds != null)
                PaintEnds();
        }


        /// <summary>
        /// Initialisation with formula is needed for sharp rendering and computing original coordinates.
        /// </summary>
        /// <param name="formula"></param>
        public void Init(Formulas formula) {
            this.formula = formula;
        }


        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="pData"></param>
        public Renderer(PictureData pData) {
            this.pData = pData;
        }



         /// <summary>
        /// Erstellt das fertige Bild
        /// </summary>
        /// <param name="grLabel"></param>
        public virtual void Paint(Graphics grLabel) {
        }


        protected bool stopRequest = false;


        /// <summary>
        /// Render process should be stopped.
        /// </summary>
        public void Stop()
        {
            stopRequest = true;
        }


        /// <summary>
        /// Stop Render process and wait for its ending.
        /// </summary>
        public void StopAndWait()
        {
            Stop();
            while (!paintHasEnded)
                System.Threading.Thread.Sleep(10);
        }




    }
}

using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.PictureArt {


    public class Renderer {

        protected PictureData pData = null;

        protected Formulas formula = null;


        /// <summary>
        /// Initialisation. Formula is needed for sharp rendering and computing original coordinates.
        /// </summary>
        /// <param name="formula"></param>
        public void Init(Formulas formula) {
            this.formula = formula;
        }




        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public Renderer(PictureData pData) {
            this.pData = pData;
        }


        protected int width = 0;
        protected int height = 0;

         /// <summary>
        /// Erstellt das fertige Bild
        /// </summary>
        /// <param name="grLabel"></param>
        public virtual void Paint(Graphics grLabel) {
        }



    }
}

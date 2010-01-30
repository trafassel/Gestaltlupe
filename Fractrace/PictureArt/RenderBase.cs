using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.PictureArt {
  public class RenderBase : Renderer {

            /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public RenderBase(PictureData pData):base(pData) {
        }

        /// <summary>(
        /// Erstellt das fertige Bild
        /// </summary>
        /// <param name="grLabel"></param>
        public override void Paint(Graphics grLabel) {
          width = pData.Width;
          height = pData.Height;
          PreCalculate();
          for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
              Pen p = new Pen(GetColor(i, j));
              grLabel.DrawRectangle(p, i, j, (float)0.5, (float)0.5);
            }
          }
        }

            /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
        protected virtual void PreCalculate() {


        }

            /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Color GetColor(int x, int y) {
          return Color.Red;
        }


  }
}

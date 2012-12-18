using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

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
         Vec3 col=GetRgbAt(x,y);
         if (col.X < 0)
           col.X = 0;
         if (col.Y < 0)
           col.Y = 0;
         if (col.Z < 0)
           col.Z = 0;
         if (col.X > 1)
           col.X = 1;
         if (col.Y > 1)
           col.Y = 1;
         if (col.Z > 1)
           col.Z = 1;
        
          try {
            if ( double.IsNaN(col.X))
              return Color.Red;
            return Color.FromArgb((int)(255.0*col.X), (int)(255.0*col.Y), (int)(255.0*col.Z));
          } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
          }
          return Color.Black;
        }


        protected virtual Vec3 GetRgbAt(int x, int y) {
          Vec3 retVal = new Vec3(1, 0, 0); // rot
          return retVal;
        }


  }
}

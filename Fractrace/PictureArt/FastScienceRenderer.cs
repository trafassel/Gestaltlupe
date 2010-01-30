using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.PictureArt {

  /// <summary>
  /// Ein auf ein gutes Verhältnis zwischen der Bildqualität und der Berechnungszeit ausgelegter Renderer.
  /// </summary>
  public class FastScienceRenderer: ScienceRendererBase {

       /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
    public FastScienceRenderer(PictureData pData)
      : base(pData) {
        }

     /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
    protected override void PreCalculate() {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected override Color GetColor(int x, int y) {
      double red = 0;
      double green = 0;
      double blue = 0;
      PixelInfo pInfo = pData.Points[x, y];
      if (pInfo == null) {
        return Color.Black;

      }
      return Color.Gray;
    }

              

  }
}

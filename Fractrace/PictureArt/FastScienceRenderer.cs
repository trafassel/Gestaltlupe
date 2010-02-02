using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

namespace Fractrace.PictureArt {

  /// <summary>
  /// Ein auf ein gutes Verhältnis zwischen der Bildqualität und der Berechnungszeit ausgelegter Renderer.
  /// </summary>
  public class FastScienceRenderer : ScienceRendererBase {

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



    protected override Vec3 GetRgbAt(int x, int y) {
      Vec3 retVal = new Vec3(0, 0, 1); // rot
      PixelInfo pInfo = pData.Points[x, y];
      if (pInfo == null) {
        return new Vec3(0, 0, 0);
      }

      Vec3 light = new Vec3(0, 0, 0);

      if (pInfo.frontLight < 0) {
        //return Color.FromArgb((int)255, (int)(0), (int)0);
        try {
          retVal.X = 0.3;
          retVal.Y = -pInfo.frontLight/255.0;
          retVal.Z = 0;

          return retVal;

        } catch (Exception ex) {
          retVal.X = 1;
          retVal.Y = 0;
          retVal.Z = 0;
          return retVal;

        }
      }

      if (pInfo.Normal != null) {
        light = GetLight(pInfo.Normal);
      }

      retVal.X = light.X;
      retVal.Y = light.Y;
      retVal.Z = light.Z;

      return retVal;
    }


    /// <summary>
    /// Liefert die Farbe der Oberfläche entsprechend der Normalen.
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    protected Vec3 GetLight(Vec3 normal) {
      Vec3 retVal = new Vec3(0, 0, 0);

      double norm= Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z*normal.Z);
      /* Der Winkel ist nun das Skalarprodukt mit (0,-1,0)= Lichtstrahl */
      /* mit Vergleichsvektor (Beide nachträglich normiert )             */
      double winkel = 0;
      if(norm!=0) 
      winkel=Math.Acos((normal.Y) /norm);
      winkel = 1 - winkel;

      if (winkel < 0)
        winkel = 0;
      if (winkel > 1)
        winkel = 1;

      winkel *= winkel;

      retVal.X = winkel;
      retVal.Y = winkel;
      retVal.Z = winkel;

      return retVal;
    }


  }
}

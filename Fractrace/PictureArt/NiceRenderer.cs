using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

namespace Fractrace.PictureArt {
  class NiceRenderer : FastScienceRenderer {

    /// <summary>
    /// Initialisierung
    /// </summary>
    /// <param name="pData"></param>
    public NiceRenderer(PictureData pData)
      : base(pData) {
    }


    private Vec3[,] normalesSmooth1 = null;

    private Vec3[,] normalesSmooth2 = null;

    private Vec3[,] colorAmbient1 = null;

    private Vec3[,] colorAmbient2 = null;


    /// <summary>
    /// Allgemeine Informationen werden erzeugt
    /// PreCalculate sollte parallel ausgeführt werden. Bei GetColor ist das nicht mehr
    /// möglich, da die Farbinformationen auf das Graphic-Objekt geschrieben werden.
    /// </summary>
    protected override void PreCalculate() {
      normalesSmooth1=new Vec3[pData.Width,pData.Height];
      normalesSmooth2 = new Vec3[pData.Width, pData.Height];


      // Normieren
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          Vec3 center = null;
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            pInfo.Normal.Normalize();
          }
        }
      }

      for(int i=0;i<pData.Width;i++) {
        for (int j = 0; j < pData.Height;j++ ) {
          Vec3 center = null;
          PixelInfo pInfo = pData.Points[i, j];
             if (pInfo != null) {
               center = pInfo.Normal;
              }
          // Test ohne smooth-Factor
          // Nachbarelemente zusammenrechnen
             Vec3 neighbors = new Vec3();
             int neighborFound = 0;
          
             for (int k = -1; k <= 1; k++) {
               for (int l = -1; l <= 1; l++) {
                 int posX = i + k;
                 int posY = j + l;
                 if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                   PixelInfo pInfoBorder = pData.Points[posX, posY];
                   if (pInfoBorder != null) {
                     neighbors.Add(pInfoBorder.Normal);
                     neighborFound++;
                   }
                 }
               }
             }
          neighbors.Normalize();
          if (center != null) {
            normalesSmooth1[i, j] = center;
            if (center != null || neighborFound > 4) {
              neighbors.Add(center.Mult(1.1));
              neighbors.Normalize();
              normalesSmooth1[i, j] = neighbors;
            }
          } else {
            if (neighborFound > 4) {
              normalesSmooth1[i, j] = neighbors;
            }
          }

        }

      }
    }


    /// <summary>
    /// Liefert die Farbe zum Punkt x,y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected override Vec3 GetRgbAt(int x, int y) {
      Vec3 retVal = new Vec3(0, 0, 1); // rot
      /*
           PixelInfo pInfo = pData.Points[x, y];
           if (pInfo == null) {
             return retVal;
           }
       */
      Vec3 normal = normalesSmooth1[x, y];
      if (normal == null) {
        return new Vec3(0, 0, 0);
      }

      Vec3 light = new Vec3(0, 0, 0);
      light = GetLight(normal);

      retVal.X = light.X;
      retVal.Y = light.Y;
      retVal.Z = light.Z;

      return retVal;
    }






  }
}

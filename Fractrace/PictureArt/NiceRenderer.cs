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
      for(int i=0;i<pData.Width;i++) {
        for (int j = 0; j < pData.Height;j++ ) {
          Vec3 center = null;
          PixelInfo pInfo = pData.Points[i, j];
             if (pInfo != null) {
               center = pInfo.Normal;
              }
          // Test ohne smooth-Factor
             normalesSmooth1[i, j] = center;
        }

      }
    }

    protected override Vec3 GetRgbAt(int x, int y) {
      Vec3 retVal = new Vec3(0, 0, 1); // rot
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

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

    private double[,] smoothDeph1 = null;

    private double[,] smoothDeph2 = null;

    private double[,] smoothDeph3 = null;

    private double minY = double.MaxValue;

    private double maxY = double.MinValue;

    /// <summary>
    /// Allgemeine Informationen werden erzeugt
    /// PreCalculate sollte parallel ausgeführt werden. Bei GetColor ist das nicht mehr
    /// möglich, da die Farbinformationen auf das Graphic-Objekt geschrieben werden.
    /// </summary>
    protected override void PreCalculate() {
      CreateSmoothNormales();
      CreateSmoothDeph();
    }

    protected void CreateSmoothNormales() {
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

      // normalesSmooth1 erzeugen
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

      // normalesSmooth2 erzeugen
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          Vec3 center = normalesSmooth1[i,j];
          // Test ohne smooth-Factor
          // Nachbarelemente zusammenrechnen
          Vec3 neighbors = new Vec3();
          int neighborFound = 0;

          for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
              int posX = i + k;
              int posY = j + l;
              if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                Vec3 newNormal =normalesSmooth1[posX, posY];
                if (newNormal != null) {
                  neighbors.Add(newNormal);
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
              normalesSmooth2[i, j] = neighbors;
            }
          } else {
            if (neighborFound > 4) {
              normalesSmooth2[i, j] = neighbors;
            }
          }

        }

      }
    }


    /// <summary>
    /// Lokale Tiefeninformationen werden erzeugt.
    /// </summary>
    protected void CreateSmoothDeph() {
      smoothDeph1 = new double[pData.Width, pData.Height];
      smoothDeph2 = new double[pData.Width, pData.Height];
      smoothDeph3 = new double[pData.Width, pData.Height];

      // Normieren
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            smoothDeph1[i, j] = pInfo.Coord.Y;
            if (minY > pInfo.Coord.Y)
              minY = pInfo.Coord.Y;
            if (maxY < pInfo.Coord.Y)
              maxY = pInfo.Coord.Y;
          } else smoothDeph1[i, j] = double.MinValue;
        }
      }

      SetSmoothDeph(smoothDeph1, smoothDeph2);
      SetSmoothDeph(smoothDeph2, smoothDeph3);
      SetSmoothDeph(smoothDeph3, smoothDeph1);
      SetSmoothDeph(smoothDeph1, smoothDeph3);
      SetSmoothDeph(smoothDeph3, smoothDeph1);
      SetSmoothDeph(smoothDeph1, smoothDeph3);
      SetSmoothDeph(smoothDeph3, smoothDeph1); 
    }


    protected void SetSmoothDeph(double[,] sdeph1,double[,] sdeph2) {

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          int neighborFound = 0;
          double smoothDeph = 0;
          sdeph2[i, j] = double.MinValue;
          for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
              int posX = i + k;
              int posY = j + l;
              if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                double newDeph = sdeph1[posX, posY];
                if (newDeph != double.MinValue) {
                  smoothDeph += newDeph;
                  neighborFound++;
                }
              }
            }
          }
          if (neighborFound > 0) {
            smoothDeph /= (double)neighborFound;
            sdeph2[i, j] = smoothDeph;
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
      Vec3 normal = normalesSmooth2[x, y];
      if (normal == null) {
        return new Vec3(0, 0, 0);
      }

      Vec3 light = new Vec3(0, 0, 0);
      light = GetLight(normal);

      retVal.X = light.X;
      retVal.Y = light.Y;
      retVal.Z = light.Z;

      // Lokale Tiefeninfo hinzurechnen
      if(smoothDeph2[x, y]!=double.MinValue && smoothDeph1[x, y]!=double.MinValue ) {
        double localDeph = smoothDeph2[x, y] - smoothDeph1[x, y];
        double testVar = 100;
        localDeph = localDeph * testVar;
        if (localDeph > 1)
          localDeph = 1;
        if (localDeph < -1)
          localDeph = -1;
        if (localDeph > 0)
          localDeph = localDeph/3.0;
        

        retVal.Z = 0.5 * light.Z + 0.5 *localDeph* light.Z;
        retVal.X = 0.5 * light.X + 0.5 * localDeph * light.X;
        retVal.Y = 0.5 * light.Y + 0.5 * localDeph * light.Y;
      }

      return retVal;
    }






  }
}

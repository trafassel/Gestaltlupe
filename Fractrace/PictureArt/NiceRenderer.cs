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


    /// <summary>
    /// Tiefe, wenn Schatten von unten rechts kommt.
    /// </summary>
    private double[,] shadowInfo11 = null;
    private double[,] shadowInfo10 = null;
    private double[,] shadowInfo01 = null;
    private double[,] shadowInfo00 = null;
    private double[,] shadowInfo11v = null;
    private double[,] shadowInfo10v = null;
    private double[,] shadowInfo01h = null;
    private double[,] shadowInfo00h = null;

    private double[,] shadowPlane = null;



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
      CreateShadowInfo();
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
    /// Schatteninformationen, wenn das Licht von oben rechts kommen, werden erzeugt.
    /// </summary>
    protected void CreateShadowInfo() {
      // Beginnend von rechts oben werden die Bereiche, die im Dunklen liegen, berechnet.
      shadowInfo11 = new double[pData.Width, pData.Height];
      shadowInfo01 = new double[pData.Width, pData.Height]; 
      shadowInfo10 = new double[pData.Width, pData.Height]; 
      shadowInfo00 = new double[pData.Width, pData.Height];
      shadowInfo11v = new double[pData.Width, pData.Height];
      shadowInfo01h = new double[pData.Width, pData.Height];
      shadowInfo10v = new double[pData.Width, pData.Height];
      shadowInfo00h = new double[pData.Width, pData.Height];
      shadowPlane = new double[pData.Width, pData.Height];
      double[,] shadowTempPlane= new double[pData.Width, pData.Height];

      double diffy = maxY - minY;
      double yd = diffy / ((double)(pData.Width + pData.Height));
      double ydv = diffy / ((double)(pData.Height));
      double ydh = diffy / ((double)(pData.Width));

      yd *= 4.0; ydv *= 4.0; ydh *= 4.0;

        for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          shadowInfo11[i, j] = smoothDeph2[i, j];
          shadowInfo10[i, j] = smoothDeph2[i, j];
          shadowInfo01[i, j] = smoothDeph2[i, j];
          shadowInfo00[i, j] = smoothDeph2[i, j];
          shadowInfo11v[i, j] = smoothDeph2[i, j];
          shadowInfo10v[i, j] = smoothDeph2[i, j];
          shadowInfo01h[i, j] = smoothDeph2[i, j];
          shadowInfo00h[i, j] = smoothDeph2[i, j];

        }
         }
        for (int k = 0; k < 100; k++) {
          for (int i = 0; i < pData.Width; i++) {
            for (int j = 0; j < pData.Height; j++) {

              // Licht von unten rechts
              if (i < pData.Width - 1 && j < pData.Height - 1) {
                double localShadow = shadowInfo11[i + 1, j + 1] - yd;
                if (localShadow > shadowInfo11[i, j])
                  shadowInfo11[i, j] = localShadow;
              }

              // Licht von oben rechts
              if (i < pData.Width - 1 && j > 0) {
                double localShadow = shadowInfo10[i + 1, j - 1] - yd;
                if (localShadow > shadowInfo10[i, j])
                  shadowInfo10[i, j] = localShadow;
              }

              // Licht von unten links
              if (i >0 && j < pData.Height - 1) {
                double localShadow = shadowInfo01[i - 1, j + 1] - yd;
                if (localShadow > shadowInfo01[i, j])
                  shadowInfo01[i, j] = localShadow;
              }

              // Licht von oben links
              if (i >0 && j >0) {
                double localShadow = shadowInfo00[i - 1, j - 1] - yd;
                if (localShadow > shadowInfo00[i, j])
                  shadowInfo00[i, j] = localShadow;
              }

              // Licht von links
              if (i > 0) {
                double localShadow = shadowInfo00h[i - 1, j] - ydh;
                if (localShadow > shadowInfo00h[i, j])
                  shadowInfo00h[i, j] = localShadow;
              }


              // Licht von rechts
              if (i < pData.Width - 1) {
                double localShadow = shadowInfo01h[i + 1, j] - ydh;
                if (localShadow > shadowInfo01h[i, j])
                  shadowInfo01h[i, j] = localShadow;
              }

              // Licht von oben
              if (j < pData.Height - 1) {
                double localShadow = shadowInfo10v[i, j+1] - ydv;
                if (localShadow > shadowInfo10v[i, j])
                  shadowInfo10v[i, j] = localShadow;
              }

              // Licht von unten
              if (j >0) {
                double localShadow = shadowInfo11v[i, j - 1] - ydv;
                if (localShadow > shadowInfo11v[i, j])
                  shadowInfo11v[i, j] = localShadow;
              }



            }
          }
        }

      // Die shadowplane aufbauen
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            shadowTempPlane[i, j] = 1;
            if (smoothDeph2[i, j] != double.MinValue) {
              double tempdiff = 0;
              if (smoothDeph2[i, j] < shadowInfo11[i, j]) {
                tempdiff++;
              }
              if (smoothDeph2[i, j] < shadowInfo01[i, j]) {
                tempdiff++;
              }

              if (smoothDeph2[i, j] < shadowInfo10[i, j]) {
                tempdiff++;
              }

              if (smoothDeph2[i, j] < shadowInfo00[i, j]) {
                tempdiff++;
              }
              if (smoothDeph2[i, j] < shadowInfo11v[i, j]) {
                tempdiff++;
              }
              if (smoothDeph2[i, j] < shadowInfo01h[i, j]) {
                tempdiff++;
              }

              if (smoothDeph2[i, j] < shadowInfo10v[i, j]) {
                tempdiff++;
              }

              if (smoothDeph2[i, j] < shadowInfo00h[i, j]) {
                tempdiff++;
              }

              shadowTempPlane[i, j] = 1 - tempdiff / 8.0;
            }
          }
        }

        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            // TODO: smooth
            shadowPlane[i,j]=shadowTempPlane[i,j];
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

      double ydiff = maxY - minY;
      // Lokale Tiefeninfo hinzurechnen
      double localDeph = 0;
      if(smoothDeph2[x, y]!=double.MinValue && smoothDeph1[x, y]!=double.MinValue ) {
        localDeph = smoothDeph2[x, y] - smoothDeph1[x, y];
     
        double testVar = 50.0/ydiff;
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

      /*
      if(smoothDeph2[x, y]!=double.MinValue) {
        double yd = ydiff / ((double)(pData.Width + pData.Height));
        yd *= 4.0;
        double shadowfactor = 0;
        double tempdiff = 0;
        
        if (smoothDeph2[x, y] < shadowInfo11[x, y]) {
          tempdiff++;
        }
        
        
        if (smoothDeph2[x, y] < shadowInfo01[x, y]) {
          tempdiff++;
        }
        
          if (smoothDeph2[x, y] < shadowInfo10[x, y]) {
          tempdiff++;
          }
          if (smoothDeph2[x, y] < shadowInfo00[x, y]) {
            tempdiff++;
          }

          // TODO: den Unterschied von smoothDeph2 und smoothDeph1 einfließen lassen
        // wenn dieser groß ist, hat auch der Schatten großen Einfluss
        // also die Var localdeph
          if (localDeph > 0)
            localDeph = 0;
          localDeph /= -4.0;
          double newFac = (4.0 -(0.75+localDeph)* tempdiff) / 4.0;
       */
      double newFac =shadowPlane[x,y];
          retVal.Z = newFac * retVal.Z;
          retVal.X = newFac * retVal.X;
          retVal.Y = newFac * retVal.Y;


      return retVal;
    }






  }
}

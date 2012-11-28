using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;


namespace Fractrace.PictureArt {
  class SharpRenderer : FastScienceRenderer {

    /// <summary>
    /// Initialisierung
    /// </summary>
    /// <param name="pData"></param>
    public SharpRenderer(PictureData pData)
      : base(pData) {
    }

    protected bool useAmbient = false;

    protected bool useDarken = false;

    private double borderMinY = 0;

    private double borderMaxY = 0;


    private Vec3[,] normalesSmooth1 = null;

    /// <summary>
    /// Zusatzinformationen zum Bild.
    /// 0 keine info
    /// 1 Element der Schnittmenge mit dem Bildschirm
    /// </summary>
    private int[,] picInfo = null;

    private Vec3[,] normalesSmooth2 = null;



    private Vec3[,] rgbPlane = null;

    private Vec3[,] rgbSmoothPlane1 = null;
    private Vec3[,] rgbSmoothPlane2 = null;

    private double[,] smoothDeph1 = null;

    private double[,] smoothDeph2 = null;


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

    // Abstand zum Schatten
    private double[,] shadowInfo11dist = null;
    private double[,] shadowInfo10dist = null;
    private double[,] shadowInfo01dist = null;
    private double[,] shadowInfo00dist = null;
    private double[,] shadowInfo11vdist = null;
    private double[,] shadowInfo10vdist = null;
    private double[,] shadowInfo01hdist = null;
    private double[,] shadowInfo00hdist = null;

    private double[,] shadowPlane = null;



    private double minY = double.MaxValue;

    private double maxY = double.MinValue;

 

    /// <summary>
    /// Allgemeine Informationen werden erzeugt
    /// PreCalculate sollte parallel ausgeführt werden. Bei GetColor ist das nicht mehr
    /// möglich, da die Farbinformationen auf das Graphic-Objekt geschrieben werden.
    /// </summary>
    protected override void PreCalculate() {
      useAmbient = ParameterDict.Exemplar.GetBool("Composite.UseAmbient");
      useDarken = ParameterDict.Exemplar.GetBool("Composite.UseDarken");
      borderMinY = ParameterDict.Exemplar.GetDouble("Border.Min.y");
      borderMaxY = ParameterDict.Exemplar.GetDouble("Border.Max.y");
      picInfo = new int[pData.Width, pData.Height];
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          picInfo[i, j] = 0;
        }
      }
      EstimateColorIsDefinedInFormula();
      CreateSmoothNormales();
      CreateSmoothDeph();
      CreateShadowInfo();
      DrawPlane();
      SmoothEmptyPixel();
      if (ParameterDict.Exemplar.GetBool("Composite.Normalize"))
        NormalizePlane();
      if (useDarken)
        DarkenPlane();
      if (useAmbient)
        SmoothPlane();
      //  ParameterDict.Exemplar["Composite.Normalize"] = "1";


    }

    /// <summary>
    /// Bildpunkte, die auf Grund fehlender Informationen nicht geladen werden konnten, werden
    /// aus den Umgebungsinformationen gemittelt.
    /// </summary>
    protected void SmoothEmptyPixel() {
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];

          if (pInfo == null) { // Dieser Wert ist zu setzen
             Vec3 col= rgbPlane[i, j];
             col.X = 0; col.Y = 0; col.Z = 0;
            double pixelCount = 0;
            for (int k = i - 1; k <= i + 1; k++) {
              for (int l = j - 1; l <= j + 1; l++) {
                if (k >= 0 && k < pData.Width && l >= 0 && l < pData.Height) {
                  PixelInfo pInfo2 = pData.Points[k, l];
                  if (pInfo2 != null) {
                    pixelCount++;
                    Vec3 otherColor = rgbPlane[k, l];
                    col.X += otherColor.X;
                    col.Y += otherColor.Y;
                    col.Z += otherColor.Z;
                  }
                }
              }
            }
            pixelCount++; // Etwas dunkler sollte es schon werden
            if (pixelCount > 0) {
              col.X /= pixelCount;
              col.Y /= pixelCount;
              col.Z /= pixelCount;
            }
          }
        }
      }
    }


    /// <summary>
    /// Bestimmt, ob in der Formel zusätzliche Farbinformationen festgelegt wurden
    /// und setzt die Farbe colorIsDefinedInFormula entsprechend.
    /// </summary>
    protected void EstimateColorIsDefinedInFormula() {
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
         

        }
      }
    }


    /// <summary>
    /// Die Gestalt wird nach hinten abgedunkelt.
    /// </summary>
    protected void DarkenPlane() {
      double mainDeph = maxY - minY;// borderMaxY - borderMinY;
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          Vec3 col = rgbPlane[i, j];
          double yd = smoothDeph2[i, j];
          if (yd == double.MinValue)
            yd = minY;
          double ydNormalized = (yd - minY) / mainDeph;
          ydNormalized = Math.Sqrt(ydNormalized);
          ydNormalized *= 2 * ydNormalized;
          //System.Diagnostics.Debug.WriteLine("ydNormalized=" + ydNormalized.ToString());
          if (ydNormalized > 0.95) {
            ydNormalized = 0.95;
          }
          ydNormalized += 0.05;
          // ydNormalized = Math.Sqrt(ydNormalized);
          col.X = col.X * ydNormalized;
          col.Y = col.Y * ydNormalized;
          ydNormalized += 0.05;
          col.Z = ydNormalized * col.Z;

        }
      }
    }

    /// <summary>
    /// Unschärfe wird dazugerechnet.
    /// </summary>
    protected void SmoothPlane() {
      rgbSmoothPlane1 = new Vec3[pData.Width, pData.Height];
      rgbSmoothPlane2 = new Vec3[pData.Width, pData.Height];
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          rgbSmoothPlane2[i, j] = rgbPlane[i, j];
        }
      }

      double mainDeph = maxY - minY;// borderMaxY - borderMinY;


      for (int m = 0; m < 10; m++) {

        // Zweiter Durchlauf
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            double neighborsFound = 0;
            Vec3 nColor = new Vec3();
            for (int k = -1; k <= 1; k++) {
              for (int l = -1; l <= 1; l++) {
                int posX = i + k;
                int posY = j + l;
                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                  nColor.Add(rgbSmoothPlane2[posX, posY]);
                  neighborsFound++;
                }
              }
            }
            if (neighborsFound > 0)
              nColor = nColor.Mult(1 / neighborsFound);
            //Vec3 nCenterColor = rgbSmoothPlane2[i, j];
            //nCenterColor.Add(nColor);
            //nCenterColor = nCenterColor.Mult(0.5);

            double yd = smoothDeph2[i, j];
            if (yd == double.MinValue)
              yd = minY;
            double ydNormalized = (yd - minY) / mainDeph;
            ydNormalized = ydNormalized * ydNormalized;
            ydNormalized = ydNormalized - 0.5;
            ydNormalized = 2.0 * Math.Abs(ydNormalized);
            if (ydNormalized > 1)
              ydNormalized = 1;
            if (ydNormalized < 0)
              ydNormalized = 0;

            ydNormalized = 1 - ydNormalized;
            //   ydNormalized = 0.5;
            Vec3 nCenterColor = rgbSmoothPlane2[i, j];
            //  ydNormalized *= ydNormalized;
            /*ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;*/
            //ydNormalized = 0;
            ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = Math.Sqrt(ydNormalized);
            nCenterColor = nCenterColor.Mult(ydNormalized);
            nColor = nColor.Mult(1.0 - ydNormalized);
            nCenterColor.Add(nColor);

            rgbSmoothPlane1[i, j] = nCenterColor;
          }
        }


        // Dritter Durchlauf
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            double neighborsFound = 0;
            Vec3 nColor = new Vec3();
            for (int k = -1; k <= 1; k++) {
              for (int l = -1; l <= 1; l++) {
                int posX = i + k;
                int posY = j + l;
                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                  nColor.Add(rgbSmoothPlane1[posX, posY]);
                  neighborsFound++;
                }
              }
            }
            if (neighborsFound > 0)
              nColor = nColor.Mult(1 / neighborsFound);

            double yd = smoothDeph2[i, j];
            if (yd == double.MinValue)
              yd = minY;
            double ydNormalized = (yd - minY) / mainDeph;
            ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = ydNormalized * ydNormalized;
            if (ydNormalized > 0.2) {

            }
            ydNormalized = ydNormalized - 0.8;
            ydNormalized = 2.0 * Math.Abs(ydNormalized);
            if (ydNormalized > 1)
              ydNormalized = 1;
            if (ydNormalized < 0)
              ydNormalized = 0;
            ydNormalized = 1 - ydNormalized;
            //   ydNormalized = 0.5;
            Vec3 nCenterColor = rgbSmoothPlane1[i, j];
            //ydNormalized = 0;
            // ydNormalized *= ydNormalized;
            /*
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;
            ydNormalized *= ydNormalized;*/
            //ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = Math.Sqrt(ydNormalized);
            nCenterColor = nCenterColor.Mult(ydNormalized);
            nColor = nColor.Mult(1.0 - ydNormalized);
            nCenterColor.Add(nColor);

            //Vec3 nCenterColor = rgbSmoothPlane1[i, j];
            //nCenterColor.Add(nColor);
            //nCenterColor = nCenterColor.Mult(0.5);
            rgbSmoothPlane2[i, j] = nCenterColor;
          }
        }
      }

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          rgbPlane[i, j] = rgbSmoothPlane2[i, j];
        }
      }
    }


    /// <summary>
    /// Erzeugt das Bild im rgb-Format
    /// </summary>
    protected void DrawPlane() {
      rgbPlane = new Vec3[pData.Width, pData.Height];
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          rgbPlane[i, j] = GetRgb(i, j);
        }
      }
    }


    /// <summary>
    /// Weißabgleich und Helligkeitskorrektur.
    /// </summary>
    protected void NormalizePlane() {
      double maxRed = 0;
      double maxGreen = 0;
      double maxBlue = 0;

      for (int i = 1; i < pData.Width - 1; i++) {
        for (int j = 1; j < pData.Height - 1; j++) {
          if ((picInfo[i, j] == 0) && (picInfo[i + 1, j] == 0) && (picInfo[i - 1, j] == 0) && (picInfo[i, j - 1] == 0) &&
              (picInfo[i, j + 1] == 0) && (picInfo[i - 1, j - 1] == 0) && (picInfo[i - 1, j + 1] == 0) && (picInfo[i + 1, j - 1] == 0) &&
              (picInfo[i - 1, j + 1] == 0)) {
            Vec3 col = rgbPlane[i, j];
            if (col.X > maxRed)
              maxRed = col.X;
            if (col.Y > maxGreen)
              maxGreen = col.Y;
            if (col.Z > maxBlue)
              maxBlue = col.Z;
          }
        }
      }
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          Vec3 col = rgbPlane[i, j];
          if (picInfo[i, j] == 0) {

            if (maxRed > 0)
              col.X /= maxRed;
            if (maxGreen > 0)
              col.Y /= maxGreen;
            if (maxBlue > 0)
              col.Z /= maxBlue;
          }
          if (col.X < 0)
            col.X = 0;
          if (col.X > 1)
            col.X = 1;
          if (col.Y < 0)
            col.Y = 0;
          if (col.Y > 1)
            col.Y = 1;
          if (col.Z < 0)
            col.Z = 0;
          if (col.Z > 1)
            col.Z = 1;

        }
      }

    }

    protected void CreateSmoothNormales() {
      normalesSmooth1 = new Vec3[pData.Width, pData.Height];
      normalesSmooth2 = new Vec3[pData.Width, pData.Height];

      // Normieren
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            pInfo.Normal.Normalize();
          }
        }
      }

      // normalesSmooth1 erzeugen
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
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
          Vec3 center = normalesSmooth1[i, j];
          // Test ohne smooth-Factor
          // Nachbarelemente zusammenrechnen
          Vec3 neighbors = new Vec3();
          int neighborFound = 0;

          for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
              int posX = i + k;
              int posY = j + l;
              if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                Vec3 newNormal = normalesSmooth1[posX, posY];
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
      //smoothDeph3 = new double[pData.Width, pData.Height];

      // Normieren
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            smoothDeph1[i, j] = pInfo.Coord.Y;
            if (pInfo.Coord.Y != 0) { // Unterscheidung, ob Schnitt mit Begrenzung vorliegt.
              if (minY > pInfo.Coord.Y)
                minY = pInfo.Coord.Y;
              if (maxY < pInfo.Coord.Y)
                maxY = pInfo.Coord.Y;
            }
          } else smoothDeph1[i, j] = double.MinValue;
        }
      }

      // Höhe der Begrenzungsfläche auf ymax setzen

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            if (pInfo.Coord.Y == 0) {
              smoothDeph1[i, j] = maxY;
            }
          }
        }
      }

      SetSmoothDeph(smoothDeph1, smoothDeph2);
      /*
        SetSmoothDeph(smoothDeph2, smoothDeph3);
        SetSmoothDeph(smoothDeph3, smoothDeph1);
        SetSmoothDeph(smoothDeph1, smoothDeph3);
        SetSmoothDeph(smoothDeph3, smoothDeph1);
        SetSmoothDeph(smoothDeph1, smoothDeph3);
        SetSmoothDeph(smoothDeph3, smoothDeph1);
       * */
    }


    protected virtual void SetSmoothDeph(double[,] sdeph1, double[,] sdeph2) {

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
    protected virtual void CreateShadowInfo() {
      // Beginnend von rechts oben werden die Bereiche, die im Dunklen liegen, berechnet.
      shadowInfo11 = new double[pData.Width, pData.Height];
      shadowInfo01 = new double[pData.Width, pData.Height];
      shadowInfo10 = new double[pData.Width, pData.Height];
      shadowInfo00 = new double[pData.Width, pData.Height];
      shadowInfo11v = new double[pData.Width, pData.Height];
      shadowInfo01h = new double[pData.Width, pData.Height];
      shadowInfo10v = new double[pData.Width, pData.Height];
      shadowInfo00h = new double[pData.Width, pData.Height];

      shadowInfo11dist = new double[pData.Width, pData.Height];
      shadowInfo01dist = new double[pData.Width, pData.Height];
      shadowInfo10dist = new double[pData.Width, pData.Height];
      shadowInfo00dist = new double[pData.Width, pData.Height];
      shadowInfo11vdist = new double[pData.Width, pData.Height];
      shadowInfo01hdist = new double[pData.Width, pData.Height];
      shadowInfo10vdist = new double[pData.Width, pData.Height];
      shadowInfo00hdist = new double[pData.Width, pData.Height];

      shadowPlane = new double[pData.Width, pData.Height];
      double[,] shadowTempPlane = new double[pData.Width, pData.Height];

      double diffy = maxY - minY;
      double yd = diffy / ((double)(pData.Width + pData.Height));
      double ydv = diffy / ((double)(pData.Height));
      double ydh = diffy / ((double)(pData.Width));

      yd *= 4.0; ydv *= 1.7; ydh *= 1.7;

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

          shadowInfo11dist[i, j] = 0;
          shadowInfo10dist[i, j] = 0;
          shadowInfo01dist[i, j] = 0;
          shadowInfo00dist[i, j] = 0;
          shadowInfo11vdist[i, j] = 0;
          shadowInfo10vdist[i, j] = 0;
          shadowInfo01hdist[i, j] = 0;
          shadowInfo00hdist[i, j] = 0;

        }
      }

      for (int i = pData.Width - 1; i >= 0; i--) {
        for (int j = pData.Height - 1; j >= 0; j--) {
          if (i < pData.Width - 1 && j < pData.Height - 1) {
            double localShadow = shadowInfo11[i + 1, j + 1] - yd;
            if (localShadow > shadowInfo11[i, j]) {
              shadowInfo11[i, j] = localShadow;
              shadowInfo11dist[i, j] = shadowInfo11dist[i + 1, j + 1] + 1;
            }
          }
          // Licht von rechts
          if (i < pData.Width - 1) {
            double localShadow = shadowInfo01h[i + 1, j] - ydh;
            if (localShadow > shadowInfo01h[i, j]) {
              shadowInfo01h[i, j] = localShadow;
              shadowInfo01hdist[i, j] = shadowInfo01hdist[i + 1, j] + 1;
            }
          }
          // Licht von oben
          if (j < pData.Height - 1) {
            double localShadow = shadowInfo10v[i, j + 1] - ydv;
            if (localShadow > shadowInfo10v[i, j])
              shadowInfo10v[i, j] = localShadow;
            shadowInfo10vdist[i, j] = shadowInfo10vdist[i, j + 1] + 1;
          }



        }
      }

      /*
      for (int k = 0; k < 300; k++) {
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {

            // Licht von unten rechts
            
            //if (i < pData.Width - 1 && j < pData.Height - 1) {
            //  double localShadow = shadowInfo11[i + 1, j + 1] - yd;
            //  if (localShadow > shadowInfo11[i, j])
            //    shadowInfo11[i, j] = 0.5 * (shadowInfo11[i, j] + localShadow);
            //}

            // Licht von oben rechts
            if (i < pData.Width - 1 && j > 0) {
              double localShadow = shadowInfo10[i + 1, j - 1] - yd;
              if (localShadow > shadowInfo10[i, j])
                shadowInfo10[i, j] = 0.5 * (shadowInfo10[i, j] + localShadow);
            }

            // Licht von unten links
            if (i > 0 && j < pData.Height - 1) {
              double localShadow = shadowInfo01[i - 1, j + 1] - yd;
              if (localShadow > shadowInfo01[i, j])
                shadowInfo01[i, j] = 0.5 * (shadowInfo01[i, j] + localShadow);
            }

            // Licht von oben links
            if (i > 0 && j > 0) {
              double localShadow = shadowInfo00[i - 1, j - 1] - yd;
              if (localShadow > shadowInfo00[i, j])
                shadowInfo00[i, j] = 0.5 * (shadowInfo00[i, j] + localShadow);
            }

            // Licht von links
            if (i > 0) {
              double localShadow = shadowInfo00h[i - 1, j] - ydh;
              if (localShadow > shadowInfo00h[i, j])
                shadowInfo00h[i, j] = 0.5 * (shadowInfo00h[i, j] + localShadow);
            }

            
            // Licht von rechts
            //if (i < pData.Width - 1) {
            //  double localShadow = shadowInfo01h[i + 1, j] - ydh;
            //  if (localShadow > shadowInfo01h[i, j])
            //    shadowInfo01h[i, j] = 0.5 * (shadowInfo01h[i, j] + localShadow);
            //}
            
            // Licht von oben
            
            //if (j < pData.Height - 1) {
            //  double localShadow = shadowInfo10v[i, j + 1] - ydv;
            //  if (localShadow > shadowInfo10v[i, j])
            //    shadowInfo10v[i, j] = 0.5 * (shadowInfo10v[i, j] + localShadow);
            //}

            // Licht von unten
            if (j > 0) {
              double localShadow = shadowInfo11v[i, j - 1] - ydv;
              if (localShadow > shadowInfo11v[i, j])
                shadowInfo11v[i, j] = 0.5 * (shadowInfo11v[i, j] + localShadow);
            }



          }
        }
      }
       */

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
            if (shadowTempPlane[i, j] < 0)
              shadowTempPlane[i, j] = 0;
          }
        }
      }

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          // TODO: smooth
          shadowPlane[i, j] = shadowTempPlane[i, j];
          double sCount = 0;
          double sPlaneEntry = 0;
          for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
              int posX = i + k;
              int posY = j + l;
              if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                sCount++;
                sPlaneEntry += shadowTempPlane[posX, posY];
              }
            }
          }
          if (sCount > 0)
            shadowPlane[i, j] = 0.4 * shadowTempPlane[i, j] + 0.6 * sPlaneEntry / sCount;
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
      return rgbPlane[x, y];
    }


    /// <summary>
    /// Liefert die Farbe zum Punkt x,y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected Vec3 GetRgb(int x, int y) {
      // TODO: Wenn Schnitt mit Begrenzung vorliegt, soll die Anzahl der durchgeführten Iterationen als
      // Farbwert dargestellt werden.
      Vec3 retVal = new Vec3(0, 0, 0); // blau

      PixelInfo pInfo = pData.Points[x, y];
      
      if (pInfo != null) {
        if (pInfo.iterations >= 0) {
          // return retVal;

          double it = -pInfo.frontLight / 255.0;
          if (it > 1)
            it = 1;
          retVal.X = it;
          retVal.Y = 0.2 + 0.8 * it;
          retVal.Z = it;

          picInfo[x, y] = 1;

          return retVal;
        }
      }

      // debug only
      if (picInfo == null) {
        retVal.X = 1;
        return retVal;
      }

      /*
      retVal.Y = 1;
      return retVal;
      */
      // ende debug only

      Vec3 normal = normalesSmooth2[x, y];
      if (normal == null) {
        return new Vec3(0, 0, 0);
      }

      Vec3 light = new Vec3(0, 0, 0);

      light = GetLight(normal);
      double derivation = 0;
      Vec3 derivationVec = normalesSmooth1[x, y];
      if (derivationVec != null) {
        Vec3 der = derivationVec.Diff(normal);
        derivation = der.Norm;
      }

      light.X += derivation / 2.0;
      retVal.X = light.X;
      retVal.Y = light.Y;
      retVal.Z = light.Z;

      // debug only
      if (pInfo == null) {
        retVal.X = 0;
        retVal.Y = 1;
        retVal.Z = 0;
        return retVal;
      }

      double ydiff = maxY - minY;
      // Lokale Tiefeninfo hinzurechnen
      double localDeph = 0;
      if (smoothDeph2[x, y] != double.MinValue && smoothDeph1[x, y] != double.MinValue) {
        localDeph = smoothDeph2[x, y] - smoothDeph1[x, y];

        double testVar = 50.0 / ydiff;
        localDeph = localDeph * testVar;
        if (localDeph > 1)
          localDeph = 1;
        if (localDeph < -1)
          localDeph = -1;
        if (localDeph > 0)
          localDeph = localDeph / 3.0;

        retVal.Z = 0.5 * light.Z + 0.5 * localDeph * light.Z;
        retVal.X = 0.5 * light.X + 0.5 * localDeph * light.X;
        retVal.Y = 0.5 * light.Y + 0.5 * localDeph * light.Y;
      }

      double newFac = shadowPlane[x, y];
      retVal.Z = newFac * retVal.Z;
      retVal.X = newFac * retVal.X;
      retVal.Y = (0.02 + 0.98 * newFac) * retVal.Y;

      // Falls weitere Farbinformation vorhanden sind, werden diese nun auf die Punkte angewendet
      bool useAdditionalColorinfo = true;
      if (useAdditionalColorinfo && pInfo != null && pInfo.AdditionalInfo != null) {
        // Normalisieren;
        double r1 = pInfo.AdditionalInfo.red;
        double g1 = pInfo.AdditionalInfo.green;
        double b1 = pInfo.AdditionalInfo.blue;
        if (r1 < 0)
          r1 = -r1;
        if (g1 < 0)
          g1 = -g1;
        if (b1 < 0)
          b1 = -b1;
        double minr = r1;
        if (minr > g1)
          minr = g1;
        if (minr > b1)
          minr = b1;

        //r1 -= minr;
        //g1 -= minr;
        //b1 -= minr;

        double norm = Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1) / Math.Sqrt(3.0);
        if (norm != 0) {
          retVal.X *= (r1) / norm;
          retVal.Y *= (g1) / norm;
          retVal.Z *= (b1) / norm;
        }
      } 
      if(pInfo==null) {
        retVal.X =0;
        retVal.Y = 0;
        retVal.Z = 0;
      }
      return retVal;
    }


    /// <summary>
    /// Liefert die Farbe der Oberfläche entsprechend der Normalen.
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    protected override Vec3 GetLight(Vec3 normal) {
      Vec3 retVal = new Vec3(0, 0, 0);
      if (normal == null)
        return retVal;

      double norm = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
      /* Der Winkel ist nun das Skalarprodukt mit (0,-1,0)= Lichtstrahl */
      /* mit Vergleichsvektor (Beide nachträglich normiert )             */
      double winkel = 0;
      if (norm == 0)
        return retVal;


      // erste Lichtquelle
      // 0 -1 0  
      winkel = Math.Acos((normal.Y) / norm);
      winkel = 1 - winkel;

      if (winkel < 0)
        winkel = 0;
      if (winkel > 1)
        winkel = 1;

      winkel *= winkel;


      // Zum Test nur zweite Lichtquelle
      winkel = 0; // Frontlight wird ausgeschaltet.
      // Zweite Lichtquelle
      // 1 -1 1  
      double norm2 = Math.Sqrt(3.0);
      double winkel2 = Math.Acos((normal.X + normal.Y + normal.Z) / (norm * norm2));
      winkel2 = 1 - winkel2;

      if (winkel2 < 0)
        winkel2 = 0;
      if (winkel2 > 1)
        winkel2 = 1;

      winkel2 *= winkel2;

      winkel += winkel2;

      // Dritte Lichtquelle
      // 0 -1 1  
      double norm3 = Math.Sqrt(2.0);
      double winkel3 = Math.Acos((normal.Y + normal.Z) / (norm * norm3));
      winkel3 = 1 - winkel3;

      if (winkel3 < 0)
        winkel3 = 0;
      if (winkel3 > 1)
        winkel3 = 1;

      winkel3 *= winkel3;

      winkel += winkel3;

      // Vierte Lichtquelle
      // 0 -1 1  
      double norm4 = Math.Sqrt(2.0);
      double winkel4 = Math.Acos((normal.X + normal.Y) / (norm * norm4));
      winkel4 = 1 - winkel4;

      if (winkel4 < 0)
        winkel4 = 0;
      if (winkel4 > 1)
        winkel4 = 1;

      winkel4 *= winkel4;

      winkel += winkel4;

      // fünfte Lichtquelle
      // -1 -1 1  
      double norm5 = Math.Sqrt(3.0);
      double winkel5 = Math.Acos((-normal.X + normal.Y + normal.Z) / (norm * norm5));
      winkel5 = 1 - winkel5;

      if (winkel5 < 0)
        winkel5 = 0;
      if (winkel5 > 1)
        winkel5 = 1;

      winkel5 *= winkel5;

      winkel += 0.1 * winkel5;

      // 6. Lichtquelle
      // 0 -1 -1  
      double norm6 = Math.Sqrt(2.0);
      double winkel6 = Math.Acos((normal.Y - normal.Z) / (norm * norm3));
      winkel6 = 1 - winkel6;

      if (winkel6 < 0)
        winkel6 = 0;
      if (winkel6 > 1)
        winkel6 = 1;

      winkel6 *= winkel6;

      winkel += 0.1 * winkel6;

      // 7. Lichtquelle
      // -1 -1 0  
      double norm7 = Math.Sqrt(2.0);
      double winkel7 = Math.Acos((-normal.X + normal.Y) / (norm * norm7));
      winkel7 = 1 - winkel7;

      if (winkel7 < 0)
        winkel7 = 0;
      if (winkel7 > 1)
        winkel7 = 1;

      winkel7 *= winkel7;

      winkel += 0.1 * winkel7;
      //            winkel /= 1.7;
      winkel /= 1.7;
      if (winkel > 1)
        winkel = 1;
      if (winkel < 0)
        winkel = 0;

      retVal.X = winkel;
      retVal.Y = winkel;
      retVal.Z = winkel;
      /*
            retVal.X = 1;
            retVal.Y = 1;
            retVal.Z = 1;
      */
      return retVal;
    }





  }
}
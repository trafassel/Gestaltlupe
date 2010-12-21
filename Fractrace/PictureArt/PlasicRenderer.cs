using System;
using System.Collections.Generic;
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
  public class PlasicRenderer : ScienceRendererBase {

    /// <summary>
    /// Initialisierung
    /// </summary>
    /// <param name="pData"></param>
    public PlasicRenderer(PictureData pData)
      : base(pData) {
    }

    private Vec3[,] normalesSmooth1 = null;


    /// <summary>
    /// Zusatzinformationen zum Bild.
    /// 0 keine info
    /// 1 Element der Schnittmenge mit dem Bildschirm
    /// </summary>
    private int[,] picInfo = null;


    private Vec3[,] normalesSmooth2 = null;

    private double[,] shadowInfo11 = null;
    private double[,] shadowInfo10 = null;
    private double[,] shadowInfo01 = null;
    private double[,] shadowInfo00 = null;
    private double[,] shadowInfo11sharp = null;
    private double[,] shadowInfo10sharp = null;
    private double[,] shadowInfo01sharp = null;
    private double[,] shadowInfo00sharp = null;
    private double[,] shadowPlane = null;

    private double[,] smoothDeph1 = null;
    private double[,] smoothDeph2 = null;

    private Vec3[,] rgbPlane = null;

    private Vec3[,] rgbSmoothPlane1 = null;
    private Vec3[,] rgbSmoothPlane2 = null;

    private double minY = double.MaxValue;

    private double maxY = double.MinValue;

   
      // Corresponds to the number of shadows
    private int shadowNumber=1;
      // Intensity of the FieldOfView
    private int ambientIntensity=4;
      // Intensity of the Surface Color
    private double colorIntensity=0.5;

    /// <summary>
    /// Allgemeine Informationen werden erzeugt
    /// </summary>
    protected override void PreCalculate() {
      shadowNumber=ParameterDict.Exemplar.GetInt("Composite.Renderer.Plasic.ShadowNumber");
      ambientIntensity=ParameterDict.Exemplar.GetInt("Composite.Renderer.Plasic.AmbientIntensity");
      colorIntensity=ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorIntensity");
      picInfo = new int[pData.Width, pData.Height];
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          picInfo[i, j] = 0;
        }
      }
      CreateSmoothNormales();
      CreateSmoothDeph();
      CreateShadowInfo();
      DrawPlane();
      if(ParameterDict.Exemplar.GetBool("Composite.Renderer.Plasic.UseDarken"))
        DarkenPlane();
      SmoothEmptyPixel();
      SmoothPlane();
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
    /// Bildpunkte, die auf Grund fehlender Informationen nicht geladen werden konnten, werden
    /// aus den Umgebungsinformationen gemittelt.
    /// </summary>
    protected void SmoothEmptyPixel() {
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];

          if (pInfo == null) { // Dieser Wert ist zu setzen
            Vec3 col = rgbPlane[i, j];
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




    protected  Vec3 GetRgb(int x, int y) {
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
          if (retVal.Y < 0)
            retVal.Y = 0;
          if (retVal.Y > 255)
            retVal.Y = 255;

          retVal.Z = 0;

          return retVal;

        } catch (Exception ex) {
          System.Diagnostics.Debug.WriteLine(ex.ToString());
          retVal.X = 1;
          retVal.Y = 0;
          retVal.Z = 0;
          return retVal;

        }
      }

      Vec3 normal = normalesSmooth2[x, y];
      if (normal == null) { normal = pInfo.Normal; }
     if (normal == null)
       return new Vec3(0, 0, 0);

      if (pInfo.Normal != null) {
        light = GetLight(normal);
      }


      
      retVal.X = light.X;
      retVal.Y = light.Y;
      retVal.Z = light.Z;

      double d1 = maxY - minY;
      double d2 = pData.Width + pData.Height;
      double d3 = d1 / d2;


      double shadowlight = 0.34 * shadowPlane[x, y];
      retVal.Z = 0.2*retVal.Z+0.8*Math.Max(0,retVal.Z- shadowlight);
      retVal.X = 0.2 * retVal.X + 0.8 * Math.Max(0, retVal.X - shadowlight);
      retVal.Y = 0.2 * retVal.Y + 0.8 * Math.Max(0, retVal.Y - shadowlight);

      if (retVal.Y < 0)
        retVal.Y = 0;
      if (retVal.Z < 0)
        retVal.Z = 0;
      if (retVal.Y > 1)
        retVal.Y = 1;
      if (retVal.Z > 1)
        retVal.Z = 1;


      // Falls weitere Farbinformation vorhanden sind, werden diese nun auf die Punkte angewendet
      bool useAdditionalColorinfo = true;
      if (useAdditionalColorinfo) {
        if (pInfo != null && pInfo.AdditionalInfo != null) {
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

          //double norm = Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1);
          double norm = (r1+ g1 + b1);
          // etwas grauer machen
          r1 += (1- colorIntensity)* norm;
          g1 += (1 - colorIntensity) * norm;
          b1 += (1 - colorIntensity) * norm;
          norm = (r1 + g1 + b1)/3.0;

          if (norm != 0) {
            retVal.X *= (r1) / norm;
            retVal.Y *= (g1) / norm;
            retVal.Z *= (b1) / norm;
          }
        }
      }


      if (retVal.Y < 0)
        retVal.Y = 0;
      if (retVal.Z < 0)
        retVal.Z = 0;
      if (retVal.Y > 1)
        retVal.Y = 1;
      if (retVal.Z > 1)
        retVal.Z = 1;


      return retVal;
    }


    /// <summary>
    /// Liefert die Farbe der Oberfläche entsprechend der Normalen.
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    protected virtual Vec3 GetLight(Vec3 normal) {

      Vec3 retVal = new Vec3(0, 0, 0);
      return new Vec3(1, 1, 1);
      if (normal == null)
        return retVal;

      double shininess = 4;
      double weight_shini = 0.5;
      double weight_diffuse = 1 - weight_shini;
      double weight_frontLight = 0.1;
      double weight_second_light = 1 - weight_frontLight;
      double shininess_front = 1.2;
      double emissive = 0.3;

      double norm = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
      /* Der Winkel ist nun das Skalarprodukt mit (0,-1,0)= Lichtstrahl */
      /* mit Vergleichsvektor (Beide nachträglich normiert )             */
      double winkel = 0;
      if (norm == 0)
        return retVal;
      if (norm != 0)
        winkel = Math.Acos((normal.Y) / norm);
      winkel = 1 - winkel;

      if (winkel < 0)
        winkel = 0;
      if (winkel > 1)
        winkel = 1;

      winkel = Math.Pow(winkel, shininess_front);
    //  winkel = 0.5;
   
      // Zweite Lichtquelle
      // alt: 1 -1 1  
      // alt: 1 -2 1  
      double norm2 = Math.Sqrt(6.0);
      double winkel2 = Math.Acos((normal.X + 2.0*normal.Y + normal.Z) / (norm * norm2));


      winkel2 = 1 - winkel2;
      if (winkel2 < 0)
        winkel2 = 0;
      if (winkel2 > 1)
        winkel2 = 1;
      winkel2 = weight_diffuse*winkel2 + weight_shini*Math.Pow(winkel2, shininess);


      if (winkel2 < 0)
        winkel2 = 0;
      if (winkel2 > 1)
        winkel2 = 1;

      winkel *= 2.1;
      winkel2 *= 1.3;

      double colorComponent1 = 1.1 * weight_frontLight * winkel + 0.9 * weight_second_light * winkel2;
      double colorComponent2 = 0.9 * weight_frontLight * winkel + 1.1 * weight_second_light * winkel2;
      colorComponent1 = (1.0 - emissive) * colorComponent1 + emissive;
      colorComponent2 = (1.0 - emissive) * colorComponent2 + emissive;

      if (colorComponent1 > 1)
        colorComponent1 = 1;
      if (colorComponent1 < 0)
        colorComponent1 = 0;
      if (colorComponent2 > 1)
        colorComponent2 = 1;
      if (colorComponent2 < 0)
        colorComponent2 = 0;

      retVal.X = colorComponent1;
      retVal.Y = colorComponent1;
      retVal.Z = colorComponent2;

      return retVal;
    }


    /// <summary>
    /// Schatteninformationen, wenn das Licht von oben rechts kommen, werden erzeugt.
    /// Vorbereitet für weitere Lichtquellen.
    /// </summary>
    protected virtual void CreateShadowInfo() {
      double sharpness = 2.5; // 
      // Beginnend von rechts oben werden die Bereiche, die im Dunklen liegen, berechnet.
      shadowInfo11 = new double[pData.Width, pData.Height];
      shadowInfo01 = new double[pData.Width, pData.Height];
      shadowInfo10 = new double[pData.Width, pData.Height];
      shadowInfo00 = new double[pData.Width, pData.Height];
      shadowInfo11sharp = new double[pData.Width, pData.Height];
      shadowInfo01sharp = new double[pData.Width, pData.Height];
      shadowInfo10sharp = new double[pData.Width, pData.Height];
      shadowInfo00sharp = new double[pData.Width, pData.Height];

      shadowPlane = new double[pData.Width, pData.Height];
      double[,] shadowTempPlane = new double[pData.Width, pData.Height];

      double diffy = maxY - minY;


      // Main Iteration:
       double yd = 0;
      double ydv = 0;
      double ydh=0;

      double dShadowNumber = shadowNumber;

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          shadowPlane[i, j] = 0;
        }
      }

      for (int shadowIter = 1; shadowIter < shadowNumber + 1; shadowIter++) {

        yd = diffy / ((double)(pData.Width + pData.Height));
        ydv = diffy / ((double)(pData.Height));
        ydh = diffy / ((double)(pData.Width));

        yd *= 2.0 * shadowIter / dShadowNumber; ydv *= 1.2 * shadowIter / dShadowNumber; ydh *= 1.2 * shadowIter / dShadowNumber;

        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            shadowPlane[i, j] += shadowTempPlane[i, j] / dShadowNumber;
            shadowTempPlane[i, j] = 0;
          }
        }
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            shadowInfo11[i, j] = smoothDeph1[i, j];
            shadowInfo10[i, j] = smoothDeph1[i, j];
            shadowInfo01[i, j] = smoothDeph1[i, j];
            shadowInfo00[i, j] = smoothDeph1[i, j];
            shadowInfo11sharp[i, j] = smoothDeph1[i, j];
            shadowInfo10sharp[i, j] = smoothDeph1[i, j];
            shadowInfo01sharp[i, j] = smoothDeph1[i, j];
            shadowInfo00sharp[i, j] = smoothDeph1[i, j];
          }
        }

        for (int i = pData.Width - 1; i >= 0; i--) {
          for (int j = pData.Height - 1; j >= 0; j--) {
            if (j < pData.Height - 1) {
              double localShadow = shadowInfo11[i, j + 1] - ydh;
              if (localShadow > shadowInfo11[i, j]) {
                shadowInfo11[i, j] = localShadow;
              }
              localShadow = shadowInfo11sharp[i, j + 1] - sharpness * ydh;
              if (localShadow > shadowInfo11sharp[i, j]) {
                shadowInfo11sharp[i, j] = localShadow;
              }
            }
          }
          for (int j = 0; j < pData.Height; j++) {
            // Licht von rechts
            if (i < pData.Width - 1) {
              double localShadow = shadowInfo01[i + 1, j] - ydh;
              if (localShadow > shadowInfo01[i, j]) {
                shadowInfo01[i, j] = localShadow;
              }
              localShadow = shadowInfo01sharp[i + 1, j] - sharpness * ydh;
              if (localShadow > shadowInfo01sharp[i, j]) {
                shadowInfo01sharp[i, j] = localShadow;
              }
            }
          }
        }
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            if (j > 0) {
              double localShadow = shadowInfo10[i, j - 1] - ydv;
              if (localShadow > shadowInfo10[i, j])
                shadowInfo10[i, j] = localShadow;
              localShadow = shadowInfo10sharp[i, j - 1] - sharpness * ydv;
              if (localShadow > shadowInfo10sharp[i, j])
                shadowInfo10sharp[i, j] = localShadow;
            }
            if (i > 0) {
              double localShadow = shadowInfo00[i - 1, j] - ydh;
              if (localShadow > shadowInfo00[i, j])
                shadowInfo00[i, j] = localShadow;
              localShadow = shadowInfo00sharp[i - 1, j] - sharpness * ydh;
              if (localShadow > shadowInfo00sharp[i, j])
                shadowInfo00sharp[i, j] = localShadow;
            }
          }
        }

        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            double shadowMapEntry = 0;
            double currentShadowMapEntry = 0;
            double height = smoothDeph1[i, j];
            double shadowHeight = 0;
            double sharpShadowHeight = 0;

            for (int k = 0; k < 4; k++) {
              switch (k) {
                case 0:
                  shadowHeight = shadowInfo00[i, j];
                  sharpShadowHeight = shadowInfo00sharp[i, j];
                  break;

                case 1:
                  shadowHeight = shadowInfo01[i, j];
                  sharpShadowHeight = shadowInfo01sharp[i, j];
                  break;

                case 2:
                  shadowHeight = shadowInfo10[i, j];
                  sharpShadowHeight = shadowInfo10sharp[i, j];
                  break;

                case 3:
                  shadowHeight = shadowInfo11[i, j];
                  sharpShadowHeight = shadowInfo11sharp[i, j];
                  break;

              }

              double magicNumber = 0.000001;
              if (height != double.MinValue) {
                height += magicNumber; // magic number
                if (height <= sharpShadowHeight) // inside the sharp shadow
                  currentShadowMapEntry = 0.4;
                if (height <= shadowHeight) // inside the sharp shadow
                  currentShadowMapEntry += 0.4;
                shadowMapEntry += currentShadowMapEntry;
              }
            }
            shadowMapEntry /= 4.0;
            if (shadowMapEntry > 1)
              shadowMapEntry = 1;
            shadowTempPlane[i, j] += shadowMapEntry;
          }
        }

        // Stufe2: Schräge Schatten
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            shadowInfo11[i, j] = smoothDeph1[i, j];
            shadowInfo10[i, j] = smoothDeph1[i, j];
            shadowInfo01[i, j] = smoothDeph1[i, j];
            shadowInfo00[i, j] = smoothDeph1[i, j];
            shadowInfo11sharp[i, j] = smoothDeph1[i, j];
            shadowInfo10sharp[i, j] = smoothDeph1[i, j];
            shadowInfo01sharp[i, j] = smoothDeph1[i, j];
            shadowInfo00sharp[i, j] = smoothDeph1[i, j];
          }
        }

        for (int i = pData.Width - 1; i >= 0; i--) {
          for (int j = pData.Height - 1; j >= 0; j--) {
            if (i < pData.Width - 1 && j < pData.Height - 1) {
              double localShadow = shadowInfo11[i + 1, j + 1] - ydh;
              if (localShadow > shadowInfo11[i, j]) {
                shadowInfo11[i, j] = localShadow;
              }
              localShadow = shadowInfo11sharp[i + 1, j + 1] - sharpness * ydh;
              if (localShadow > shadowInfo11sharp[i, j]) {
                shadowInfo11sharp[i, j] = localShadow;
              }
            }
          }
          for (int j = 0; j < pData.Height; j++) {
            // Licht von rechts
            if (i < pData.Width - 1 && j > 0) {
              double localShadow = shadowInfo01[i + 1, j - 1] - ydh;
              if (localShadow > shadowInfo01[i, j]) {
                shadowInfo01[i, j] = localShadow;
              }
              localShadow = shadowInfo01sharp[i + 1, j - 1] - sharpness * ydh;
              if (localShadow > shadowInfo01sharp[i, j]) {
                shadowInfo01sharp[i, j] = localShadow;
              }
            }
          }
        }
        for (int i = 0; i < pData.Width; i++) {
          for (int j = pData.Height - 1; j >= 0; j--) {
            if (i > 0 && j < pData.Height - 1) {
              double localShadow = shadowInfo10[i - 1, j + 1] - ydv;
              if (localShadow > shadowInfo10[i, j])
                shadowInfo10[i, j] = localShadow;
              localShadow = shadowInfo10sharp[i - 1, j + 1] - sharpness * ydv;
              if (localShadow > shadowInfo10sharp[i, j])
                shadowInfo10sharp[i, j] = localShadow;
            }
          }
          for (int j = 0; j < pData.Height; j++) {
            if (i > 0 && j > 0) {
              double localShadow = shadowInfo00[i - 1, j - 1] - ydh;
              if (localShadow > shadowInfo00[i, j])
                shadowInfo00[i, j] = localShadow;
              localShadow = shadowInfo00sharp[i - 1, j - 1] - sharpness * ydh;
              if (localShadow > shadowInfo00sharp[i, j])
                shadowInfo00sharp[i, j] = localShadow;
            }
          }
        }

        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            double shadowMapEntry = 0;
            double currentShadowMapEntry = 0;
            double height = smoothDeph1[i, j];
            double shadowHeight = 0;
            double sharpShadowHeight = 0;
            for (int k = 0; k < 4; k++) {
              switch (k) {

                case 0:
                  shadowHeight = shadowInfo00[i, j];
                  sharpShadowHeight = shadowInfo00sharp[i, j];
                  break;

                case 1:
                  shadowHeight = shadowInfo01[i, j];
                  sharpShadowHeight = shadowInfo01sharp[i, j];
                  break;

                case 2:
                  shadowHeight = shadowInfo10[i, j];
                  sharpShadowHeight = shadowInfo10sharp[i, j];
                  break;

                case 3:
                  shadowHeight = shadowInfo11[i, j];
                  sharpShadowHeight = shadowInfo11sharp[i, j];
                  break;

              }

              double magicNumber = 0.000001;
              if (height != double.MinValue) {
                height += magicNumber; // magic number
                if (height <= sharpShadowHeight) // inside the sharp shadow
                  currentShadowMapEntry = 0.4;
                if (height <= shadowHeight) // inside the sharp shadow
                  currentShadowMapEntry += 0.4;
                shadowMapEntry += currentShadowMapEntry;
              }
            }
            shadowMapEntry /= 4.0;
            //shadowMapEntry /= (2.0 * (double)shadowNumber);
            if (shadowMapEntry > 1)
              shadowMapEntry = 1;
            shadowMapEntry += shadowTempPlane[i, j];
            shadowMapEntry /= 2.0;
            if (shadowMapEntry > 1)
              shadowMapEntry = 1;
            // alt:
            shadowTempPlane[i, j] = shadowMapEntry;
            //shadowPlane[i, j] += shadowMapEntry / shadowNumber;
          }
        }
      }

      // Release Memory:
      shadowInfo11 = null;
      shadowInfo01 = null;
      shadowInfo10 = null;
      shadowInfo00 = null;
      shadowInfo11sharp = null;
      shadowInfo01sharp = null;
      shadowInfo10sharp = null;
      shadowInfo00sharp = null;

      // Smooth shadowPlane
            // 2 times
      for(int m=0;m<4;m++)
        for (int i = 0; i < pData.Width; i++) {
          for (int j = 0; j < pData.Height; j++) {
            double neighborsFound = 0;
            double sumNeighbors=0;
            for (int k = -1; k <= 1; k++) {
              for (int l = -1; l <= 1; l++) {
                int posX = i + k;
                int posY = j + l;
                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                  sumNeighbors+=shadowPlane[posX, posY];
                  neighborsFound++;
                }
              }
            }
            if (neighborsFound > 0)
              shadowPlane[i, j]=0.3*shadowPlane[i, j]+0.7*sumNeighbors/neighborsFound;
          }
        }
    }

    /// <summary>
    /// Die Oberflächennormalen werden abgerundet.
    /// </summary>
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

          for (int k = -2; k <= 2; k++) {
            for (int l = -2; l <= 2; l++) {
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
      // Release Memory
      normalesSmooth1 = null;
    }


    /// <summary>
    /// Lokale Tiefeninformationen werden erzeugt.
    /// </summary>
    protected void CreateSmoothDeph() {
      smoothDeph1 = new double[pData.Width, pData.Height];
      smoothDeph2 = new double[pData.Width, pData.Height];

      // Normieren
      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          PixelInfo pInfo = pData.Points[i, j];
          if (pInfo != null) {
            smoothDeph2[i, j] = pInfo.Coord.Y;
            // if (pInfo.Coord.Y != 0) { // Unterscheidung, ob Schnitt mit Begrenzung vorliegt.
            if (minY > pInfo.Coord.Y)
              minY = pInfo.Coord.Y;
            if (maxY < pInfo.Coord.Y)
              maxY = pInfo.Coord.Y;
            //}
          } else {
            smoothDeph2[i, j] = double.MinValue;
          }
        }
      }

    
      
      SetSmoothDeph(smoothDeph2, smoothDeph1);
    }


    /// <summary>
    /// Tiefeninformationen werden weicher gemacht.
    /// </summary>
    /// <param name="sdeph1"></param>
    /// <param name="sdeph2"></param>
    protected virtual void SetSmoothDeph(double[,] sdeph1, double[,] sdeph2) {

      for (int i = 0; i < pData.Width; i++) {
        for (int j = 0; j < pData.Height; j++) {
          int neighborFound = 0;
          double smoothDeph = 0;
          sdeph2[i, j] = double.MinValue;
          //int k=0;
          for (int k = -1; k <= 1; k++) {
            //int l = -1;
            for (int l = -1; l <= 1; l++) {
              int posX = i + k;
              int posY = j + l;
              if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                double newDeph = sdeph1[posX, posY];

                // neu: es werden nur die Punkte benutzt, die echt größer sind
                if (newDeph != double.MinValue) {
                  double valToAdded =  sdeph1[i, j];
                  if (newDeph > valToAdded)
                    valToAdded = newDeph;
                  smoothDeph += newDeph;
                  neighborFound++;

                }
              }
            }
          }
          if (neighborFound > 0 && sdeph1[i, j]!=double.MinValue) {
            smoothDeph /= (double)neighborFound;
            sdeph2[i, j] =0.5*(sdeph1[i, j]+ smoothDeph);
          } else {
            sdeph2[i, j] = double.MinValue;
          }
        }
      }
    }



    /// <summary>
    /// Unschärfe wird dazugerechnet.
    /// </summary>
    protected void SmoothPlane() {
      double fieldOfViewStart = -0.5;
      double regularSmooth = 0.98; // Bei 1 ist der Vordergrund maximal scharf.
     // double smoothIntensity = 1;
      
        double ydGlobal = (maxY - minY) / ((double)(pData.Width + pData.Height));
        rgbSmoothPlane1 = new Vec3[pData.Width, pData.Height];
        rgbSmoothPlane2 = new Vec3[pData.Width, pData.Height];
        for (int i = 0; i < pData.Width; i++) {
            for (int j = 0; j < pData.Height; j++) {
                rgbSmoothPlane2[i, j] = rgbPlane[i, j];
            }
        }

        double mainDeph = maxY - minY;
        for (int m = 0; m < ambientIntensity+1; m++) {



            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    double neighborsFound = 0;
                    Vec3 nColor = new Vec3();
                    for (int k = -1; k <= 1; k++) {
                        for (int l = -1; l <= 1; l++) {
                            int posX = i + k;
                            int posY = j + l;
                            if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                                // Ein Element im Vordergrund wird nicht mit in die Unschärfe einbezogen.
                                double ylocalDiff = smoothDeph1[i, j] - smoothDeph1[posX, posY];
                                if (ylocalDiff > -3.0 * ydGlobal) {
                                    nColor.Add(rgbSmoothPlane2[posX, posY]);
                                    neighborsFound++;
                                }
                            }
                        }
                    }
                    if (neighborsFound > 0)
                        nColor = nColor.Mult(1 / neighborsFound);

                    double yd = smoothDeph2[i, j];
                    if (yd == double.MinValue)
                        yd = minY;
                    double ydNormalized = (yd - minY) / mainDeph;
                    //ydNormalized = ydNormalized * ydNormalized;
                    ydNormalized -= fieldOfViewStart;
                    //ydNormalized = 1.0 * Math.Abs(ydNormalized);
                    if (ydNormalized > regularSmooth)
                      ydNormalized = regularSmooth;
                    if (ydNormalized < 0)
                        ydNormalized = 0;

                    //ydNormalized = 1 - ydNormalized;
                    Vec3 nCenterColor = rgbSmoothPlane2[i, j];
                    //ydNormalized = ydNormalized * ydNormalized;
                    ydNormalized = 2*Math.Sqrt(ydNormalized)-1;
                    if (ydNormalized < 0)
                      ydNormalized = 0;

                    
                    //ydNormalized = Math.Sqrt(ydNormalized);
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
                                // Ein Element im Vordergrund wird nicht mit in die Unschärfe einbezogen.
                                double ylocalDiff = smoothDeph1[i, j] - smoothDeph1[posX, posY];
                                if (ylocalDiff > -3.0 * ydGlobal) {
                                    nColor.Add(rgbSmoothPlane1[posX, posY]);
                                    neighborsFound++;
                                }
                            }
                        }
                    }
                    if (neighborsFound > 0)
                        nColor = nColor.Mult(1 / neighborsFound);

                    double yd = smoothDeph2[i, j];
                    if (yd == double.MinValue)
                        yd = minY;
                    double ydNormalized = (yd - minY) / mainDeph;
                    //ydNormalized = Math.Sqrt(ydNormalized);
                    //ydNormalized = ydNormalized * ydNormalized;
                    ydNormalized -= fieldOfViewStart;
                    //ydNormalized = 1.0 * Math.Abs(ydNormalized);
                    if (ydNormalized > regularSmooth)
                      ydNormalized = regularSmooth;
                    if (ydNormalized < 0)
                        ydNormalized = 0;
                    //ydNormalized = 1 - ydNormalized;
                    Vec3 nCenterColor = rgbSmoothPlane1[i, j];
                    //ydNormalized = ydNormalized * ydNormalized;
                    ydNormalized = 2 * Math.Sqrt(ydNormalized) - 1;
                    if (ydNormalized < 0)
                      ydNormalized = 0;
                    //ydNormalized = Math.Sqrt(ydNormalized);
                    nCenterColor = nCenterColor.Mult(ydNormalized);
                    nColor = nColor.Mult(1.0 - ydNormalized);
                    nCenterColor.Add(nColor);
                    rgbSmoothPlane2[i, j] = nCenterColor;
                }
            }
        }

        for (int i = 0; i < pData.Width; i++) {
            for (int j = 0; j < pData.Height; j++) {
                rgbPlane[i, j] = rgbSmoothPlane2[i, j];
            }
        }

        rgbSmoothPlane1 = null;
        rgbSmoothPlane2 = null;
    }


    /// <summary>
    /// Die Gestalt wird nach hinten abgedunkelt.
    /// </summary>
    protected void DarkenPlane() {
        double mainDeph = maxY - minY;// borderMaxY - borderMinY;
        for (int i = 0; i < pData.Width; i++) {
            for (int j = 0; j < pData.Height; j++) {
                Vec3 col = rgbPlane[i, j];
                double yd = smoothDeph1[i, j];
                if (yd != double.MinValue) {
                    //if (yd == double.MinValue)
                    //    yd = minY;
                    double ydNormalized = (yd - minY) / mainDeph;
                    ydNormalized = Math.Sqrt(ydNormalized);
                    ydNormalized *= 2 * ydNormalized;
                    if (ydNormalized > 0.95) {
                        ydNormalized = 0.95;
                    }
                    ydNormalized += 0.05;
                    col.X = col.X * ydNormalized;
                    col.Y = col.Y * ydNormalized;
                    ydNormalized += 0.05;
                    col.Z = ydNormalized * col.Z;
                }
            }
        }
    }


  }
}

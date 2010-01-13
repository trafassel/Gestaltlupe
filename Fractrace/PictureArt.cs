using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;

namespace Fractrace {
    public class PictureArt {


        protected PictureData pData = null;


        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public PictureArt(PictureData pData) {
            this.pData = pData;
        }


        int width = 0;
        int height = 0;


        /// <summary>
        /// Erstellt das fertige Bild
        /// </summary>
        /// <param name="grLabel"></param>
        public void Paint(Graphics grLabel) {
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


        protected double minY = 0;
        protected double maxY = 0;
        protected double medianY = 0;
        protected double minFrontLight = 0;
        protected double maxFrontLight = 0;
        protected double minDerivation = double.MaxValue;
        protected double maxDerivation = double.MinValue;


        protected bool useAmbient = false;

        protected bool useDarken = false;

        protected bool useMedian = false;

        protected bool useDerivation = false;

        protected double mFrontLight = 1;

        protected double mAmbientLight = 0;

        protected double mShininess = 1;

        protected bool mUseColor1 = false;

        protected double mColor1Factor = 20;

        protected int mColor1TestArea = 10;

        /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
        protected void PreCalculate() {
            useAmbient = ParameterDict.Exemplar.GetBool("Composite.UseAmbient");
            useDarken = ParameterDict.Exemplar.GetBool("Composite.UseDarken");
            useMedian = ParameterDict.Exemplar.GetBool("Composite.UseMedian");
            useDerivation = ParameterDict.Exemplar.GetBool("Composite.UseDerivation");
            mUseColor1 = ParameterDict.Exemplar.GetBool("Composite.UseColor1");

            mColor1Factor = ParameterDict.Exemplar.GetDouble("Composite.Color1Factor");
            mColor1TestArea= ParameterDict.Exemplar.GetInt("Composite.Color1TestArea");


          mShininess = ParameterDict.Exemplar.GetDouble("Composite.Shininess");
            if (mShininess > 1)
                mShininess = 1;
            if (mShininess < 0)
                mShininess = 0;
            mFrontLight = ParameterDict.Exemplar.GetDouble("Composite.FrontLight"); ;
            mAmbientLight = ParameterDict.Exemplar.GetDouble("Composite.Shininess"); ;

            minY = double.MaxValue;
            maxY = double.MinValue;
            minFrontLight = double.MaxValue;
            maxFrontLight = double.MinValue;
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null) {
                        if (minY > pInfo.Coord.Y)
                            minY = pInfo.Coord.Y;
                        if (maxY < pInfo.Coord.Y)
                            maxY = pInfo.Coord.Y;
                        if (minFrontLight > pInfo.frontLight)
                            minFrontLight = pInfo.frontLight;
                        if (maxFrontLight < pInfo.frontLight)
                            maxFrontLight = pInfo.frontLight;
                        if (minDerivation > pInfo.derivation)
                            minDerivation = pInfo.derivation;
                        if (maxDerivation < pInfo.derivation)
                            maxDerivation = pInfo.derivation;
                    }
                }
            }
            // Median ausrechnen:
            if (useMedian) {
              int dim = 5000; // Je größer dieser Wert, desto genauer die Bestimmung des Medians
              int[] yCount = new int[dim];
              double yd = (maxY - minY) / (double)dim;

              int pixelCount = 0; // Zählt alle dargestellten Pixel
              for (int k = 0; k < dim; k++) {
                yCount[k] = 0;
              }

              for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                  PixelInfo pInfo = pData.Points[i, j];
                  if (pInfo != null) {
                    pixelCount++;
                    // TODO: Median direkt zuordnen (statt den Zähler hochzuzählen)
                    double ytest = pInfo.Coord.Y - minY;
                    int medianIndex = (int)(ytest / yd);
                    if (medianIndex >= dim || medianIndex < 0) {
                      yCount[dim - 1]++;
                    } else
                      yCount[medianIndex]++;
                  }
                }
              }

              medianY = (minY + maxY) / 2.0;

              int medianCount = 0;
              for (int k = 0; k < dim; k++) {
                medianCount += yCount[k];
                if (medianCount >= pixelCount / 2.0) {
                  medianY = minY + (k - 0.5) * yd;
                  break;
                }
              }
            }

        }


        /// <summary>
        /// Liefert Lichtintensität entsprechend des eingestellten Farbmodells
        /// </summary>
        /// <param name="frontLight"></param>
        /// <returns></returns>
        protected double GetLight(double frontLight) {
            if (!(mFrontLight>0.5))
                return 0.7;
            double fDiff = maxFrontLight - minFrontLight;
            double retVal = frontLight;
            retVal *= retVal;

           double retVal2 = Math.PI * retVal / 2.0;
           retVal = mShininess * retVal + (1-mShininess) * Math.Sin(retVal2);

            return retVal;

        }



      /// <summary>
      /// Liefert die Erhebung abhängig von der nächsten Umgebung
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      protected double  GetLocalDeph(int x, int y) {
        double retVal = 0;
        PixelInfo center = pData.Points[x, y];
        if (center == null)
          return 0;
        double centerY = center.Coord.Y;
        // um steile Abhänge auszuschließen
        double localTest= (maxY - minY) / 100.0;
        int[] neigborCount = new int[2*mColor1TestArea+2];
        double[] deph = new double[2*mColor1TestArea+2];
        for (int i = 0; i <= 2 * mColor1TestArea + 1; i++) {
            deph[i] = 0;
          }
          double otherPos = 0;
          double otherPosCount = 0;
          for (int i = -mColor1TestArea; i <= mColor1TestArea; i++) {
            for (int j = -mColor1TestArea; j <= mColor1TestArea; j++) {
              int xx = x + i;
              int yy = y + j;
              if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                int dist = Math.Abs(i) + Math.Abs(j);
            
                PixelInfo pInfo = pData.Points[xx, yy];
                if (pInfo != null) {
                  if (Math.Abs(pInfo.Coord.Y - centerY) < localTest) {

                    otherPos += pInfo.Coord.Y;
                    otherPosCount += 1;
                    neigborCount[dist]++;
                    deph[dist] += pInfo.Coord.Y;
                  }
                }
              }
            }
          }
      //    double col = 0;
        /*
          double otherPos = 0;
          for (int i = 1; i <= 4; i++) {
            if (neigborCount[i] > 0) {
              deph[i] = deph[i] / ((double)neigborCount[i]);
              otherPos += deph[i]; // Jeder Abstand wird mit 1 gewichtet.
              //col += color[i] * weight[i];
            }
          }
         */
        if(otherPosCount>0)
            otherPos = otherPos / otherPosCount-center.Coord.Y;
          return otherPos;
      }


        /// <summary>
        /// Unschärfe, je kleiner Weight, desto unschärfer das Bild.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        protected double GetAmbient(int x, int y, double weight) {
            if (weight > 1)
                weight = 1;
            if (weight < 0)
                weight = 0;

          // debug Unschärfe hart einstellen
          //  weight = 0; 
            double d = 1 - weight;
            return GetAmbient(x, y, new double[] { weight, 0.6*d, 0.2*d, 0.15*d,0.05*d });
        }


        /// <summary>
        /// Wird zur Bestimmung der Unschärfe verwendet.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected double GetAmbient(int x, int y,double[] weight) {
            if (!useAmbient) {
                PixelInfo pInfo = pData.Points[x, y];
                if (pInfo != null) {
                    return GetLight(pInfo.frontLight);
                }
                else return 0;
            }


            int[] neigborCount = new int[5];
            double[] color= new double[5];
            for (int i = 0; i <= 4; i++) {
                color[i] = 0;
            }

            for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {
                    int xx=x+i;
                    int yy=y+j;
                    if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                        int dist = Math.Abs(i) + Math.Abs(j);
                        neigborCount[dist]++;
                        PixelInfo pInfo = pData.Points[xx, yy];
                        if (pInfo != null) {
                            color[dist] +=GetLight( pInfo.frontLight);
                        }
                    }
                }
            }
            double col = 0;
            for (int i = 0; i <= 4; i++) {
              if (neigborCount[i] > 0) {
                color[i] = color[i] / ((double)neigborCount[i]);
                col += color[i] * weight[i];
              }
            }
            return col;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Color GetColor(int x, int y) {
            double red = 0;
            double green = 0;
            double blue = 0;
            PixelInfo pInfo = pData.Points[x, y];
            if (pInfo == null) {
              // return Color.FromArgb((int)255, (int)(0), (int)0);
              double col = GetAmbient(x, y, new double[] { 0, 0.3, 0.35, 0.25, 0.1 });

              if(col<0.01)
                return (new ColorRGB(col)).Color;
              if (col > 1)
                col = 1;

              red = 255.0 * col;
              green = 255.0 * col;
              blue = 255.0 * col;

            //  pInfo = GetNearestPinfo(x, y);
              // TODO: besser Durchschnitt bilden
              for (int i = -2; i <= 2; i++) {
                for (int j = -2; j <= 2; j++) {
                  int xx = x + i;
                  int yy = y + j;
                  if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                    pInfo = pData.Points[xx, yy];
                    if (pInfo != null) {
                      i = 3;
                      break;
                    }
                  }
                }
              }

              if(pInfo==null)
                return (new ColorRGB(col)).Color;
              // debug only
              /*
              if (col > 0.2) {
                return Color.FromArgb((int)0, (int)(255), (int)0);
              }
              // ende debug only
              try {
                return (new ColorRGB(col)).Color;
              } catch (Exception ex) {
                return (new ColorRGB(1)).Color;
              }*/
            } else {
              red = 255.0 * GetLight(pInfo.frontLight);
              green = 255.0 * GetLight(pInfo.frontLight);
              blue = 255.0 * GetLight(pInfo.frontLight);
            }
            if (pInfo.frontLight < 0) {
              //return Color.FromArgb((int)255, (int)(0), (int)0);
              try {
                return Color.FromArgb((int)0, (int)(-pInfo.frontLight), (int)0);
              } catch (Exception ex) {
                return (new ColorRGB(1)).Color;
              }
            }
          

            double dephPoint = medianY;
            double ydiff=maxY-minY;
            double sharpness = 1;
            double maxYpara = maxY;
            double minYpara = minY;
            if (!useMedian) {
               maxYpara = ParameterDict.Exemplar.GetDouble("Border.Max.y");
               minYpara = ParameterDict.Exemplar.GetDouble("Border.Min.y");
              ydiff = maxYpara - minYpara;
              dephPoint = 0.7 * maxYpara + 0.3 * minYpara;
              sharpness = Math.Abs(pInfo.Coord.Y - dephPoint) / ydiff;


              //sharpness *= sharpness;
              sharpness =0.5*(sharpness+ Math.Sqrt(sharpness));
              sharpness = 1.0 - sharpness;
            } else {
              // Alles unterhalb dieser Grenze wird unscharf dargestellt
              double minAmbig = (minY + dephPoint) / 2.0;
              // Alles oberhalb dieser Grenze wird scharf dargestellt
              double maxAmbig = (maxY + dephPoint) / 2.0;

              if (pInfo.Coord.Y < minAmbig)
                sharpness = 0;
              else if (pInfo.Coord.Y >= maxAmbig) {
                // Ganz vorne wird auch unscharf dargestellt
                double dist = maxY - dephPoint;

                //sharpness = 1;
                sharpness = 1 - (pInfo.Coord.Y - maxAmbig) / dist;
              } else {
                double d = maxAmbig - minAmbig;
                sharpness = (pInfo.Coord.Y - minAmbig) / d;
              }
            }
          
            // Unschärfe hart einstellen
                double col1 = GetAmbient(x,y,sharpness);

          // Test auf lokale Erhebung
              

                red = 245.0 * col1;
                green = 245.0 * col1;
                blue = 245.0 * col1;


                if (mUseColor1) {

                  double localdeph = GetLocalDeph(x, y) / (maxY - minY);
                  localdeph = mColor1Factor * localdeph;
                  if (localdeph > 1)
                    localdeph = 1;
                  if (localdeph < -1)
                    localdeph = -1;

                  red = 0.5 * red - 0.5 * localdeph * red;
                  blue = 0.5 * blue + 0.5 * localdeph * blue;

                  red = red / 255.0;
                  blue = blue / 255.0;
                  red = Math.Sqrt(red);
                  blue = Math.Sqrt(blue);
                  red = 255.0 * red;
                  blue = 255.0 * blue;
                }


                // Krümmung hinzufügen
                double dVal = 0;
                double otherDerivation = pInfo.derivation;
        
                if (otherDerivation < -0.7)
                    otherDerivation = -0.7;
                if (otherDerivation > 0.7)
                    otherDerivation = 0.7;
                if (double.IsNaN(otherDerivation))
                    otherDerivation = 0;
                if (otherDerivation <= 0) {
                    dVal = -otherDerivation / 4.0;
                    red += 0.1 * dVal*red;
                    blue += 0.1 * dVal*blue;
                    green -= 0.1 * dVal*green;
                }
                else {
                    //dVal=pInfo.derivation/maxDerivation;
                    dVal = otherDerivation / 4.0;
                    green += 0.2 *green* dVal;
                    red -= 0.1 * dVal*red;
                    blue -= 0.1 * dVal*blue;
                }

                // Hintere Elemente werden dunkler dargestellt
                double deph = (pInfo.Coord.Y - minYpara) / (maxYpara - minYpara);
                deph *= 1.3; // Nahe Elemente sollen stets hell dargestellt werden
                if (deph > 1)
                  deph = 1;
                deph *= deph;
                //deph = Math.Sqrt(deph);
                red = 0.05 * red + 0.95 * deph * red;
                green = 0.05 * green + 0.95 * deph * green;
                blue = 0.15 * blue + 0.85 * deph * blue;


            if (red < 0)
                red = 0;
            if (red > 255)
                red = 255;
            if (green < 0)
                green = 0;
            if (green > 255)
                green = 255;
            if (blue < 0)
                blue = 0;
            if (blue > 255)
                blue = 255;

            try {
                return Color.FromArgb((int)red, (int)green, (int)blue);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            return Color.Black;
        }
    }
}

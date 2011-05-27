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


        /// <summary>
        /// Coordninate of the bottom, left, front point of the Boundingbox (in original Coordinates).  
        /// </summary>
        private Vec3 minPoint = new Vec3(0, 0, 0);


        /// <summary>
        /// Coordninate of the top, right, backside point of the Boundingbox (in original Coordinates).  
        /// </summary>
        private Vec3 maxPoint = new Vec3(0, 0, 0);



        private Vec3[,] normalesSmooth1 = null;


        /// <summary>
        /// Zusatzinformationen zum Bild.
        /// 0 keine info
        /// 1 Element der Schnittmenge mit dem Bildschirm
        /// </summary>
        private int[,] picInfo = null;


        private Vec3[,] normalesSmooth2 = null;
        private double[,] sharpShadow = null;

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
        private int shadowNumber = 1;


        // Intensity of the FieldOfView
        private int ambientIntensity = 4;


        // Intensity of the Surface Color
        private double colorIntensity = 0.5;


        // if useLight==false, only the shades are computed. 
        private bool useLight = true;


        // Shadow height factor
        private double shadowJustify = 1;


        // Influence of the shininess (0 <= shininessFactor <=1)
        private double shininessFactor = 0.7;

        // Shininess ( 0... 1000)
        private double shininess = 8;

        // Normal of the light source     
        private Vec3 lightRay = new Vec3();

        // If thrue, sharp shadow rendering is activated (warning: time consuming) 
        private bool useSharpShadow = false;


        private double colorFactorRed = 1;
        private double colorFactorGreen = 1;
        private double colorFactorBlue = 1;

        private double lightIntensity = 0.5;

        // If ColorGreyness=1, no color is rendered
        private double colorGreyness = 0;


        /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
        protected override void PreCalculate() {
            shadowNumber = ParameterDict.Exemplar.GetInt("Composite.Renderer.Plasic.ShadowNumber");
            ambientIntensity = ParameterDict.Exemplar.GetInt("Composite.Renderer.Plasic.AmbientIntensity");
            colorIntensity = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorIntensity");
            useLight = ParameterDict.Exemplar.GetBool("Composite.Renderer.Plasic.UseLight");
            shadowJustify = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ShadowJustify");

            shininessFactor = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ShininessFactor");
            shininess = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.Shininess");
            lightRay.X = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.Light.X");
            lightRay.Y = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.Light.Y");
            lightRay.Z = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.Light.Z");
            useSharpShadow = ParameterDict.Exemplar.GetBool("Composite.Renderer.Plasic.UseSharpShadow");

            colorFactorRed = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorFactor.Red");
            colorFactorGreen = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorFactor.Green");
            colorFactorBlue = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorFactor.Blue");

            lightIntensity = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.LightIntensity");

            colorGreyness = ParameterDict.Exemplar.GetDouble("Composite.Renderer.Plasic.ColorGreyness");

            if (lightIntensity > 1)
                lightIntensity = 1;
            if (lightIntensity < 0)
                lightIntensity = 0;

            picInfo = new int[pData.Width, pData.Height];

            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    picInfo[i, j] = 0;
                }
            }
            CreateStatisticInfo();
            if (useSharpShadow)
                CreateSharpShadow();
            // Testweise auskommentiert (braucht man eventuell auch nicht):
            // CreateSmoothNormales();
            CreateSmoothDeph();
            CreateShadowInfo();
            DrawPlane();
            return;
            if (ParameterDict.Exemplar.GetBool("Composite.Normalize"))
                NormalizePlane();
            if (ParameterDict.Exemplar.GetBool("Composite.Renderer.Plasic.UseDarken"))
                DarkenPlane();
            SmoothEmptyPixel();
            SmoothPlane();
        }


        /// <summary>
        /// Creates boundingbox infos.
        /// </summary>
        protected void CreateStatisticInfo() {
            minPoint.X = Double.MaxValue;
            minPoint.Y = Double.MaxValue;
            minPoint.Z = Double.MaxValue;
            maxPoint.X = Double.MinValue;
            maxPoint.Y = Double.MinValue;
            maxPoint.Z = Double.MinValue;
            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null) {
                        Vec3 coord = formula.GetTransform(pInfo.Coord.X, pInfo.Coord.Y, pInfo.Coord.Z);
                        if (coord.X < minPoint.X)
                            minPoint.X = coord.X;
                        if (coord.Y < minPoint.Y)
                            minPoint.Y = coord.Y;
                        if (coord.Z < minPoint.Z)
                            minPoint.Z = coord.Z;
                        if (coord.X > maxPoint.X)
                            maxPoint.X = coord.X;
                        if (coord.Y > maxPoint.Y)
                            maxPoint.Y = coord.Y;
                        if (coord.Z > maxPoint.Z)
                            maxPoint.Z = coord.Z;


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
                        //pixelCount++; // Etwas dunkler sollte es schon werden
                        if (pixelCount > 0) {
                            col.X /= pixelCount;
                            col.Y /= pixelCount;
                            col.Z /= pixelCount;
                        }
                        //col.X = 0; col.Y = 0; col.Z = 1;
                    }
                }
            }
        }




        /// <summary>
        /// Der Schlagschatten wird erzeugt.
        /// </summary>
        protected void CreateSharpShadow() {
            // Erst ab 4 Pixel handelt es sich um einen Schlagschatten
            int shadowIntensityCount = 4;
            // Je höher, desto größer die Rechenzeit und desto genau die Schlagschattenberechnung.
            int shadowCorrectness = 100;
            double rayDist = minPoint.Dist(maxPoint);
            sharpShadow = new double[pData.Width, pData.Height];
            double[,] sharpTempShadow = new double[pData.Width, pData.Height];

            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    picInfo[i, j] = 0;
                    sharpShadow[i, j] = 0;
                }
            }

            Vec3 normal = null;
            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null) {
                        normal = pInfo.Normal;
                        sharpShadow[i, j] = 0;
                        Vec3 coord = formula.GetTransform(pInfo.Coord.X, pInfo.Coord.Y, pInfo.Coord.Z);
                        int sharpShadowIntensity = IsInSharpShadow(coord, lightRay, rayDist, pInfo.IsInside, shadowIntensityCount, shadowCorrectness);
                        sharpShadow[i, j] = sharpShadowIntensity / ((double)shadowIntensityCount);
                    }
                }
            }

            double[,] sShad1 = sharpShadow;
            double[,] sShad2 = sharpTempShadow;

            for (int m = 0; m < 2; m++) {

                for (int i = 0; i < pData.Width; i++) {
                    for (int j = 0; j < pData.Height; j++) {
                        double neighborsFound = 0;
                        double sumNeighbors = 0;
                        for (int k = -1; k <= 1; k++) {
                            for (int l = -1; l <= 1; l++) {
                                int posX = i + k;
                                int posY = j + l;
                                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height) {
                                    sumNeighbors += sShad1[posX, posY];
                                    neighborsFound++;
                                }
                            }
                        }
                        if (neighborsFound > 0)
                            sShad2[i, j] = 0.4 * sShad1[i, j] + 0.6 * sumNeighbors / neighborsFound;
                    }
                }

                sShad1 = sharpTempShadow;
                sShad2 = sharpShadow;
            }
            //sharpShadow

        }


        /// <summary>
        /// Get the color information of the bitmap at (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Vec3 GetRgb(int x, int y) {
            Vec3 retVal = new Vec3(0, 0, 1); // blau
            PixelInfo pInfo = pData.Points[x, y];
            if (pInfo == null) {
                return new Vec3(0, 0, 0);
            }

            Vec3 light = new Vec3(0, 0, 0);

            if (pInfo.frontLight < 0) {
                //return Color.FromArgb((int)255, (int)(0), (int)0);
                try {
                    retVal.X = 0.3;
                    retVal.Y = -pInfo.frontLight / 255.0;
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

            Vec3 normal = null;
                
//               normal= normalesSmooth2[x, y];
            if (normal == null) { normal = pInfo.Normal; }
            // Testweise original Normale verwenden
            normal = pInfo.Normal;
            // TODO: Obiges auskommentieren
            if (normal == null)
                return new Vec3(0, 0, 0);

            // tempcoord2 enthält die umgerechnete Oberflächennormale. 
            double tfac = 1;
            
            Vec3 coord = formula.GetTransformWithoutProjection(pInfo.Coord.X, pInfo.Coord.Y, pInfo.Coord.Z);
            Vec3 tempcoord2 = formula.GetTransformWithoutProjection(pInfo.Coord.X + tfac * normal.X, pInfo.Coord.Y + tfac * normal.Y, pInfo.Coord.Z + tfac * normal.Z);
            
            tempcoord2.X -= coord.X;
            tempcoord2.Y -= coord.Y;
            tempcoord2.Z -= coord.Z;

            // Normalize:
            tempcoord2.Normalize();

            if (pInfo.Normal != null) {
                light = GetLight(tempcoord2);
                if (sharpShadow != null) { light = light.Mult(1 - sharpShadow[x, y]); }
            }

            retVal.X = light.X;
            retVal.Y = light.Y;
            retVal.Z = light.Z;

            double d1 = maxY - minY;
            double d2 = pData.Width + pData.Height;
            double d3 = d1 / d2;


            // lightIntensity
            retVal.X = lightIntensity * retVal.X + (1 - lightIntensity) * (1 - shadowPlane[x, y]);
            retVal.Y = lightIntensity * retVal.Y + (1 - lightIntensity) * (1 - shadowPlane[x, y]);
            retVal.Z = lightIntensity * retVal.Z + (1 - lightIntensity) * (1 - shadowPlane[x, y]);

            /*
        double shadowlight = 0.34 * shadowPlane[x, y];
        retVal.Z = 0.2 * retVal.Z + 0.8 * Math.Max(0, retVal.Z - shadowlight);
        retVal.X = 0.2 * retVal.X + 0.8 * Math.Max(0, retVal.X - shadowlight);
        retVal.Y = 0.2 * retVal.Y + 0.8 * Math.Max(0, retVal.Y - shadowlight);
             */

            if (retVal.Y < 0)
                retVal.Y = 0;
            if (retVal.Z < 0)
                retVal.Z = 0;
            if (retVal.Y > 1)
                retVal.Y = 1;
            if (retVal.Z > 1)
                retVal.Z = 1;


            // Add surface color
            bool useAdditionalColorinfo = true;
            if (colorIntensity <= 0)
                useAdditionalColorinfo = false;
            if (useAdditionalColorinfo) {
                if (pInfo != null && pInfo.AdditionalInfo != null) {
                    // Normalisieren;
                    double r1 = colorFactorRed * Math.Pow(pInfo.AdditionalInfo.red, colorIntensity);
                    double g1 = colorFactorGreen * Math.Pow(pInfo.AdditionalInfo.green, colorIntensity);
                    double b1 = colorFactorBlue * Math.Pow(pInfo.AdditionalInfo.blue, colorIntensity);
                    if (r1 < 0)
                        r1 = -r1;
                    if (g1 < 0)
                        g1 = -g1;
                    if (b1 < 0)
                        b1 = -b1;

                    double norm = Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1);
                    norm = norm / 3.0;
                    r1 = r1 / norm;
                    g1 = g1 / norm;
                    b1 = b1 / norm;

                    if (colorGreyness > 0) {
                        r1 = 0.5 * colorGreyness + (1 - colorGreyness) * r1;
                        g1 = 0.5 * colorGreyness + (1 - colorGreyness) * g1;
                        b1 = 0.5 * colorGreyness + (1 - colorGreyness) * b1;
                    }

                    if (r1 > 1)
                        r1 = 1;
                    if (b1 > 1)
                        b1 = 1;
                    if (g1 > 1)
                        g1 = 1;

                    if (norm != 0) {
                        retVal.X *= r1;
                        retVal.Y *= g1;
                        retVal.Z *= b1;
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
            if (!useLight) {
                return new Vec3(0.5, 0.5, 0.5);
            }
            if (normal == null)
                return retVal;

            double weight_shini = shininessFactor;
            double weight_diffuse = 1 - weight_shini;

            double norm = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            // Der Winkel ist nun das Skalarprodukt mit (0,-1,0)= Lichtstrahl
            // mit Vergleichsvektor (Beide nachträglich normiert )            
            double angle = 0;
            if (norm == 0)
                return retVal;

            Vec3 lightVec = new Vec3(lightRay.X, lightRay.Y, lightRay.Z);
            lightVec.Normalize();
            double norm2 = lightVec.Norm;
            angle = Math.Acos((normal.X * lightVec.X + normal.Y * lightVec.Y + normal.Z * lightVec.Z) / (norm * norm2)) / (Math.PI / 2.0);

            angle = 1 - angle;
            if (angle < 0)
                angle = 0;
            if (angle > 1)
                angle = 1;
            double light = weight_diffuse * angle + weight_shini * Math.Pow(angle, shininess);
            //   double light =(Math.Pow(angle, 128));
            //   double light = angle;
            if (light < 0)
                light = 0;
            if (light > 1)
                light = 1;

            retVal.X = light;
            retVal.Y = light;
            retVal.Z = light;

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

            double diffy = shadowJustify * (maxY - minY);


            // Main Iteration:
            double yd = 0;
            double ydv = 0;
            double ydh = 0;

            double dShadowNumber = shadowNumber;

            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    shadowPlane[i, j] = 0;
                }
            }

            double shadowVal = 0.3;


         //   for (int shadowMode = 0; shadowMode < 3; shadowMode++) {
                for (int shadowMode = 1; shadowMode <=1; shadowMode++) {
                switch (shadowMode) {
                    case 0:
                        diffy = 0.3 * shadowJustify * (maxY - minY);
                        shadowVal = 0.19;
                        break;
                    case 1:
                        diffy = shadowJustify * (maxY - minY);
                        shadowVal = 0.3;
                        break;
                    case 2:
                        diffy = 3.0 * shadowJustify * (maxY - minY);
                        shadowVal = 0.19;
                        break;

                }

                for (int shadowIter = 1; shadowIter < shadowNumber + 1; shadowIter++) {

                    yd = diffy / ((double)(pData.Width + pData.Height));
                    ydv = diffy / ((double)(pData.Height));
                    ydh = diffy / ((double)(pData.Width));


                    yd *= 2.0 * (double)shadowIter / (double)dShadowNumber; ydv *= 1.2 * (double)shadowIter / (double)dShadowNumber; ydh *= 1.2 * (double)shadowIter / (double)dShadowNumber;


                    // Clean Plane
                    for (int i = 0; i < pData.Width; i++) {
                        for (int j = 0; j < pData.Height; j++) {
                            shadowTempPlane[i, j] = 0;
                        }
                    }

                    /*

for (int i = 0; i < pData.Width; i++) {
    for (int j = 0; j < pData.Height; j++) {
        PixelInfo pInfo = pData.Points[i, j];
        if (pInfo != null) {
            shadowInfo11[i, j] = smoothDeph1[i, j];
            shadowInfo10[i, j] = smoothDeph1[i, j];
            shadowInfo01[i, j] = smoothDeph1[i, j];
            shadowInfo00[i, j] = smoothDeph1[i, j];
            shadowInfo11sharp[i, j] = smoothDeph1[i, j];
            shadowInfo10sharp[i, j] = smoothDeph1[i, j];
            shadowInfo01sharp[i, j] = smoothDeph1[i, j];
            shadowInfo00sharp[i, j] = smoothDeph1[i, j];
        } else {

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
}

for (int i = pData.Width - 1; i >= 0; i--) {
                        
    // *********  Fill shadowInfo11  ***********
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

    // *********  Fill shadowInfo01  ***********
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
        // *********  Fill shadowInfo10  ***********
        if (j > 0) {
            double localShadow = shadowInfo10[i, j - 1] - ydv;
            if (localShadow > shadowInfo10[i, j])
                shadowInfo10[i, j] = localShadow;
            localShadow = shadowInfo10sharp[i, j - 1] - sharpness * ydv;
            if (localShadow > shadowInfo10sharp[i, j])
                shadowInfo10sharp[i, j] = localShadow;
        }
        // *********  Fill shadowInfo00  ***********
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


// *********  Combine shadowInfo11  ***********
// *********  create shadowTempPlane **********

for (int i = 0; i < pData.Width; i++) {
    for (int j = 0; j < pData.Height; j++) {
        double shadowMapEntry = 0;
        double currentShadowMapEntry = 0;
        double height = smoothDeph1[i, j];
        double shadowHeight = 0;
        double sharpShadowHeight = 0;

       // for (int k = 0; k < 4; k++) {
         for (int k = 1; k < 0; k++) {  
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

            double magicNumber = 0.000001 * diffy;
            if (height != double.MinValue) {
                height += magicNumber; // magic number
                if (height <= sharpShadowHeight) // inside the sharp shadow
                    currentShadowMapEntry = shadowVal;
                if (height <= shadowHeight) // inside the sharp shadow
                    currentShadowMapEntry += shadowVal;
                shadowMapEntry += currentShadowMapEntry;
            }
        }
        shadowMapEntry /= 4.0;
        if (shadowMapEntry > 1)
            shadowMapEntry = 1;
        shadowTempPlane[i, j] += shadowMapEntry;
    }
}

*/

                    // *************************************
                    // Same again, but with diagonal shadows

                    // initialize shadowInfo00, ... shadowInfo11, shadowInfo00sharp, ... , shadowInfo11sharp
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

                    int currentIntXval = 1;
                    int currentIntYval = 1;

                   System.Random rand=new Random();
                    double rt = 6.0 * rand.NextDouble() * rand.NextDouble() + rand.NextDouble();
                        currentIntXval += (int) rt;
                        rt = 6.0 * rand.NextDouble() * rand.NextDouble() + rand.NextDouble();
                        currentIntYval += (int)rt;
                        double r1 = rand.NextDouble();
                        if (r1 < 0.15) {
                            currentIntXval = 1;
                            currentIntYval = 0;
                        } else if (r1 < 0.2) {
                            currentIntXval = 0;
                            currentIntYval = 1;
                        }

                    for (int i = pData.Width - currentIntXval; i >= 0; i--) {
                        for (int j = pData.Height - currentIntYval; j >= 0; j--) {

                            // *********  Fill shadowInfo11  ***********
                            if (i < pData.Width - currentIntXval && j < pData.Height - currentIntYval) {
                                double localShadow = shadowInfo11[i + currentIntXval, j + currentIntYval] - ydh;
                                if (localShadow > shadowInfo11[i, j]) {
                                    shadowInfo11[i, j] = localShadow;
                                }
                                localShadow = shadowInfo11sharp[i + currentIntXval, j + currentIntYval] - sharpness * ydh;
                                if (localShadow > shadowInfo11sharp[i, j]) {
                                    shadowInfo11sharp[i, j] = localShadow;
                                }
                            }
                        }
                        for (int j = 0; j < pData.Height; j++) {

                            // *********  Fill shadowInfo01  ***********
                            if (i < pData.Width - currentIntXval && j >= currentIntYval) {
                                double localShadow = shadowInfo01[i + currentIntXval, j - currentIntYval] - ydh;
                                if (localShadow > shadowInfo01[i, j]) {
                                    shadowInfo01[i, j] = localShadow;
                                }
                                localShadow = shadowInfo01sharp[i + currentIntXval, j - currentIntYval] - sharpness * ydh;
                                if (localShadow > shadowInfo01sharp[i, j]) {
                                    shadowInfo01sharp[i, j] = localShadow;
                                }
                            }
                        }
                    }
                

                    for (int i = 0; i < pData.Width; i++) {

                        // *********  Fill shadowInfo10  ***********
                        for (int j = pData.Height - currentIntXval; j >= 0; j--) {
                            if (i >= currentIntXval && j < pData.Height - currentIntYval) {
                                double localShadow = shadowInfo10[i - currentIntXval, j + currentIntYval] - ydv;
                                if (localShadow > shadowInfo10[i, j])
                                    shadowInfo10[i, j] = localShadow;
                                localShadow = shadowInfo10sharp[i - currentIntXval, j + currentIntYval] - sharpness * ydv;
                                if (localShadow > shadowInfo10sharp[i, j])
                                    shadowInfo10sharp[i, j] = localShadow;
                            }
                        }
                        // *********  Fill shadowInfo00  ***********
                        for (int j = 0; j < pData.Height; j++) {

                            if (i >= currentIntXval && j >= currentIntYval) {
                            //if (i > 1 && j > 1 && i < pData.Width-1 && j<pData.Height-1) {
                                //double localShadow = shadowInfo00[i - 1, j - 1] - ydh;
                                //double localShadow = shadowInfo00[i - 1, j - 2] - ydh;
                                double localShadow = shadowInfo00[i - currentIntXval, j - currentIntYval] - ydh;
                                if (localShadow > shadowInfo00[i, j])
                                    shadowInfo00[i, j] = localShadow;
                                //localShadow = shadowInfo00sharp[i - 1, j - 1] - sharpness * ydh;

                                localShadow = shadowInfo00sharp[i - currentIntXval, j - currentIntYval] - sharpness * ydh;
                                //double localShadow = shadowInfo00[i - currentIntXval, j - currentIntYval] - ydh;
                                if (localShadow > shadowInfo00sharp[i, j])
                                    shadowInfo00sharp[i, j] = localShadow;
                            }
                        }
                    }


                    // *********  Combine shadowInfo00, ..., shadowInfo11sharp  ***********
                    // *********  create shadowTempPlane **********

                    for (int i = 0; i < pData.Width; i++) {
                        for (int j = 0; j < pData.Height; j++) {
                            double shadowMapEntry = 0;
                            double currentShadowMapEntry = 0;
                            double height = smoothDeph1[i, j];
                            double shadowHeight = 0;
                            double sharpShadowHeight = 0;
                            for (int k = 0; k < 4; k++) {
     //                       for (int k = 0; k <= 0; k++) {
  //                          for (int k = 0; k < 0; k++) {
                                //   for (int k = 1; k <= 1; k++) {

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
                                        currentShadowMapEntry = shadowVal;
                                    if (height <= shadowHeight) // inside the sharp shadow
                                        currentShadowMapEntry += shadowVal;
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

                    for (int i = 0; i < pData.Width; i++) {
                        for (int j = 0; j < pData.Height; j++) {
                            shadowPlane[i, j] += shadowTempPlane[i, j] / dShadowNumber;
                        }
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

            // Normalise:
            double sMin = Double.MaxValue;
            double sMax = Double.MinValue;
            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    if (sMin > shadowPlane[i, j])
                        sMin = shadowPlane[i, j];
                    if (sMax < shadowPlane[i, j])
                        sMax = shadowPlane[i, j];

                }
            }
            double sDiff = sMax - sMin;
            for (int i = 0; i < pData.Width; i++) {
                for (int j = 0; j < pData.Height; j++) {
                    shadowPlane[i, j] = (shadowPlane[i, j] - sMin) / sDiff;
                    if (double.IsNaN(shadowPlane[i, j]) || double.IsInfinity(shadowPlane[i, j])) {
                        shadowPlane[i, j] = 0;
                    }
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
                        if (minY > pInfo.Coord.Y && pInfo.Coord.Y != 0)
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
                                    double valToAdded = sdeph1[i, j];
                                    if (newDeph > valToAdded)
                                        valToAdded = newDeph;
                                    smoothDeph += newDeph;
                                    neighborFound++;

                                }
                            }
                        }
                    }
                    if (neighborFound > 0 && sdeph1[i, j] != double.MinValue) {
                        smoothDeph /= (double)neighborFound;
                        sdeph2[i, j] = 1.0 * (0.8 * sdeph1[i, j] + 0.2 * smoothDeph);
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


            // rgbSmoothPlane1 is set
            double mainDeph = maxY - minY;
            for (int m = 0; m < ambientIntensity; m++) {
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
                                    // neu: doch:
                                    double ylocalDiff = smoothDeph1[i, j] - smoothDeph1[posX, posY];
                                    //if (ylocalDiff > -3.0 * ydGlobal) {
                                        nColor.Add(rgbSmoothPlane2[posX, posY]);
                                        neighborsFound++;
                                    //}
                                }
                            }
                        }
                        if (neighborsFound > 0)
                            nColor = nColor.Mult(1 / neighborsFound);

                        double yd = smoothDeph2[i, j];
                        if (yd == double.MinValue)
                            yd = minY;
                        double ydNormalized = (yd - minY) / mainDeph;
                        ydNormalized -= fieldOfViewStart;
                        if (ydNormalized > regularSmooth)
                            ydNormalized = regularSmooth;
                        if (ydNormalized < 0)
                            ydNormalized = 0;

                        Vec3 nCenterColor = rgbSmoothPlane2[i, j];
                        ydNormalized = 2 * Math.Sqrt(ydNormalized) - 1;
                        if (ydNormalized < 0)
                            ydNormalized = 0;


                        nCenterColor = nCenterColor.Mult(ydNormalized);
                        nColor = nColor.Mult(1.0 - ydNormalized);
                        nCenterColor.Add(nColor);

                        rgbSmoothPlane1[i, j] = nCenterColor;
                    }
                }


                // rgbSmoothPlane2 is set (from rgbSmoothPlane1)
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
                                    // Neu: doch
                                    double ylocalDiff = smoothDeph1[i, j] - smoothDeph1[posX, posY];
                                    //if (ylocalDiff > -3.0 * ydGlobal) {
                                        nColor.Add(rgbSmoothPlane1[posX, posY]);
                                        neighborsFound++;
                                    //}
                                }
                            }
                        }
                        if (neighborsFound > 0)
                            nColor = nColor.Mult(1 / neighborsFound);

                        double yd = smoothDeph2[i, j];
                        if (yd == double.MinValue)
                            yd = minY;
                        double ydNormalized = (yd - minY) / mainDeph;
                        ydNormalized -= fieldOfViewStart;
                        if (ydNormalized > regularSmooth)
                            ydNormalized = regularSmooth;
                        if (ydNormalized < 0)
                            ydNormalized = 0;
                        Vec3 nCenterColor = rgbSmoothPlane1[i, j];
                        ydNormalized = 2 * Math.Sqrt(ydNormalized) - 1;
                        if (ydNormalized < 0)
                            ydNormalized = 0;
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


        /// <summary>
        /// Test, if the given point is inside the sharp shadow. 
        /// Returns the number of intersection with the ray and the fractal, but not more than maxIntersections.
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="ray"></param>
        /// <param name="rayLenght"></param>
        /// <returns></returns>
        protected int IsInSharpShadow(Vec3 point, Vec3 ray, double rayLenght, bool inverse, int maxIntersections, int steps) {
            //int steps = 100;
            inverse = false;
            double dSteps = steps;
            double dist = 0;
            int shadowCount = 0;
            for (int gSteps = 0; gSteps < 6; gSteps++) {
                dist = rayLenght / dSteps;
                Vec3 currentPoint = new Vec3(point);
                currentPoint.Add(ray.Mult(dist));
                for (int i = 0; i < steps; i++) {
                    currentPoint.Add(ray.Mult(dist));
                    if (formula.TestPoint(currentPoint.X, currentPoint.Y, currentPoint.Z, inverse)) {
                        shadowCount++;
                        if (shadowCount >= maxIntersections)
                            return maxIntersections;
                    } else {
                        //  return false;
                    }
                }
                rayLenght /= 1.4;
            }
            return shadowCount;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

namespace Fractrace.PictureArt
{


    /// <summary>
    /// Gestaltlupe default Renderer.
    /// </summary>
    public class PlasicRenderer : ScienceRendererBase
    {


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="pData"></param>
        public PlasicRenderer(PictureData pData)
            : base(pData)
        {
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
        /// Additional informationen for the picture.
        /// 0 no info
        /// 1 elemtent of the cut with the screen
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
        /// RGB-Componente type, i.e.:rgbType==4 Change Red and Blue component.
        /// </summary>
        private int rgbType = 1;

        // Minimal value of FieldOfView
        private double minFieldOfView = 0;

        // Maximal value of FieldOfView
        private double maxFieldOfView = 1;

        // Red component of background color 
        private double backColorRed = 0.4;

        // Blue component of background color 
        private double backColorBlue = 0.6;

        // Green component of background color 
        private double backColorGreen = 0.4;

        /// <summary>
        /// Difference between maximal and minimal y value in computing area
        /// </summary>
        private double areaDeph = 0;


        private double brightness = 1;

        private double contrast = 1;


        /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
        protected override void PreCalculate()
        {
            string parameterNode = "Renderer.";
            shadowNumber = ParameterDict.Current.GetInt(parameterNode + "ShadowNumber");
            ambientIntensity = ParameterDict.Current.GetInt(parameterNode + "AmbientIntensity");
            minFieldOfView = ParameterDict.Current.GetDouble(parameterNode + "MinFieldOfView");
            maxFieldOfView = ParameterDict.Current.GetDouble(parameterNode + "MaxFieldOfView");

            brightness = ParameterDict.Current.GetDouble(parameterNode + "Brightness");
            contrast = ParameterDict.Current.GetDouble(parameterNode + "Contrast");

            colorIntensity = ParameterDict.Current.GetDouble(parameterNode + "ColorIntensity");
            useLight = ParameterDict.Current.GetBool(parameterNode + "UseLight");
            shadowJustify = ParameterDict.Current.GetDouble(parameterNode + "ShadowJustify");

            shininessFactor = ParameterDict.Current.GetDouble(parameterNode + "ShininessFactor");
            shininess = ParameterDict.Current.GetDouble(parameterNode + "Shininess");
            lightRay.X = ParameterDict.Current.GetDouble(parameterNode + "Light.X");
            lightRay.Y = ParameterDict.Current.GetDouble(parameterNode + "Light.Y");
            lightRay.Z = ParameterDict.Current.GetDouble(parameterNode + "Light.Z");

            areaDeph = ParameterDict.Current.GetDouble("Border.Max.y") - ParameterDict.Current.GetDouble("Border.Min.y");
            // Rotate lightvec:
            Vec3 coord = formula.GetTransformWithoutProjection(0, 0, 0);
            Vec3 tempcoord2 = formula.GetTransformWithoutProjection(lightRay.X, lightRay.Y, lightRay.Z);
            tempcoord2.X -= coord.X;
            tempcoord2.Y -= coord.Y;
            tempcoord2.Z -= coord.Z;
            tempcoord2.Normalize();
            lightRay.X = tempcoord2.X;
            lightRay.Y = tempcoord2.Y;
            lightRay.Z = tempcoord2.Z;

            useSharpShadow = ParameterDict.Current.GetBool(parameterNode + "UseSharpShadow");

            colorFactorRed = ParameterDict.Current.GetDouble(parameterNode + "ColorFactor.Red");
            colorFactorGreen = ParameterDict.Current.GetDouble(parameterNode + "ColorFactor.Green");
            colorFactorBlue = ParameterDict.Current.GetDouble(parameterNode + "ColorFactor.Blue");

            lightIntensity = ParameterDict.Current.GetDouble(parameterNode + "LightIntensity");
            if (lightIntensity >= 1.0)
                shadowNumber = 0;

            colorGreyness = ParameterDict.Current.GetDouble(parameterNode + "ColorGreyness");
            rgbType = ParameterDict.Current.GetInt(parameterNode + "ColorFactor.RgbType");

            backColorRed = ParameterDict.Current.GetDouble("Renderer.BackColor.Red");
            backColorGreen = ParameterDict.Current.GetDouble("Renderer.BackColor.Green");
            backColorBlue = ParameterDict.Current.GetDouble("Renderer.BackColor.Blue");

            if (lightIntensity > 1)
                lightIntensity = 1;
            if (lightIntensity < 0)
                lightIntensity = 0;

            picInfo = new int[pData.Width, pData.Height];

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    picInfo[i, j] = 0;
                }
            }
            if (stopRequest)
                return;
            CreateStatisticInfo();
            if (useSharpShadow)
                CreateSharpShadow();
            if (stopRequest)
                return;
            CreateSmoothNormales();
            if (stopRequest)
                return;
            CreateSmoothDeph();
            if (stopRequest)
                return;
            CreateShadowInfo();
            if (stopRequest)
                return;
            DrawPlane();
            if (stopRequest)
                return;
            if (ParameterDict.Current.GetBool(parameterNode + "Normalize"))
                NormalizePlane();
            if (stopRequest)
                return;
            if (ParameterDict.Current.GetBool(parameterNode + "UseDarken"))
                DarkenPlane();
            if (stopRequest)
                return;
            SmoothEmptyPixel();
            if (stopRequest)
                return;
            SmoothPlane();
        }


        /// <summary>
        /// Creates boundingbox infos.
        /// </summary>
        protected void CreateStatisticInfo()
        {
            minPoint.X = Double.MaxValue;
            minPoint.Y = Double.MaxValue;
            minPoint.Z = Double.MaxValue;
            maxPoint.X = Double.MinValue;
            maxPoint.Y = Double.MinValue;
            maxPoint.Z = Double.MinValue;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null)
                    {
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
        protected override Vec3 GetRgbAt(int x, int y)
        {
            return rgbPlane[x, y];
        }


        /// <summary>
        /// Erzeugt das Bild im rgb-Format
        /// </summary>
        protected void DrawPlane()
        {
            rgbPlane = new Vec3[pData.Width, pData.Height];
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    rgbPlane[i, j] = GetRgb(i, j);
                }
            }
        }


        /// <summary>
        /// Bildpunkte, die auf Grund fehlender Informationen nicht geladen werden konnten, werden
        /// aus den Umgebungsinformationen gemittelt.
        /// </summary>
        protected void SmoothEmptyPixel()
        {
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    PixelInfo pInfo = pData.Points[i, j];

                    if (pInfo == null)
                    { // Dieser Wert ist zu setzen
                        // Aber nur, wenn es sich nicht um den Hintergrund handelt.
                        Vec3 col = rgbPlane[i, j];
                        col.X = backColorRed; col.Y = backColorGreen; col.Z = backColorBlue;
                        double pixelCount = 0;
                        for (int k = i - 1; k <= i + 1; k++)
                        {
                            for (int l = j - 1; l <= j + 1; l++)
                            {
                                if (k >= 0 && k < pData.Width && l >= 0 && l < pData.Height && k != i && l != j)
                                {
                                    PixelInfo pInfo2 = pData.Points[k, l];
                                    if (pInfo2 != null)
                                    {
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
                        if (pixelCount > 0)
                        {
                            col.X /= pixelCount;
                            col.Y /= pixelCount;
                            col.Z /= pixelCount;
                        }
                    }
                }
            }
        }




        /// <summary>
        /// Der Schlagschatten wird erzeugt.
        /// </summary>
        protected void CreateSharpShadow()
        {
            // Erst ab 4 Pixel handelt es sich um einen Schlagschatten
            int shadowIntensityCount = 4;
            // Je höher, desto größer die Rechenzeit und desto genau die Schlagschattenberechnung.
            int shadowCorrectness = 100;
            double rayDist = minPoint.Dist(maxPoint);
            sharpShadow = new double[pData.Width, pData.Height];
            double[,] sharpTempShadow = new double[pData.Width, pData.Height];

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    picInfo[i, j] = 0;
                    sharpShadow[i, j] = 0;
                }
            }

            Vec3 normal = null;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null)
                    {
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

            for (int m = 0; m < 2; m++)
            {

                for (int i = 0; i < pData.Width; i++)
                {
                    for (int j = 0; j < pData.Height; j++)
                    {
                        double neighborsFound = 0;
                        double sumNeighbors = 0;
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int posX = i + k;
                                int posY = j + l;
                                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                                {
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
        protected Vec3 GetRgb(int x, int y)
        {

            Vec3 retVal = new Vec3(0, 0, 1); // blau
            PixelInfo pInfo = pData.Points[x, y];
            if (pInfo == null)
            {
                return new Vec3(backColorRed, backColorGreen, backColorBlue);
            }

            Vec3 light = new Vec3(0, 0, 0);
            Vec3 normal = null;

            normal = normalesSmooth1[x, y];
            if (normal == null) { normal = pInfo.Normal; }
            // Testweise original Normale verwenden
            //  normal = pInfo.Normal;
            // TODO: Obiges auskommentieren
            if (normal == null)
                return new Vec3(0, 0, 0);

            // tempcoord2 enthält die umgerechnete Oberflächennormale. 
            double tfac = 1000;

            Vec3 coord = formula.GetTransform(pInfo.Coord.X, pInfo.Coord.Y, pInfo.Coord.Z);
            normal.Normalize();
            Vec3 tempcoord2 = formula.GetTransform(pInfo.Coord.X + tfac * normal.X, pInfo.Coord.Y + tfac * normal.Y, pInfo.Coord.Z + tfac * normal.Z);

            tempcoord2.X -= coord.X;
            tempcoord2.Y -= coord.Y;
            tempcoord2.Z -= coord.Z;

            // Normalize:
            tempcoord2.Normalize();

            // debug only
            tempcoord2 = normal;

            if (pInfo.Normal != null)
            {
                light = GetLight(tempcoord2);
                if (sharpShadow != null) { light = light.Mult(1 - sharpShadow[x, y]); }
            }

            retVal.X = light.X;
            retVal.Y = light.Y;
            retVal.Z = light.Z;

            double d1 = maxY - minY;
            double d2 = pData.Width + pData.Height;
            double d3 = d1 / d2;

            retVal.X = (lightIntensity * retVal.X + (1 - lightIntensity) * (1 - shadowPlane[x, y]));
            retVal.Y = (lightIntensity * retVal.Y + (1 - lightIntensity) * (1 - shadowPlane[x, y]));
            retVal.Z = (lightIntensity * retVal.Z + (1 - lightIntensity) * (1 - shadowPlane[x, y]));

            if (retVal.X < 0)
                retVal.X = 0;
            if (retVal.Y < 0)
                retVal.Y = 0;
            if (retVal.Z < 0)
                retVal.Z = 0;

            if (retVal.X > 1)
                retVal.X = 1;
            if (retVal.Y > 1)
                retVal.Y = 1;
            if (retVal.Z > 1)
                retVal.Z = 1;

            double brightLightLevel = ParameterDict.Current.GetDouble("Renderer.BrightLightLevel");
            if (brightLightLevel > 0)
            {
                retVal.X = (1 - brightLightLevel) * retVal.X + brightLightLevel * light.X * (1 - shadowPlane[x, y]);
                retVal.Y = (1 - brightLightLevel) * retVal.Y + brightLightLevel * light.Y * (1 - shadowPlane[x, y]);
                retVal.Z = (1 - brightLightLevel) * retVal.Z + brightLightLevel * light.Z * (1 - shadowPlane[x, y]);
            }

            if (retVal.X < 0)
                retVal.X = 0;
            if (retVal.Y < 0)
                retVal.Y = 0;
            if (retVal.Z < 0)
                retVal.Z = 0;

            if (retVal.X > 1)
                retVal.X = 1;
            if (retVal.Y > 1)
                retVal.Y = 1;
            if (retVal.Z > 1)
                retVal.Z = 1;

            // Add surface color
            bool useAdditionalColorinfo = true;
            if (colorIntensity <= 0)
                useAdditionalColorinfo = false;
            if (useAdditionalColorinfo)
            {
                if (pInfo != null && pInfo.AdditionalInfo != null)
                {
                    // Normalise;
                    double r1 = colorFactorRed * Math.Pow(pInfo.AdditionalInfo.red, colorIntensity);
                    double g1 = colorFactorGreen * Math.Pow(pInfo.AdditionalInfo.green, colorIntensity);
                    double b1 = colorFactorBlue * Math.Pow(pInfo.AdditionalInfo.blue, colorIntensity);
                    if (r1 < 0)
                        r1 = -r1;
                    if (g1 < 0)
                        g1 = -g1;
                    if (b1 < 0)
                        b1 = -b1;

                    // Normalize:
                    double norm = Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1);
                    r1 = r1 / norm;
                    g1 = g1 / norm;
                    b1 = b1 / norm;


                    /*
                    double redLumen = 0.08;
                    double greenLumen = 0.9;
                    double blueLumen = 0.1;
                     */
                    double redLumen = 1.5;
                    double greenLumen = 1.1;
                    double blueLumen = 0.15;


                    /*
                    double redLumen = 0.08;
                    double greenLumen = 0.9;
                    double blueLumen = 0.03;
                     */


                    double norm1 = redLumen * r1 + greenLumen * g1 + blueLumen * b1;

                    r1 = r1 / norm1;
                    g1 = g1 / norm1;
                    b1 = b1 / norm1;

                    if (r1 > 1)
                        r1 = 1;
                    if (b1 > 1)
                        b1 = 1;
                    if (g1 > 1)
                        g1 = 1;

                    if (colorGreyness > 0)
                    {
                        r1 = 0.5 * colorGreyness + (1 - colorGreyness) * r1;
                        g1 = 0.5 * colorGreyness + (1 - colorGreyness) * g1;
                        b1 = 0.5 * colorGreyness + (1 - colorGreyness) * b1;
                    }

                    if (norm != 0)
                    {
                        switch (rgbType)
                        {
                            case 1:
                                retVal.X *= r1;
                                retVal.Y *= g1;
                                retVal.Z *= b1;
                                break;

                            case 2:
                                retVal.X *= r1;
                                retVal.Y *= b1;
                                retVal.Z *= g1;
                                break;

                            case 3:
                                retVal.X *= g1;
                                retVal.Y *= r1;
                                retVal.Z *= b1;
                                break;

                            case 4:
                                retVal.X *= g1;
                                retVal.Y *= b1;
                                retVal.Z *= r1;
                                break;

                            case 5:
                                retVal.X *= b1;
                                retVal.Y *= r1;
                                retVal.Z *= g1;
                                break;

                            case 6:
                                retVal.X *= b1;
                                retVal.Y *= g1;
                                retVal.Z *= r1;
                                break;

                            default:
                                retVal.X *= r1;
                                retVal.Y *= g1;
                                retVal.Z *= b1;
                                break;

                        }

                    }
                }
            }

            if (contrast != 1)
            {
                retVal.X = Math.Pow(retVal.X, contrast);
                retVal.Y = Math.Pow(retVal.Y, contrast);
                retVal.Z = Math.Pow(retVal.Z, contrast);

            }

            if (brightness > 1)
            {
                retVal.X *= brightness;
                retVal.Y *= brightness;
                retVal.Z *= brightness;
            }

            if (retVal.X < 0)
                retVal.X = 0;
            if (retVal.X > 1)
                retVal.X = 1;
            if (retVal.Y < 0)
                retVal.Y = 0;
            if (retVal.Z < 0)
                retVal.Z = 0;
            if (retVal.Y > 1)
                retVal.Y = 1;
            if (retVal.Z > 1)
                retVal.Z = 1;

            if (pInfo != null && pInfo.AdditionalInfo != null)
            {
                pInfo.AdditionalInfo.red2 = retVal.X;
                pInfo.AdditionalInfo.green2 = retVal.Y;
                pInfo.AdditionalInfo.blue2 = retVal.Z;
            }

            return retVal;
        }


        /// <summary>
        /// Liefert die Farbe der Oberfläche entsprechend der Normalen.
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        protected virtual Vec3 GetLight(Vec3 normal)
        {

            Vec3 retVal = new Vec3(backColorRed, backColorGreen, backColorBlue);
            if (!useLight)
            {
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
        protected virtual void CreateShadowInfo()
        {
            // Noch nicht öffentliche Parameter:
            Random rand = new Random();
            double glow = ParameterDict.Current.GetDouble("Renderer.ShadowGlow");
            // Drei "Schattenlichtquellen"
            // Eine für die Dunklen Tiefen
            // Eine für die breite Normalasicht
            // Und eine für die mit sehr geringen Eintreffwinkel
            // Ist bei perspektivischen Aufnahmen noch unbrauchbar.

            // Shadowlight1
            // 1 ist der Durchschnittswert.
            double shadowlight1Val = 0.1;
            // Die maximale Abweichung der Auftreffwinkel.
            double shadowlight1Range = 1;

            double shadowlight1Intensity = 0.2;

            double shadowlight2Val = 0.2;
            // Die maximale Abweichung der Auftreffwinkel.
            double shadowlight2Range = 2;
            double shadowlight2Intensity = 0.6;


            double shadowlight3Val = 1.5;
            // Die maximale Abweichung der Auftreffwinkel.
            double shadowlight3Range = 0.05;
            double shadowlight3Intensity = 1;


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
            double diffy = shadowJustify * (areaDeph);

            // Main Iteration:
            double yd = 0;
            double ydv = 0;
            double ydh = 0;

            double dShadowNumber = shadowNumber;

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    shadowPlane[i, j] = 0;
                }
            }

            double currentShadowlightRange = 0;
            double shadowVal = 0.1;
            int shadowTypeCount = 0;
            double shadowlight1Level = 0.25;
            double shadowlight2Level = 0.5;
            double shadowlight3Level = 0.25;
            double currentIntensity = 1;

            for (int shadowMode = 0; shadowMode < 3; shadowMode++)
            {
                //       for (int shadowMode = 1; shadowMode <=1; shadowMode++) {
                switch (shadowMode)
                {

                    case 0:
                        diffy = shadowJustify * shadowlight1Val * (areaDeph);
                        shadowVal = shadowlight1Level;
                        currentShadowlightRange = shadowlight1Range;
                        currentIntensity = shadowlight1Intensity;
                        break;

                    case 1:
                        diffy = shadowJustify * shadowlight2Val * (areaDeph);
                        shadowVal = shadowlight2Level;
                        currentShadowlightRange = shadowlight2Range;
                        currentIntensity = shadowlight2Intensity;
                        break;

                    case 2:
                        diffy = shadowJustify * shadowlight3Val * (areaDeph);
                        shadowVal = shadowlight3Level;
                        currentShadowlightRange = shadowlight3Range;
                        currentIntensity = shadowlight3Intensity;
                        break;

                }

                int usedShadowNumber = shadowNumber + 1;
                if (shadowMode == 0 || shadowMode == 2)
                    usedShadowNumber = (int)(0.5 * shadowNumber + 1);
                /*
              if (shadowMode == 3 || shadowMode == 4)
                usedShadowNumber = (int)(0.1 * shadowNumber + 1);
              */


                for (int shadowIter = 1; shadowIter < usedShadowNumber + 1; shadowIter++)
                {

                    yd = diffy / ((double)(pData.Width + pData.Height));
                    ydv = diffy / ((double)(pData.Height));
                    ydh = diffy / ((double)(pData.Width));


                    yd *= (1.0 + currentShadowlightRange * 2.0 * (double)shadowIter / (double)dShadowNumber);
                    ydv *= (1.0 + currentShadowlightRange * 1.2 * (double)shadowIter / (double)dShadowNumber);
                    ydh *= (1.0 + currentShadowlightRange * 1.2 * (double)shadowIter / (double)dShadowNumber);


                    // Clean Plane
                    for (int i = 0; i < pData.Width; i++)
                    {
                        for (int j = 0; j < pData.Height; j++)
                        {
                            shadowTempPlane[i, j] = 0;
                        }
                    }


                    // initialize shadowInfo00, ... shadowInfo11, shadowInfo00sharp, ... , shadowInfo11sharp
                    for (int i = 0; i < pData.Width; i++)
                    {
                        for (int j = 0; j < pData.Height; j++)
                        {
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

                    // Randomize diagonal
                    int currentIntXval = 1;
                    int currentIntYval = 1;


                    shadowTypeCount++;
                    //  if (shadowTypeCount >= 35)
                    if (shadowTypeCount > 11)
                        shadowTypeCount = 1;

                    // Gleichmäßige Aufteilung bei 11


                    //shadowTypeCount = 4;
                    switch (shadowTypeCount)
                    {
                        case 1:
                            currentIntXval = 1;
                            currentIntYval = 1;
                            break;

                        case 2:
                            currentIntXval = 1;
                            currentIntYval = 0;
                            break;

                        case 3:
                            currentIntXval = 0;
                            currentIntYval = 1;
                            break;

                        case 4:
                            currentIntXval = 2;
                            currentIntYval = 1;
                            break;

                        case 5:
                            currentIntXval = 1;
                            currentIntYval = 2;
                            break;

                        case 6:
                            currentIntXval = 3;
                            currentIntYval = 1;
                            break;

                        case 7:
                            currentIntXval = 1;
                            currentIntYval = 3;
                            break;

                        case 8:
                            currentIntXval = 4;
                            currentIntYval = 3;
                            break;

                        case 9:
                            currentIntXval = 3;
                            currentIntYval = 4;
                            break;

                        case 10:
                            currentIntXval = 1;
                            currentIntYval = 5;
                            break;

                        case 11:
                            currentIntXval = 5;
                            currentIntYval = 1;
                            break;

                        case 12:
                            currentIntXval = 2;
                            currentIntYval = 7;
                            break;

                        case 13:
                            currentIntXval = 7;
                            currentIntYval = 2;
                            break;


                        case 14:
                            currentIntXval = 3;
                            currentIntYval = 5;
                            break;
                        case 15:
                            currentIntXval = 5;
                            currentIntYval = 3;
                            break;
                        case 16:
                            currentIntXval = 7;
                            currentIntYval = 3;
                            break;
                        case 17:
                            currentIntXval = 3;
                            currentIntYval = 7;
                            break;
                        case 18:
                            currentIntXval = 8;
                            currentIntYval = 3;
                            break;
                        case 19:
                            currentIntXval = 3;
                            currentIntYval = 8;
                            break;
                        case 20:
                            currentIntXval = 4;
                            currentIntYval = 5;
                            break;
                        case 21:
                            currentIntXval = 5;
                            currentIntYval = 4;
                            break;
                        case 22:
                            currentIntXval = 7;
                            currentIntYval = 4;
                            break;
                        case 23:
                            currentIntXval = 4;
                            currentIntYval = 7;
                            break;
                        case 24:
                            currentIntXval = 1;
                            currentIntYval = 1;
                            break;
                        case 25:
                            currentIntXval = 5;
                            currentIntYval = 4;
                            break;
                        case 26:
                            currentIntXval = 4;
                            currentIntYval = 5;
                            break;
                        case 27:
                            currentIntXval = 0;
                            currentIntYval = 1;
                            break;
                        case 28:
                            currentIntXval = 1;
                            currentIntYval = 0;
                            break;
                        case 29:
                            currentIntXval = 1;
                            currentIntYval = 6;
                            break;
                        case 30:
                            currentIntXval = 6;
                            currentIntYval = 1;
                            break;
                        case 31:
                            currentIntXval = 3;
                            currentIntYval = 7;
                            break;
                        case 32:
                            currentIntXval = 4;
                            currentIntYval = 7;
                            break;
                        case 33:
                            currentIntXval = 3;
                            currentIntYval = 8;
                            break;
                        case 34:
                            currentIntXval = 2;
                            currentIntYval = 7;
                            break;
                        case 35:
                            currentIntXval = 6;
                            currentIntYval = 2;
                            break;
                        case 36:
                        case 37:
                        case 38:
                        case 39:
                        case 40:
                            currentIntXval = 3;
                            currentIntYval = 7;
                            break;

                    }



                    // ***********  generate shadowplane ************

                    for (int i = pData.Width - currentIntXval; i >= 0; i--)
                    {
                        for (int j = pData.Height - currentIntYval; j >= 0; j--)
                        {

                            // *********  Fill shadowInfo11  ***********
                            if (i < pData.Width - currentIntXval && j < pData.Height - currentIntYval)
                            {
                                double localShadow = shadowInfo11[i + currentIntXval, j + currentIntYval] - ydh;
                                if (localShadow > shadowInfo11[i, j] && (rand.NextDouble() < glow))
                                {
                                    shadowInfo11[i, j] = localShadow;
                                }
                                localShadow = shadowInfo11sharp[i + currentIntXval, j + currentIntYval] - sharpness * ydh;
                                if (localShadow > shadowInfo11sharp[i, j] && (rand.NextDouble() < glow))
                                {
                                    shadowInfo11sharp[i, j] = localShadow;
                                }
                            }
                        }
                        for (int j = 0; j < pData.Height; j++)
                        {

                            // *********  Fill shadowInfo01  ***********
                            if (i < pData.Width - currentIntXval && j >= currentIntYval)
                            {
                                double localShadow = shadowInfo01[i + currentIntXval, j - currentIntYval] - ydh;
                                if (localShadow > shadowInfo01[i, j] && (rand.NextDouble() < glow))
                                {
                                    shadowInfo01[i, j] = localShadow;
                                }
                                localShadow = shadowInfo01sharp[i + currentIntXval, j - currentIntYval] - sharpness * ydh;
                                if (localShadow > shadowInfo01sharp[i, j] && (rand.NextDouble() < glow))
                                {
                                    shadowInfo01sharp[i, j] = localShadow;
                                }
                            }
                        }
                    }


                    for (int i = 0; i < pData.Width; i++)
                    {

                        // *********  Fill shadowInfo10  ***********
                        for (int j = pData.Height - currentIntXval; j >= 0; j--)
                        {
                            if (i >= currentIntXval && j < pData.Height - currentIntYval)
                            {
                                double localShadow = shadowInfo10[i - currentIntXval, j + currentIntYval] - ydv;
                                if (localShadow > shadowInfo10[i, j] && (rand.NextDouble() < glow))
                                    shadowInfo10[i, j] = localShadow;
                                localShadow = shadowInfo10sharp[i - currentIntXval, j + currentIntYval] - sharpness * ydv;
                                if (localShadow > shadowInfo10sharp[i, j] && (rand.NextDouble() < glow))
                                    shadowInfo10sharp[i, j] = localShadow;
                            }
                        }

                        // *********  Fill shadowInfo00  ***********
                        for (int j = 0; j < pData.Height; j++)
                        {
                            if (i >= currentIntXval && j >= currentIntYval)
                            {
                                double localShadow = shadowInfo00[i - currentIntXval, j - currentIntYval] - ydh;
                                if (localShadow > shadowInfo00[i, j] && (rand.NextDouble() < glow))
                                    shadowInfo00[i, j] = localShadow;
                                localShadow = shadowInfo00sharp[i - currentIntXval, j - currentIntYval] - sharpness * ydh;
                                if (localShadow > shadowInfo00sharp[i, j] && (rand.NextDouble() < glow))
                                    shadowInfo00sharp[i, j] = localShadow;
                            }
                        }
                    }


                    // *********  Combine shadowInfo00, ..., shadowInfo11sharp  ***********
                    // *********  create shadowTempPlane **********

                    for (int i = 0; i < pData.Width; i++)
                    {
                        for (int j = 0; j < pData.Height; j++)
                        {

                            double shadowMapEntry = 0;
                            double currentShadowMapEntry = 0;
                            double height = smoothDeph1[i, j];
                            double shadowHeight = 0;
                            double sharpShadowHeight = 0;

                            for (int k = 0; k < 4; k++)
                            {

                                switch (k)
                                {

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


                                if (height != double.MinValue)
                                {
                                    if (height < sharpShadowHeight) // inside the sharp shadow
                                        currentShadowMapEntry += shadowVal;
                                    if (height < shadowHeight) // inside the shadow
                                        currentShadowMapEntry += shadowVal;

                                    // Test:
                                    //currentShadowMapEntry = 0;
                                    shadowMapEntry += currentShadowMapEntry;
                                }
                            }
                            // shadowMapEntry /= 1114.0;
                            shadowMapEntry /= 16.0;
                            if (shadowMapEntry > 1)
                                shadowMapEntry = 1;
                            shadowMapEntry += shadowTempPlane[i, j];
                            shadowMapEntry /= 2.0;
                            if (shadowMapEntry > 1)
                                shadowMapEntry = 1;
                            shadowTempPlane[i, j] = shadowMapEntry;
                        }
                    }

                    for (int i = 0; i < pData.Width; i++)
                    {
                        for (int j = 0; j < pData.Height; j++)
                        {
                            shadowPlane[i, j] += currentIntensity * shadowTempPlane[i, j] / dShadowNumber;
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


            // Normalize:
            double sMin = Double.MaxValue;
            double sMax = Double.MinValue;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    if (sMin > shadowPlane[i, j])
                        sMin = shadowPlane[i, j];
                    if (sMax < shadowPlane[i, j])
                        sMax = shadowPlane[i, j];

                }
            }
            double sDiff = sMax - sMin;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    shadowPlane[i, j] = (shadowPlane[i, j] - sMin) / sDiff;
                    if (double.IsNaN(shadowPlane[i, j]) || double.IsInfinity(shadowPlane[i, j]))
                    {
                        shadowPlane[i, j] = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Die Oberflächennormalen werden abgerundet.
        /// </summary>
        protected void CreateSmoothNormales()
        {
            normalesSmooth1 = new Vec3[pData.Width, pData.Height];
            normalesSmooth2 = new Vec3[pData.Width, pData.Height];

            // Normieren
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null)
                    {
                        // pInfo.Normal.Normalize();
                        normalesSmooth1[i, j] = pInfo.Normal;
                        normalesSmooth1[i, j].Normalize();
                    }
                }
            }

            Vec3[,] currentSmooth = normalesSmooth1;
            Vec3[,] nextSmooth = normalesSmooth2;
            Vec3[,] tempSmooth;

            int smoothLevel = (int)ParameterDict.Current.GetDouble("Renderer.SmoothNormalLevel");
            for (int currentSmoothLevel = 0; currentSmoothLevel < smoothLevel; currentSmoothLevel++)
            {

                // create nextSmooth
                for (int i = 0; i < pData.Width; i++)
                {
                    for (int j = 0; j < pData.Height; j++)
                    {
                        Vec3 center = null;
                        center = currentSmooth[i, j];
                        PixelInfo pInfo = pData.Points[i, j];
                        // Test ohne smooth-Factor
                        // Nachbarelemente zusammenrechnen
                        Vec3 neighbors = new Vec3();
                        int neighborFound = 0;
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int posX = i + k;
                                int posY = j + l;
                                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                                {
                                    Vec3 currentNormal = null;
                                    currentNormal = currentSmooth[i + k, j + l];
                                    PixelInfo pInfo2 = pData.Points[i + k, j + l];

                                    if (currentNormal != null)
                                    {
                                        double amount = 1;
                                        if (pInfo != null && pInfo2 != null)
                                        {
                                            double dist = pInfo.Coord.Dist(pInfo2.Coord);

                                            double dGlobal = maxPoint.Dist(minPoint);
                                            dGlobal /= 1500;
                                            if (dist < dGlobal)
                                                amount = 1.0;
                                            else if (dist > dGlobal && dist < 5.0 * dGlobal)
                                                amount = 1.0 - (dGlobal / dist / 5.0);
                                            else
                                                amount = 0;
                                        }

                                        neighbors.Add(currentNormal.Mult(amount));
                                        neighborFound++;
                                    }
                                }
                            }
                        }
                        neighbors.Normalize();
                        if (center != null)
                        {
                            nextSmooth[i, j] = center;
                            if (center != null || neighborFound > 1)
                            {
                                Vec3 center2 = center;
                                center2.Mult(200);
                                neighbors.Add(center2.Mult(4));
                                neighbors.Normalize();
                                nextSmooth[i, j] = neighbors;
                            }
                        }
                        else
                        {
                            if (neighborFound > 4)
                            {
                                nextSmooth[i, j] = neighbors;
                            }
                        }
                    }
                }

                tempSmooth = currentSmooth;
                currentSmooth = nextSmooth;
                nextSmooth = tempSmooth;

            }


        }


        /// <summary>
        /// Lokale Tiefeninformationen werden erzeugt.
        /// </summary>
        protected void CreateSmoothDeph()
        {
            smoothDeph1 = new double[pData.Width, pData.Height];
            smoothDeph2 = new double[pData.Width, pData.Height];

            // Normieren
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null)
                    {
                        smoothDeph2[i, j] = pInfo.Coord.Y;
                        smoothDeph1[i, j] = pInfo.Coord.Y;
                        // if (pInfo.Coord.Y != 0) { // Unterscheidung, ob Schnitt mit Begrenzung vorliegt.
                        if (minY > pInfo.Coord.Y && pInfo.Coord.Y != 0)
                            minY = pInfo.Coord.Y;
                        if (maxY < pInfo.Coord.Y)
                            maxY = pInfo.Coord.Y;
                        //}
                    }
                    else
                    {
                        smoothDeph1[i, j] = double.MinValue;
                        smoothDeph2[i, j] = double.MinValue;
                    }
                }
            }



            //    SetSmoothDeph(smoothDeph1, smoothDeph2);
        }


        /// <summary>
        /// Tiefeninformationen werden weicher gemacht.
        /// </summary>
        /// <param name="sdeph1"></param>
        /// <param name="sdeph2"></param>
        protected virtual void SetSmoothDeph(double[,] sdeph1, double[,] sdeph2)
        {

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    int neighborFound = 0;
                    double smoothDeph = 0;
                    sdeph2[i, j] = double.MinValue;
                    //int k=0;
                    for (int k = -1; k <= 1; k++)
                    {
                        //int l = -1;
                        for (int l = -1; l <= 1; l++)
                        {
                            int posX = i + k;
                            int posY = j + l;
                            if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                            {
                                double newDeph = sdeph1[posX, posY];

                                // neu: es werden nur die Punkte benutzt, die echt größer sind
                                if (newDeph != double.MinValue)
                                {
                                    double valToAdded = sdeph1[i, j];
                                    if (newDeph > valToAdded)
                                        valToAdded = newDeph;
                                    smoothDeph += newDeph;
                                    neighborFound++;

                                }
                            }
                        }
                    }
                    if (neighborFound > 0 && sdeph1[i, j] != double.MinValue)
                    {
                        smoothDeph /= (double)neighborFound;
                        sdeph2[i, j] = 1.0 * (0.8 * sdeph1[i, j] + 0.2 * smoothDeph);
                    }
                    else
                    {
                        sdeph2[i, j] = double.MinValue;
                    }
                }
            }
        }



        /// <summary>
        /// Used in field of view computing.
        /// </summary>
        double ydGlobal = 0;

        /// <summary>
        /// Compute field of view.
        /// </summary>
        protected void SmoothPlane()
        {
            double fieldOfViewStart = minFieldOfView;
            ydGlobal = (areaDeph) / ((double)(Math.Max(pData.Width, pData.Height)));
            rgbSmoothPlane1 = new Vec3[pData.Width, pData.Height];
            rgbSmoothPlane2 = new Vec3[pData.Width, pData.Height];
            int intRange = 3;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    rgbSmoothPlane2[i, j] = rgbPlane[i, j];
                }
            }
            if (ambientIntensity == 0)
            {
                //  No field of view defined:
                return;
            }

            // starts with rgbSmoothPlane2
            Vec3[,] currentPlane = rgbSmoothPlane2;
            Vec3[,] nextPlane = rgbSmoothPlane1;
            // contain the result colors
            Vec3[,] resultPlane = rgbSmoothPlane1;

            double mainDeph1 = areaDeph;
            for (int m = 0; m < ambientIntensity; m++)
            {
                if (stopRequest)
                    return;
                for (int i = 0; i < pData.Width; i++)
                {
                    for (int j = 0; j < pData.Height; j++)
                    {
                        double neighborsFound = 0;
                        PixelInfo pInfo = pData.Points[i, j];
                        Vec3 nColor = new Vec3();
                        double ydNormalized = GetAmbientValue(smoothDeph2[i, j]);
                        ydNormalized = Math.Sqrt(ydNormalized);
                        intRange = 1;
                        if (intRange == 0)
                        {
                            nColor = currentPlane[i, j];
                            neighborsFound = 1;
                        }
                        double sumColor = 0;

                        //           if(pData.Points[i, j]!=null) {
                        if (true)
                        {
                            for (int k = -intRange; k <= intRange; k++)
                            {
                                for (int l = -intRange; l <= intRange; l++)
                                {
                                    // Center Pixel is also allowed
                                    // if (k != 0 || l != 0) {
                                    int posX = i + k;
                                    int posY = j + l;
                                    if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                                    {
                                        Vec3 nColor1 = new Vec3();
                                        double ylocalDiff = smoothDeph1[i, j] - smoothDeph1[posX, posY];
                                        if (true)
                                        //   if ( (ylocalDiff > 0) ||(i==posX && j==posY))
                                        //   if ((ylocalDiff < 0) || (i == posX && j == posY))
                                        //   if(false)
                                        {
                                            //double range = (k * k + l * l) / (intRange * intRange);
                                            int range = (k * k + l * l);
                                            double mult = 1;

                                            if (range == 0)
                                            {
                                                // mult = 0.6;
                                                mult = ydNormalized * 0.3;
                                                //mult = 0.2;
                                            }
                                            if (range == 1)
                                            {
                                                //mult = 0.25;
                                                mult = (1.0 - ydNormalized) * 0.4;
                                                //mult = 0.45;
                                            }
                                            if (range == 2)
                                            {
                                                //mult=0.15;
                                                mult = (1.0 - ydNormalized) * 0.3;
                                                //mult = 0.35;

                                            }
                                            // mult += 0.00001;


                                            PixelInfo pInfo2 = pData.Points[posX, posY];

                                            double amount = 1;
                                            if (pInfo != null && pInfo2 != null)
                                            {
                                                double dist = pInfo.Coord.Dist(pInfo2.Coord);

                                                double dGlobal = maxPoint.Dist(minPoint);
                                                dGlobal /= 1500;
                                                if (dist < dGlobal)
                                                    amount = 1.0;
                                                else
                                                    //  else if (dist > dGlobal && dist < 10.0 * dGlobal)
                                                    amount = 1.0 - (dGlobal / dist) / 10.0;
                                                // else
                                                //     amount = 0.0;
                                            }

                                            mult *= amount;
                                            //  mult *= 1.0/ydNormalized;


                                            sumColor += mult;

                                            Vec3 currentColor = currentPlane[posX, posY];
                                            nColor1.X = currentColor.X;
                                            nColor1.Y = currentColor.Y;
                                            nColor1.Z = currentColor.Z;
                                            nColor1 = nColor1.Mult(mult); // Scaling;

                                            nColor.Add(nColor1);
                                            neighborsFound++;
                                        }                                   
                                    }
                                }
                            }
                        }
                       
                        if (neighborsFound > 1)
                        {
                            nColor = nColor.Mult(1 / sumColor);
                        }
                        else
                        {
                            nColor = currentPlane[i, j];
                        }
                        nextPlane[i, j] = nColor;
                    }
                }

                resultPlane = nextPlane;
                nextPlane = currentPlane;
                currentPlane = resultPlane;
            }

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    rgbPlane[i, j] = resultPlane[i, j];
                }
            }

            rgbSmoothPlane1 = null;
            rgbSmoothPlane2 = null;

            return;

        }



        /// <summary>
        /// Get the value, which is used in computing the field of view.
        /// </summary>
        /// <param name="ypos"></param>
        /// <returns></returns>
        protected double GetAmbientValue(double ypos)
        {
            double mainDeph = maxY - minY;

            if (ypos == double.MinValue)
                ypos = minY;
            double ydNormalized = (ypos - minY) / mainDeph;
            double ydist = 0;

            double maxDist = (maxFieldOfView - minFieldOfView);

            if (ydNormalized > maxFieldOfView)
            {
                ydist = ydNormalized - maxFieldOfView;
                maxDist = 1 - maxFieldOfView;
                //ydNormalized = 0;
                //return 0;
            }
            else
            {
                if (ydNormalized < minFieldOfView)
                {
                    ydist = minFieldOfView - ydNormalized;
                    maxDist = minFieldOfView;
                    //ydNormalized = 0;
                    //return 0;
                }
                else
                {
                    ydist = 0; // im field of view 
                    maxDist = 0;
                }
            }


            if (maxDist != 0)
            {
                ydist = ydist / maxDist;
            }


            ydNormalized = 1.0 - ydist;

            // Test only
            if (ydNormalized > 1)
                ydNormalized = 1;

            if (ydNormalized < 0)
                ydNormalized = 0;

            ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = Math.Sqrt(ydNormalized);
            ydNormalized = Math.Sqrt(ydNormalized);

            return ydNormalized;
        }


        /// <summary>
        /// Die Gestalt wird nach hinten abgedunkelt.
        /// </summary>
        protected void DarkenPlane()
        {
            /* Testweise grau */
            double mainDeph = maxY - minY;// borderMaxY - borderMinY;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    Vec3 col = rgbPlane[i, j];
                    double yd = smoothDeph1[i, j];
                    if (yd != double.MinValue)
                    {
                        //if (yd == double.MinValue)
                        //    yd = minY;
                        double ydNormalized = (yd - minY) / mainDeph;
                        /*
                        ydNormalized = Math.Sqrt(ydNormalized);
                        ydNormalized *= 2 * ydNormalized;
                        if (ydNormalized > 0.95) {
                          ydNormalized = 0.95;
                        }
                        ydNormalized += 0.05;
                         */
                        double greyYd = 1.0 - ydNormalized;
                        col.X = col.X * ydNormalized + (backColorRed * greyYd);
                        col.Y = col.Y * ydNormalized + (backColorGreen * greyYd);
                        //ydNormalized += 0.05;
                        col.Z = ydNormalized * col.Z + (backColorBlue * greyYd);
                    }
                    else
                    {
                        col.X = backColorRed;
                        col.Y = backColorGreen;
                        col.Z = backColorBlue;

                    }
                }
            }
        }


        /// <summary>
        /// Weißabgleich und Helligkeitskorrektur.
        /// </summary>
        protected void NormalizePlane()
        {
            double maxRed = 0;
            double maxGreen = 0;
            double maxBlue = 0;

            for (int i = 1; i < pData.Width - 1; i++)
            {
                for (int j = 1; j < pData.Height - 1; j++)
                {
                    if ((picInfo[i, j] == 0) && (picInfo[i + 1, j] == 0) && (picInfo[i - 1, j] == 0) && (picInfo[i, j - 1] == 0) &&
                        (picInfo[i, j + 1] == 0) && (picInfo[i - 1, j - 1] == 0) && (picInfo[i - 1, j + 1] == 0) && (picInfo[i + 1, j - 1] == 0) &&
                        (picInfo[i - 1, j + 1] == 0))
                    {
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
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    Vec3 col = rgbPlane[i, j];
                    if (picInfo[i, j] == 0)
                    {

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
        protected int IsInSharpShadow(Vec3 point, Vec3 ray, double rayLenght, bool inverse, int maxIntersections, int steps)
        {
            //int steps = 100;
            inverse = false;
            double dSteps = steps;
            double dist = 0;
            int shadowCount = 0;
            for (int gSteps = 0; gSteps < 6; gSteps++)
            {
                dist = rayLenght / dSteps;
                Vec3 currentPoint = new Vec3(point);
                currentPoint.Add(ray.Mult(dist));
                for (int i = 0; i < steps; i++)
                {
                    currentPoint.Add(ray.Mult(dist));
                    if (formula.TestPoint(currentPoint.X, currentPoint.Y, currentPoint.Z, inverse))
                    {
                        shadowCount++;
                        if (shadowCount >= maxIntersections)
                            return maxIntersections;
                    }
                    else
                    {
                        //  return false;
                    }
                }
                rayLenght /= 1.4;
            }
            return shadowCount;
        }



    }
}

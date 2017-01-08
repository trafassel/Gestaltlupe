

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
    public class FloatPlasicRenderer : SmallMemoryRenderer
    {


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="pData"></param>
        public FloatPlasicRenderer(PictureData pData, ParameterDict parameters)
            : base(pData, parameters)
        {
        }


        /// <summary>
        /// Coordninate of the bottom, left, front point of the Boundingbox (in original Coordinates).  
        /// </summary>
        private FloatVec3 _minPoint = new FloatVec3(0, 0, 0);


        /// <summary>
        /// Coordninate of the top, right, backside point of the Boundingbox (in original Coordinates).  
        /// </summary>
        private FloatVec3 _maxPoint = new FloatVec3(0, 0, 0);

        /// <summary>
        /// Additional informationen for the picture.
        /// 0 no info
        /// 1 elemtent of the cut with the screen
        /// </summary>
        private int[,] _picInfo = null;

        private float[,] _shadowPlane = null;

        private float[,] _heightMap = null;

        private FloatVec3[,] _rgbPlane = null;

        private FloatVec3[,] _rgbSmoothPlane1 = null;
        private FloatVec3[,] _rgbSmoothPlane2 = null;

        // true for all background pixel.
        private bool[,] _isBackground = null;

        private float _minY = float.MaxValue;
        private float _maxY = float.MinValue;

        // Corresponds to the number of shadows
        private int _shadowNumber = 1;

        // Intensity of the FieldOfView
        private int _ambientIntensity = 4;

        // Intensity of the Surface Color
        private double _colorIntensity = 0.5;

        // if useLight==false, only the shades are computed. 
        private bool _useLight = true;

        // Shadow height factor
        private float _shadowJustify = 1;

        // Influence of the shininess (0 <= shininessFactor <=1)
        private float _shininessFactor = 0.7f;

        // Shininess ( 0... 1000)
        private float _shininess = 8;

        // Normal of the light source     
        private FloatVec3 _lightRay = new FloatVec3();

        private float _colorFactorRed = 1;
        private float _colorFactorGreen = 1;
        private float _colorFactorBlue = 1;

        private float _lightIntensity = 0.5f;

        // If ColorGreyness=1, no color is rendered
        private float _colorGreyness = 0;

        /// <summary>
        /// RGB-Componente type, i.e.:rgbType==4 Change Red and Blue component.
        /// </summary>
        private int _rgbType = 1;

        // Minimal value of FieldOfView
        private float _minFieldOfView = 0;

        // Maximal value of FieldOfView
        private float _maxFieldOfView = 1;

        // Red component of background color 
        private float _backColorRed = (float)0.4;

        // Blue component of background color 
        private float _backColorBlue = (float)0.6;

        // Green component of background color 
        private float _backColorGreen = (float)0.4;

        /// <summary>
        /// Difference between maximal and minimal y value in computing area
        /// </summary>
        private float _areaDeph = 0;

        private float _brightness = 1;

        private float _contrast = 1;

        bool _colorOutside = false;
        bool _colorInside = false;
        bool _transparentBackground = false;

        float _glow = 1;

        // Renderer.ColorFactor.Threshold
        double _colorThreshold = 0;

        /// <summary>
        /// Set fields from _parameters.
        /// </summary>
        protected override void PreCalculate()
        {
            string parameterNode = "Renderer.";
            _colorThreshold = _parameters.GetDouble("Renderer.ColorFactor.Threshold");
            _shadowNumber = _parameters.GetInt(parameterNode + "ShadowNumber");
            _ambientIntensity = _parameters.GetInt(parameterNode + "AmbientIntensity");
            _minFieldOfView = (float)_parameters.GetDouble(parameterNode + "MinFieldOfView");
            _maxFieldOfView = (float)_parameters.GetDouble(parameterNode + "MaxFieldOfView");
            _brightness = (float)_parameters.GetDouble(parameterNode + "Brightness");
            _contrast = (float)_parameters.GetDouble(parameterNode + "Contrast");
            _colorIntensity = _parameters.GetDouble(parameterNode + "ColorIntensity");
            _colorOutside = _parameters.GetBool("Renderer.ColorInside");
            _colorInside = _parameters.GetBool("Renderer.ColorOutside");
            _useLight = _parameters.GetBool(parameterNode + "UseLight");
            _shadowJustify = 0.1f*(float)_parameters.GetDouble(parameterNode + "ShadowJustify");
            _shininessFactor = (float)_parameters.GetDouble(parameterNode + "ShininessFactor");
            _shininess = (float)_parameters.GetDouble(parameterNode + "Shininess");
            _lightRay.X = (float)_parameters.GetDouble(parameterNode + "Light.X");
            _lightRay.Y = (float)_parameters.GetDouble(parameterNode + "Light.Y");
            _lightRay.Z = (float)_parameters.GetDouble(parameterNode + "Light.Z");
            _areaDeph = (float)_parameters.GetDouble("Scene.Radius");
            _transparentBackground = _parameters.GetBool("Renderer.BackColor.Transparent");
            _glow = (float)_parameters.GetDouble("Renderer.ShadowGlow");

            // scale glow to get simmilaier results for different image sizes
            {
                _glow = 1 - (  300.0F/ (float)pData.Width  * (1.0F - _glow));
                     }

            Vec3 coord = formula.GetTransformWithoutProjection(0, 0, 0);
            Vec3 tempcoord2 = formula.GetTransformWithoutProjection(_lightRay.X, _lightRay.Y, _lightRay.Z);
            tempcoord2.X -= coord.X;
            tempcoord2.Y -= coord.Y;
            tempcoord2.Z -= coord.Z;
            tempcoord2.Normalize();
            _lightRay.X = (float)tempcoord2.X;
            _lightRay.Y = (float)tempcoord2.Y;
            _lightRay.Z = (float)tempcoord2.Z;
                 
            _colorFactorRed = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Red");
            _colorFactorGreen = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Green");
            _colorFactorBlue = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Blue");
            _lightIntensity = (float)_parameters.GetDouble(parameterNode + "LightIntensity");
            if (_lightIntensity >= 1.0)
                _shadowNumber = 0;

            _colorGreyness = (float)_parameters.GetDouble(parameterNode + "ColorGreyness");
            _rgbType = _parameters.GetInt(parameterNode + "ColorFactor.RgbType");
            _backColorRed = (float)_parameters.GetDouble("Renderer.BackColor.Red");
            _backColorGreen = (float)_parameters.GetDouble("Renderer.BackColor.Green");
            _backColorBlue = (float)_parameters.GetDouble("Renderer.BackColor.Blue");
            if (_lightIntensity > 1)
                _lightIntensity = 1;
            if (_lightIntensity < 0)
                _lightIntensity = 0;

            _picInfo = new int[pData.Width, pData.Height];
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    _picInfo[i, j] = 0;
                }
            }
            if (_stopRequest)
                return;
            CreateStatisticInfo();
            if (_stopRequest)
                return;

            _isBackground = new bool[pData.Width, pData.Height];
            for (int i = 0; i < pData.Width; i++)
                for (int j = 0; j < pData.Height; j++)
                    _isBackground[i, j] = pData.Points[i, j] == null;

            //CreateSmoothNormales();
            if (_stopRequest)
                return;
            CreateHeightMap();
            if (_stopRequest)
                return;
            CreateShadowInfo();
            if (_stopRequest)
                return;
            DrawPlane();
            if (_stopRequest)
                return;
            if (_parameters.GetBool(parameterNode + "Normalize"))
                NormalizePlane();
            if (_stopRequest)
                return;
            if (_parameters.GetBool(parameterNode + "UseDarken"))
                DarkenPlane();
            if (_stopRequest)
                return;
            //SmoothEmptyPixel();
            if (_stopRequest)
                return;
            SmoothPlane();
        }


        /// <summary>
        /// Creates boundingbox infos.
        /// </summary>
        protected void CreateStatisticInfo()
        {
            _minPoint.X = float.MaxValue;
            _minPoint.Y = float.MaxValue;
            _minPoint.Z = float.MaxValue;
            _maxPoint.X = float.MinValue;
            _maxPoint.Y = float.MinValue;
            _maxPoint.Z = float.MinValue;
            for (int i = 0; i < _pictureData.Width; i++)
            {
                for (int j = 0; j < _pictureData.Height; j++)
                {
                    FloatPixelInfo pInfo = _pictureData.Points[i, j];
                    if (pInfo != null)
                    {
                        FloatVec3 coord = pInfo.Coord;
                        if (coord.X < _minPoint.X)
                            _minPoint.X = coord.X;
                        if (coord.Y < _minPoint.Y)
                            _minPoint.Y = coord.Y;
                        if (coord.Z < _minPoint.Z)
                            _minPoint.Z = coord.Z;
                        if (coord.X > _maxPoint.X)
                            _maxPoint.X = coord.X;
                        if (coord.Y > _maxPoint.Y)
                            _maxPoint.Y = coord.Y;
                        if (coord.Z > _maxPoint.Z)
                            _maxPoint.Z = coord.Z;

                    }
                }
            }
        }


        protected override Vec3 GetRgbAt(int x, int y)
        {
            FloatVec3 rgb = _rgbPlane[x, y];
            return new Vec3(rgb.X, rgb.Y, rgb.Z);
        }

        /// <summary>
        /// Return rgb value at (x,y)
        /// </summary>
        protected override Color GetColor(int x, int y)
        {
            Color col = base.GetColor(x, y);
            if(_transparentBackground)
            {
                if(_isBackground[x,y])
                   col = Color.FromArgb(0, col);
            }
            return col;
        }


        /// <summary>
        /// Create rgb image.
        /// </summary>
        protected void DrawPlane()
        {
            _rgbPlane = new FloatVec3[pData.Width, pData.Height];
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    Vec3 rgb = GetRgb(i, j);
                    if(double.IsNaN(rgb.X)|| double.IsNaN(rgb.Y)|| double.IsNaN(rgb.Z))
                    {

                    }
                    _rgbPlane[i, j] = new FloatVec3((float)rgb.X, (float)rgb.Y, (float)rgb.Z);
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
                    { 
                                         
                        // Dieser Wert ist zu setzen
                        // Aber nur, wenn es sich nicht um den Hintergrund handelt.
                        FloatVec3 col = _rgbPlane[i, j];
                        col.X = _backColorRed;
                        col.Y = _backColorGreen;
                        col.Z = _backColorBlue;
                        float pixelCount = 0;
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
                                        FloatVec3 otherColor = _rgbPlane[k, l];
                                        col.X += otherColor.X;
                                        col.Y += otherColor.Y;
                                        col.Z += otherColor.Z;
                                    }
                                }
                            }
                        }
                        //pixelCount++; // Etwas dunkler sollte es schon werden
                        if (pixelCount > 1)
                        {
                            col.X /= pixelCount;
                            col.Y /= pixelCount;
                            col.Z /= pixelCount;
                            _isBackground[i, j] = false;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Get the color information of the bitmap at (x,y)
        /// </summary>
        protected Vec3 GetRgb(int x, int y)
        {

            FloatVec3 retVal = new FloatVec3(0, 0, 1); // blau
            FloatPixelInfo pInfo = _pictureData.Points[x, y];
            if (pInfo == null)
            {
                return new Vec3(_backColorRed, _backColorGreen, _backColorBlue);
            }

            FloatVec3 light = new FloatVec3(0, 0, 0);
            FloatVec3 normal = null;

            normal = pInfo.Normal;
            if (normal == null)
                return new Vec3(0, 0, 0);

            normal.Normalize();

            if (pInfo.Normal != null)
            {
                light = GetLightF(normal);
            }

            retVal.X = light.X;
            retVal.Y = light.Y;
            retVal.Z = light.Z;


            retVal.X = (float)(_lightIntensity * retVal.X + (1 - _lightIntensity) * (1 - _shadowPlane[x, y]));
            retVal.Y = (float)(_lightIntensity * retVal.Y + (1 - _lightIntensity) * (1 - _shadowPlane[x, y]));
            retVal.Z = (float)(_lightIntensity * retVal.Z + (1 - _lightIntensity) * (1 - _shadowPlane[x, y]));

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

            float brightLightLevel = (float)_parameters.GetDouble("Renderer.BrightLightLevel");
            if (brightLightLevel > 0)
            {
                retVal.X = (1 - brightLightLevel) * retVal.X + brightLightLevel * light.X * (1 - _shadowPlane[x, y]);
                retVal.Y = (1 - brightLightLevel) * retVal.Y + brightLightLevel * light.Y * (1 - _shadowPlane[x, y]);
                retVal.Z = (1 - brightLightLevel) * retVal.Z + brightLightLevel * light.Z * (1 - _shadowPlane[x, y]);
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
            if (_colorIntensity <= 0)
                useAdditionalColorinfo = false;
            
            if (useAdditionalColorinfo && ((pInfo.IsInside && _colorInside) || (!pInfo.IsInside && _colorOutside)))
            {
                if (pInfo != null && pInfo.AdditionalInfo != null)
                {
                    if (double.IsNaN(pInfo.AdditionalInfo.red))
                        pInfo.AdditionalInfo.red = 0;
                    if (double.IsNaN(pInfo.AdditionalInfo.green))
                        pInfo.AdditionalInfo.green = 0;
                    if (double.IsNaN(pInfo.AdditionalInfo.blue))
                        pInfo.AdditionalInfo.blue = 0;

                    if (pInfo.AdditionalInfo.red < 0)
                    {
                        pInfo.AdditionalInfo.green -= pInfo.AdditionalInfo.red;
                        pInfo.AdditionalInfo.blue -= pInfo.AdditionalInfo.red;
                        pInfo.AdditionalInfo.red = 0;
                    }
                    if (pInfo.AdditionalInfo.green < 0)
                    {
                        pInfo.AdditionalInfo.red -= pInfo.AdditionalInfo.green;
                        pInfo.AdditionalInfo.blue -= pInfo.AdditionalInfo.green;
                        pInfo.AdditionalInfo.green = 0;
                    }
                    if (pInfo.AdditionalInfo.blue < 0)
                    {
                        pInfo.AdditionalInfo.red -= pInfo.AdditionalInfo.blue;
                        pInfo.AdditionalInfo.blue -= pInfo.AdditionalInfo.blue;
                        pInfo.AdditionalInfo.blue = 0;
                    }


                    // Normalise;
                    float r1 = (float)(_colorFactorRed * Math.Pow(pInfo.AdditionalInfo.red, _colorIntensity));
                    float g1 = (float)(_colorFactorGreen * Math.Pow(pInfo.AdditionalInfo.green, _colorIntensity));
                    float b1 = (float)(_colorFactorBlue * Math.Pow(pInfo.AdditionalInfo.blue, _colorIntensity));
                    if (r1 < 0)
                        r1 = -r1;
                    if (g1 < 0)
                        g1 = -g1;
                    if (b1 < 0)
                        b1 = -b1;

                    // Normalize:
                    float norm = (float)(Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1) / Math.Sqrt(2.5));
                    if (norm > 0)
                    {
                        r1 = r1 / norm;
                        g1 = g1 / norm;
                        b1 = b1 / norm;

                        if (_colorThreshold > 0)
                        {
                            double thresholdIndicator = Math.Max(Math.Abs(r1 - b1), Math.Max(Math.Abs(r1 - g1), Math.Abs(g1 - b1)));
                            float lowerThresholdFactor = (float)0.7;
                            if (thresholdIndicator < _colorThreshold * lowerThresholdFactor)
                            {
                                r1 = (float)1;
                                g1 = (float)1;
                                b1 = (float)1;
                            }
                            else if (thresholdIndicator < _colorThreshold)
                            {
                                r1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + r1;
                                g1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + g1;
                                b1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + b1;
                            }
                        }

                    }

                    for (int i = 0; i < 5; i++)
                    {
                        if (r1 > 1)
                        {
                            b1 += (r1 - 1) / (float)2.0;
                            g1 += (r1 - 1) / (float)2.0;
                            r1 = 1;
                        }
                        if (b1 > 1)
                        {

                            r1 += (b1 - 1) / (float)2.0;
                            g1 += (b1 - 1) / (float)2.0;
                            b1 = 1;

                        }
                        if (g1 > 1)
                        {

                            r1 += (g1 - 1) / (float)2.0;
                            b1 += (g1 - 1) / (float)2.0;
                            g1 = 1;
                        }
                    }

                    if (r1 > 1)
                        r1 = 1;
                    if (b1 > 1)
                        b1 = 1;
                    if (g1 > 1)
                        g1 = 1;

                    if (_colorGreyness > 0)
                    {
                        r1 = (float)(_colorGreyness + (1 - _colorGreyness) * r1);
                        g1 = (float)(_colorGreyness + (1 - _colorGreyness) * g1);
                        b1 = (float)(_colorGreyness + (1 - _colorGreyness) * b1);
                    }

                    if (r1 > 1)
                        r1 = 1;
                    if (b1 > 1)
                        b1 = 1;
                    if (g1 > 1)
                        g1 = 1;

                    if (norm != 0)
                    {
                        switch (_rgbType)
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

            if (_contrast != 1)
            {
                retVal.X = (float)Math.Pow(retVal.X, _contrast);
                retVal.Y = (float)Math.Pow(retVal.Y, _contrast);
                retVal.Z = (float)Math.Pow(retVal.Z, _contrast);
            }

            if (_brightness > 1)
            {
                retVal.X *= _brightness;
                retVal.Y *= _brightness;
                retVal.Z *= _brightness;
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

            return new Vec3(retVal.X, retVal.Y, retVal.Z);
        }


        /// <summary>
        /// Surface color according to _shininessFactor and _shininess.
        /// </summary>
        protected virtual FloatVec3 GetLightF(FloatVec3 normal)
        {

            FloatVec3 retVal = new FloatVec3((float)_backColorRed, (float)_backColorGreen, (float)_backColorBlue);
            if (!_useLight)
            {
                return new FloatVec3((float)0.5, (float)0.5, (float)0.5);
            }
            if (normal == null)
                return retVal;

            float weight_shini = (float)_shininessFactor;
            float weight_diffuse = 1 - weight_shini;

            float norm = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            // Scalar product with (0,-1,0) light ray
            float angle = 0;
            if (norm == 0)
                return retVal;

            FloatVec3 lightVec = new FloatVec3((float)_lightRay.X, (float)_lightRay.Y, (float)_lightRay.Z);
            lightVec.Normalize();
            float norm2 = lightVec.Norm;
            angle = (float)(Math.Acos((normal.X * lightVec.X + normal.Y * lightVec.Y + normal.Z * lightVec.Z) / (norm * norm2)) / (Math.PI / 2.0));

            angle = 1 - angle;
            if (angle < 0)
                angle = 0;
            if (angle > 1)
                angle = 1;
            float light = (float)(weight_diffuse * angle + weight_shini * Math.Pow(angle, _shininess));
            if (light < 0)
                light = 0;
            if (light > 1)
                light = 1;

            retVal.X = light;
            retVal.Y = light;
            retVal.Z = light;

            return retVal;
        }



        protected virtual void CreateShadowInfo()
        {

            // Noch nicht öffentliche Parameter:
            Random rand = new Random();
            // Drei "Schattenlichtquellen"
            // Eine für die Dunklen Tiefen
            // Eine für die breite Normalasicht
            // Und eine für die mit sehr geringen Eintreffwinkel
            // Ist bei perspektivischen Aufnahmen noch unbrauchbar.

            // Shadowlight1
            // 1 ist der Durchschnittswert.
            float shadowlight1Val = 0.1f;
            // Die maximale Abweichung der Auftreffwinkel.
            float shadowlight1Range = 1;
            float shadowlight1Intensity = 0.2f;
            float shadowlight2Val = 0.2f;
            // Die maximale Abweichung der Auftreffwinkel.
            float shadowlight2Range = 2;
            float shadowlight2Intensity = 0.6f;
            float shadowlight3Val = 1.5f;
            // Die maximale Abweichung der Auftreffwinkel.
            float shadowlight3Range = 0.05f;
            float shadowlight3Intensity = 1;
            float sharpness = 2.5f; // 

            // Beginnend von rechts oben werden die Bereiche, die im Dunklen liegen, berechnet.


         float[,] shadowInfo = null;
         float[,] shadowInfoSharp = null;

            shadowInfo = new float[pData.Width, pData.Height];
            shadowInfoSharp = new float[pData.Width, pData.Height];

            _shadowPlane = new float[pData.Width, pData.Height];
            float[,] shadowTempPlane = new float[pData.Width, pData.Height];
            float diffy = _shadowJustify * (_areaDeph);

            // Main Iteration:
            float yd = 0;
            float ydv = 0;
            float ydh = 0;
            float dShadowNumber = _shadowNumber;

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    _shadowPlane[i, j] = 0;
                }
            }

            float currentShadowlightRange = 0;
            float shadowVal = 0.1f;
            int shadowTypeCount = 0;
            float shadowlight1Level = 0.25f;
            float shadowlight2Level = 0.5f;
            float shadowlight3Level = 0.25f;
            float currentIntensity = 1;

            for (int shadowMode = 0; shadowMode < 3; shadowMode++)
            {
                switch (shadowMode)
                {

                    case 0:
                        diffy = _shadowJustify * shadowlight1Val * (_areaDeph);
                        shadowVal = shadowlight1Level;
                        currentShadowlightRange = shadowlight1Range;
                        currentIntensity = shadowlight1Intensity;
                        break;

                    case 1:
                        diffy = _shadowJustify * shadowlight2Val * (_areaDeph);
                        shadowVal = shadowlight2Level;
                        currentShadowlightRange = shadowlight2Range;
                        currentIntensity = shadowlight2Intensity;
                        break;

                    case 2:
                        diffy = _shadowJustify * shadowlight3Val * (_areaDeph);
                        shadowVal = shadowlight3Level;
                        currentShadowlightRange = shadowlight3Range;
                        currentIntensity = shadowlight3Intensity;
                        break;

                }

                int usedShadowNumber = _shadowNumber + 1;
                if (shadowMode == 0 || shadowMode == 2)
                    usedShadowNumber = (int)(0.5 * _shadowNumber + 1);

                for (int shadowIter = 1; shadowIter < usedShadowNumber + 1; shadowIter++)
                {

                    yd = diffy / ((float)(pData.Width + pData.Height));
                    ydv = diffy / ((float)(pData.Height));
                    ydh = diffy / ((float)(pData.Width));

                    yd *= (1.0f + currentShadowlightRange * 2.0f * (float)shadowIter / (float)dShadowNumber);
                    ydv *= (1.0f + currentShadowlightRange * 1.2f * (float)shadowIter / (float)dShadowNumber);
                    ydh *= (1.0f + currentShadowlightRange * 1.2f * (float)shadowIter / (float)dShadowNumber);

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
                            shadowInfo[i, j] = _heightMap[i, j];
                            shadowInfoSharp[i, j] = _heightMap[i, j];
                        }
                    }

                    // Randomize diagonal
                    int currentIntXval = 1;
                    int currentIntYval = 1;

                    shadowTypeCount++;
                    if (shadowTypeCount > 11)
                        shadowTypeCount = 1;

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
                    for (int k = 0; k < 4; k++)
                    {
                        // Ititialize shadowInfo and shadowInfoSharp
                        for (int i = 0; i < pData.Width; i++)
                        {
                            for (int j = 0; j < pData.Height; j++)
                            {
                                shadowInfo[i, j] = _heightMap[i, j];
                                shadowInfoSharp[i, j] = _heightMap[i, j];
                            }
                        }

                        if (k == 0)
                        {
                            for (int i = pData.Width - currentIntXval; i >= 0; i--)
                            {
                                for (int j = pData.Height - currentIntYval; j >= 0; j--)
                                {
                                    // *********  Fill shadowInfo11  ***********
                                    if (i < pData.Width - currentIntXval && j < pData.Height - currentIntYval)
                                    {
                                        float localShadow = shadowInfo[i + currentIntXval, j + currentIntYval] - ydh;
                                        if (localShadow > shadowInfo[i, j] && (rand.NextDouble() < _glow))
                                        {
                                            shadowInfo[i, j] = localShadow;
                                        }
                                        localShadow = shadowInfoSharp[i + currentIntXval, j + currentIntYval] - sharpness * ydh;
                                        if (localShadow > shadowInfoSharp[i, j] && (rand.NextDouble() < _glow))
                                        {
                                            shadowInfoSharp[i, j] = localShadow;
                                        }
                                    }
                                }
                            }
                        }

                        if (k == 1)
                        {
                            for (int i = pData.Width - currentIntXval; i >= 0; i--)
                            {
                                for (int j = 0; j < pData.Height; j++)
                                {
                                    // *********  Fill shadowInfo01  ***********
                                    if (i < pData.Width - currentIntXval && j >= currentIntYval)
                                    {
                                        float localShadow = shadowInfo[i + currentIntXval, j - currentIntYval] - ydh;
                                        if (localShadow > shadowInfo[i, j] && (rand.NextDouble() < _glow))
                                        {
                                            shadowInfo[i, j] = localShadow;
                                        }
                                        localShadow = shadowInfoSharp[i + currentIntXval, j - currentIntYval] - sharpness * ydh;
                                        if (localShadow > shadowInfoSharp[i, j] && (rand.NextDouble() < _glow))
                                        {
                                            shadowInfoSharp[i, j] = localShadow;
                                        }
                                    }
                                }
                            }
                        }

                        if (k == 2)
                        {
                            for (int i = 0; i < pData.Width; i++)
                            {
                                // *********  Fill shadowInfo10  ***********
                                for (int j = pData.Height - currentIntXval; j >= 0; j--)
                                {
                                    if (i >= currentIntXval && j < pData.Height - currentIntYval)
                                    {
                                        float localShadow = shadowInfo[i - currentIntXval, j + currentIntYval] - ydv;
                                        if (localShadow > shadowInfo[i, j] && (rand.NextDouble() < _glow))
                                            shadowInfo[i, j] = localShadow;
                                        localShadow = shadowInfoSharp[i - currentIntXval, j + currentIntYval] - sharpness * ydv;
                                        if (localShadow > shadowInfoSharp[i, j] && (rand.NextDouble() < _glow))
                                            shadowInfoSharp[i, j] = localShadow;
                                    }
                                }
                            }
                        }

                        if (k == 3)
                        {
                            for (int i = 0; i < pData.Width; i++)
                            {
                                // *********  Fill shadowInfo00  ***********
                                for (int j = 0; j < pData.Height; j++)
                                {
                                    if (i >= currentIntXval && j >= currentIntYval)
                                    {
                                        float localShadow = shadowInfo[i - currentIntXval, j - currentIntYval] - ydh;
                                        if (localShadow > shadowInfo[i, j] && (rand.NextDouble() < _glow))
                                            shadowInfo[i, j] = localShadow;
                                        localShadow = shadowInfoSharp[i - currentIntXval, j - currentIntYval] - sharpness * ydh;
                                        if (localShadow > shadowInfoSharp[i, j] && (rand.NextDouble() < _glow))
                                            shadowInfoSharp[i, j] = localShadow;
                                    }
                                }
                            }
                        }


                        // *********  Combine shadowInfo00, ..., shadowInfo11sharp  ***********
                        // *********  create shadowTempPlane **********
                        for (int i = 0; i < pData.Width; i++)
                        {
                            for (int j = 0; j < pData.Height; j++)
                            {
                                float shadowMapEntry = 0;
                                float currentShadowMapEntry = 0;
                                float height = _heightMap[i, j];
                                float shadowHeight = 0;
                                float sharpShadowHeight = 0;

                                        shadowHeight = shadowInfo[i, j];
                                        sharpShadowHeight = shadowInfoSharp[i, j];

                                if (height != float.MinValue)
                                {
                                    if (height < sharpShadowHeight) // inside the sharp shadow
                                        currentShadowMapEntry += shadowVal;
                                    if (height < shadowHeight) // inside the shadow
                                        currentShadowMapEntry += shadowVal;
                                    shadowMapEntry += currentShadowMapEntry;
                                }

                                shadowMapEntry /= 16.0f;
                                if (shadowMapEntry > 1)
                                    shadowMapEntry = 1;
                                shadowMapEntry += shadowTempPlane[i, j];
                                shadowMapEntry /= 2.0f;
                                if (shadowMapEntry > 1)
                                    shadowMapEntry = 1;
                                shadowTempPlane[i, j] = shadowMapEntry;
                            }
                        }
                    }

                    for (int i = 0; i < pData.Width; i++)
                    {
                        for (int j = 0; j < pData.Height; j++)
                        {
                            _shadowPlane[i, j] += currentIntensity * shadowTempPlane[i, j] / dShadowNumber;
                        }
                    }
                }
            }
            // Release Memory:
            shadowInfo = null;
            shadowInfoSharp = null;

            // Normalize:
            float sMin = float.MaxValue;
            float sMax = float.MinValue;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    if (sMin > _shadowPlane[i, j])
                        sMin = _shadowPlane[i, j];
                    if (sMax < _shadowPlane[i, j])
                        sMax = _shadowPlane[i, j];
                }
            }
            float sDiff = sMax - sMin;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    _shadowPlane[i, j] = (_shadowPlane[i, j] - sMin) / sDiff;
                    if (float.IsNaN(_shadowPlane[i, j]) || float.IsInfinity(_shadowPlane[i, j]))
                    {
                        _shadowPlane[i, j] = 0;
                    }
                }
            }

        }




        /*
        /// <summary>
        /// Die Oberflächennormalen werden abgerundet.
        /// </summary>
        protected void CreateSmoothNormales()
        {
            _normalesSmooth1 = new FloatVec3[pData.Width, pData.Height];
            _normalesSmooth2 = new FloatVec3[pData.Width, pData.Height];

            // Normieren
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    FloatPixelInfo pInfo = _pictureData.Points[i, j];
                    if (pInfo != null)
                    {
                        // pInfo.Normal.Normalize();
                        _normalesSmooth1[i, j] = pInfo.Normal;
                        _normalesSmooth1[i, j].Normalize();
                    }
                }
            }

            FloatVec3[,] currentSmooth = _normalesSmooth1;
            FloatVec3[,] nextSmooth = _normalesSmooth2;
            FloatVec3[,] tempSmooth;
            float maxDist = _maxPoint.Dist(_minPoint);
            float dGlobal = maxDist;
            dGlobal /= 1500;

            int smoothLevel = (int)_parameters.GetDouble("Renderer.SmoothNormalLevel");
            for (int currentSmoothLevel = 0; currentSmoothLevel < smoothLevel; currentSmoothLevel++)
            {

                // create nextSmooth
                for (int i = 0; i < pData.Width; i++)
                {
                    for (int j = 0; j < pData.Height; j++)
                    {
                        FloatVec3 center = null;
                        center = currentSmooth[i, j];
                        FloatPixelInfo pInfo = _pictureData.Points[i, j];
                        // Test ohne smooth-Factor
                        // Nachbarelemente zusammenrechnen
                        FloatVec3 neighbors = new FloatVec3();
                        int neighborFound = 0;
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int posX = i + k;
                                int posY = j + l;
                                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                                {
                                    FloatVec3 currentNormal = null;
                                    currentNormal = currentSmooth[i + k, j + l];
                                    FloatPixelInfo pInfo2 = _pictureData.Points[i + k, j + l];

                                    if (currentNormal != null)
                                    {
                                        float amount = 1;
                                        if (pInfo != null && pInfo2 != null)
                                        {
                                            float dist = pInfo.Coord.Dist(pInfo2.Coord);

                                            if (dist < dGlobal)
                                                amount = 1.0f;
                                            else if (dist > dGlobal && dist < 5.0 * dGlobal)
                                                amount = 1.0f - (dGlobal / dist / 5.0f);
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
                                FloatVec3 center2 = center;
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
        */


        /// <summary>
        /// Field  _heightMap is created from _pictureData.Points.
        /// </summary>
        protected void CreateHeightMap()
        {
            _heightMap = new float[pData.Width, pData.Height];

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    FloatPixelInfo pInfo = _pictureData.Points[i, j];
                    if (pInfo != null)
                    {
                        _heightMap[i, j] = pInfo.Coord.Y;
                        if (_minY > pInfo.Coord.Y && pInfo.Coord.Y != 0)
                            _minY = pInfo.Coord.Y;
                        if (_maxY < pInfo.Coord.Y)
                            _maxY = pInfo.Coord.Y;
                    }
                    else
                    {
                        _heightMap[i, j] = float.MinValue;
                    }
                }
            }
        }
       

        /// <summary>
        /// Compute field of view.
        /// </summary>
        protected void SmoothPlane()
        {
            _rgbSmoothPlane1 = new FloatVec3[pData.Width, pData.Height];
            _rgbSmoothPlane2 = new FloatVec3[pData.Width, pData.Height];
            int intRange = 3;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    _rgbSmoothPlane2[i, j] = _rgbPlane[i, j];
                }
            }
            if (_ambientIntensity == 0)
            {
                //  No field of view defined:
                return;
            }

            // starts with rgbSmoothPlane2
            FloatVec3[,] currentPlane = _rgbSmoothPlane2;
            FloatVec3[,] nextPlane = _rgbSmoothPlane1;
            // contain the result colors
            FloatVec3[,] resultPlane = _rgbSmoothPlane1;

            for (int m = 0; m < _ambientIntensity; m++)
            {
                if (_stopRequest)
                    return;
                for (int i = 0; i < pData.Width; i++)
                {
                    for (int j = 0; j < pData.Height; j++)
                    {
                        double neighborsFound = 0;
                        FloatPixelInfo pInfo = _pictureData.Points[i, j];
                        FloatVec3 nColor = new FloatVec3();
                        float ydNormalized = (float)GetAmbientValue(_heightMap[i, j]);
                        ydNormalized = (float)Math.Sqrt(ydNormalized);
                        intRange = 1;
                        if (intRange == 0)
                        {
                            nColor = currentPlane[i, j];
                            neighborsFound = 1;
                        }
                        float sumColor = 0;

                        //           if(pData.Points[i, j]!=null) {
                        //if (true)
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
                                        FloatVec3 nColor1 = new FloatVec3();
                                        if (true)
                                        //   if ( (ylocalDiff > 0) ||(i==posX && j==posY))
                                        //   if ((ylocalDiff < 0) || (i == posX && j == posY))
                                        //   if(false)
                                        {
                                            //double range = (k * k + l * l) / (intRange * intRange);
                                            int range = (k * k + l * l);
                                            float mult = 1;

                                            if (range == 0)
                                            {
                                                // mult = 0.6;
                                                mult = ydNormalized * 0.3f;
                                                //mult = 0.2;
                                            }
                                            if (range == 1)
                                            {
                                                //mult = 0.25;
                                                mult = (1.0f - ydNormalized) * 0.4f;
                                                //mult = 0.45;
                                            }
                                            if (range == 2)
                                            {
                                                //mult=0.15;
                                                mult = (1.0f - ydNormalized) * 0.3f;
                                                //mult = 0.35;

                                            }
                                            // mult += 0.00001;


                                            FloatPixelInfo pInfo2 = _pictureData.Points[posX, posY];

                                            float amount = 1;
                                            if (pInfo != null && pInfo2 != null)
                                            {
                                                float dist = pInfo.Coord.Dist(pInfo2.Coord);

                                                float dGlobal = _maxPoint.Dist(_minPoint);
                                                dGlobal /= 1500;
                                                if (dist < dGlobal)
                                                    amount = 1.0f;
                                                else
                                                    //  else if (dist > dGlobal && dist < 10.0 * dGlobal)
                                                    amount = 1.0f - (dGlobal / dist) / 10.0f;
                                                // else
                                                //     amount = 0.0;
                                            }

                                            mult *= amount;
                                            //  mult *= 1.0/ydNormalized;


                                            sumColor += mult;

                                            FloatVec3 currentColor = currentPlane[posX, posY];
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
                    _rgbPlane[i, j] = resultPlane[i, j];
                }
            }

            _rgbSmoothPlane1 = null;
            _rgbSmoothPlane2 = null;

            return;

        }


        /// <summary>
        /// Get the value, which is used in computing the field of view.
        /// </summary>
        protected double GetAmbientValue(double ypos)
        {
            double mainDeph = _maxY - _minY;

            if (ypos == double.MinValue)
                ypos = _minY;
            double ydNormalized = (ypos - _minY) / mainDeph;
            double ydist = 0;

            double maxDist = (_maxFieldOfView - _minFieldOfView);

            if (ydNormalized > _maxFieldOfView)
            {
                ydist = ydNormalized - _maxFieldOfView;
                maxDist = 1 - _maxFieldOfView;
            }
            else
            {
                if (ydNormalized < _minFieldOfView)
                {
                    ydist = _minFieldOfView - ydNormalized;
                    maxDist = _minFieldOfView;
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
            float mainDeph = _maxY - _minY;// borderMaxY - borderMinY;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    FloatVec3 col = _rgbPlane[i, j];
                    float yd = _heightMap[i, j];
                    if (yd != float.MinValue)
                    {
                        float ydNormalized = (yd - _minY) / mainDeph;
                        float greyYd = 1.0f - ydNormalized;
                        greyYd = greyYd * greyYd;
                        col.X = col.X * ydNormalized + (_backColorRed * greyYd);
                        col.Y = col.Y * ydNormalized + (_backColorGreen * greyYd);
                        col.Z = ydNormalized * col.Z + (_backColorBlue * greyYd);
                    }
                    else
                    {
                        col.X = _backColorRed;
                        col.Y = _backColorGreen;
                        col.Z = _backColorBlue;
                    }
                }
            }
        }


        /// <summary>
        /// Weißabgleich und Helligkeitskorrektur.
        /// </summary>
        protected void NormalizePlane()
        {
            float maxRed = 0;
            float maxGreen = 0;
            float maxBlue = 0;

            for (int i = 1; i < pData.Width - 1; i++)
            {
                for (int j = 1; j < pData.Height - 1; j++)
                {
                    if ((_picInfo[i, j] == 0) && (_picInfo[i + 1, j] == 0) && (_picInfo[i - 1, j] == 0) && (_picInfo[i, j - 1] == 0) &&
                        (_picInfo[i, j + 1] == 0) && (_picInfo[i - 1, j - 1] == 0) && (_picInfo[i - 1, j + 1] == 0) && (_picInfo[i + 1, j - 1] == 0) &&
                        (_picInfo[i - 1, j + 1] == 0))
                    {
                        FloatVec3 col = _rgbPlane[i, j];
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
                    FloatVec3 col = _rgbPlane[i, j];
                    if (_picInfo[i, j] == 0)
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


    }
}

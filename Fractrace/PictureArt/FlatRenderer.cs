using Fractrace.Basic;
using Fractrace.DataTypes;
using Fractrace.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{


    /// <summary>
    /// Gestaltlupe default Renderer.
    /// </summary>
    public class FlatRenderer : SmallMemoryRenderer
    {


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="pData"></param>
        public FlatRenderer(PictureData pData, ParameterDict parameters)
            : base(pData, parameters)
        {
        }


        /// <summary>
        /// Coordinate of the bottom, left, front point of the Boundingbox (in original Coordinates).  
        /// </summary>
        private FloatVec3 _minPoint = new FloatVec3(0, 0, 0);


        /// <summary>
        /// Coordinate of the top, right, backside point of the Boundingbox (in original Coordinates).  
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

        // Shadow height factor
        private float _shadowJustify = 1;

        private float _colorFactorRed = 1;
        private float _colorFactorGreen = 1;
        private float _colorFactorBlue = 1;

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

        private float _brightness = 1;

        private float _contrast = 1;

        private float _dustlevel = 1;

        bool _colorOutside = false;
        bool _colorInside = false;
        bool _transparentBackground = false;

        float _glow = 1;

        // Renderer.ColorFactor.Threshold
        double _colorThreshold = 0;

        int _quality = 1;


        /// <summary>
        /// Set fields from _parameters.
        /// </summary>
        protected override void PreCalculate()
        {


            string parameterNode = "Renderer.";
            _colorThreshold = _parameters.GetDouble("Renderer.ColorFactor.Threshold");
            _shadowNumber = _parameters.GetInt("Renderer.ShadowNumber");
            _ambientIntensity = _parameters.GetInt("Renderer.AmbientIntensity");
            _minFieldOfView = (float)_parameters.GetDouble("Renderer.MinFieldOfView");
            _maxFieldOfView = (float)_parameters.GetDouble("Renderer.MaxFieldOfView");
            _brightness = (float)_parameters.GetDouble("Renderer.Brightness");
            _contrast = (float)_parameters.GetDouble("Renderer.Contrast");
            _colorIntensity = _parameters.GetDouble("Renderer.ColorIntensity");
            _colorOutside = _parameters.GetBool("Renderer.ColorInside");
            _colorInside = _parameters.GetBool("Renderer.ColorOutside");
            _shadowJustify = 0.1f * (float)_parameters.GetDouble("Renderer.ShadowJustify");
            _transparentBackground = _parameters.GetBool("Renderer.BackColor.Transparent");
            _glow = (float)_parameters.GetDouble("Renderer.ShadowGlow");
            _dustlevel = (float)_parameters.GetDouble("Renderer.Dustlevel");
            _quality=_parameters.GetInt("Renderer.2D.Quality");

            if (pData.Width < 150 && pData.Height < 150)
                _quality = 1;

            if (_glow > 1)
                _glow = 1;
            if (_glow < 0)
                _glow = 0;
            // Scale glow to get simmilaier results for different image sizes.
            double gt = 1 - _glow;
            double gp = ((double)pData.Width) / 1000.0;

            gt = Math.Pow(gt, gp);
            _glow = (float)(1 - gt);

            // Initialize color parameters.
            _colorFactorRed = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Red");
            _colorFactorGreen = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Green");
            _colorFactorBlue = (float)_parameters.GetDouble(parameterNode + "ColorFactor.Blue");
            _colorGreyness = (float)_parameters.GetDouble(parameterNode + "ColorGreyness");
            _rgbType = _parameters.GetInt(parameterNode + "ColorFactor.RgbType");
            _backColorRed = (float)_parameters.GetDouble("Renderer.BackColor.Red");
            _backColorGreen = (float)_parameters.GetDouble("Renderer.BackColor.Green");
            _backColorBlue = (float)_parameters.GetDouble("Renderer.BackColor.Blue");


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
   
            DrawPlane();
         
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
            float count = 0;
//            Color col;
            double red = 0;
            double green = 0;
            double blue = 0;

            for (int i = 0; i < _quality; i++)
                for (int j = 0; j < _quality; j++)
                {
                    count++;
  //                  Color col = base.GetColor(x, y);

                    int newx = _quality * x+i;
                    int newy = _quality * y+j;

                    if (newx >= _width)
                        return Color.Red;
                    if (newy >= _height)
                        return Color.Red;

                    Color col = base.GetColor(newx, newy);
                    //                    FloatVec3 color = _rgbPlane[newx, newy];
                    //                   rgb.Add(color);
                    red += col.R;
                    green += col.G;
                    blue += col.B;
                }
            if (count == 0)
                return Color.Red;

            Color retVal = Color.FromArgb((int)(red/count), (int)(green/count), (int)(blue/count));


//            Color col = base.GetColor(x, y);
            if (_transparentBackground)
            {
                if (_isBackground[x, y])
                    return Color.FromArgb(0, retVal);
            }
            return retVal;
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
                    if (double.IsNaN(rgb.X) || double.IsNaN(rgb.Y) || double.IsNaN(rgb.Z))
                    {
                        _rgbPlane[i, j] = new FloatVec3(1, 0, 0);
                    }
                    _rgbPlane[i, j] = new FloatVec3((float)rgb.X, (float)rgb.Y, (float)rgb.Z);
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

            retVal.X = retVal.Y = retVal.Z = 1;

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
                    //float norm = (float)(Math.Sqrt(r1 * r1 + g1 * g1 + b1 * b1) / Math.Sqrt(2.5));
                    //if (norm > 0)
                    //{
                    //    r1 = r1 / norm;
                    //    g1 = g1 / norm;
                    //    b1 = b1 / norm;

                    //    if (_colorThreshold > 0)
                    //    {
                    //        double thresholdIndicator = Math.Max(Math.Abs(r1 - b1), Math.Max(Math.Abs(r1 - g1), Math.Abs(g1 - b1)));
                    //        float lowerThresholdFactor = (float)0.7;
                    //        if (thresholdIndicator < _colorThreshold * lowerThresholdFactor)
                    //        {
                    //            r1 = (float)1;
                    //            g1 = (float)1;
                    //            b1 = (float)1;
                    //        }
                    //        else if (thresholdIndicator < _colorThreshold)
                    //        {
                    //            r1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + r1;
                    //            g1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + g1;
                    //            b1 = (float)(_colorThreshold - thresholdIndicator) * (float)0.5 / lowerThresholdFactor + b1;
                    //        }
                    //    }

                    //}

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

                    
                                retVal.X = r1;
                                retVal.Y = g1;
                                retVal.Z = b1;
                              
                    
                }
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

        protected virtual void CreateShadowInfo()
        {
            EnvironmentShadowRenderComponent shadowRenderer = new EnvironmentShadowRenderComponent(_pictureData, _parameters, _heightMap);
            _shadowPlane = shadowRenderer.CreateShadowPlane();
        }


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

            // Set heightmap of dust to its lowest neighbour
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    FloatPixelInfo pInfo = _pictureData.Points[i, j];


                    if (pInfo != null)
                    {
                        if (pInfo.dustlevel > _dustlevel)
                        {
                            _heightMap[i, j] = _minY;
                        }
                    }
                    else
                    {
                        // _heightMap[i, j] = float.MinValue;
                    }
                }
            }



        }


        /// <summary>
        /// Compute field of view.
        /// </summary>
        protected void SmoothPlane()
        {
            System.Random rand = new Random();
            _rgbSmoothPlane1 = new FloatVec3[pData.Width, pData.Height];
            _rgbSmoothPlane2 = new FloatVec3[pData.Width, pData.Height];
            int intRange = 3;
            float dGlobal = _maxPoint.Dist(_minPoint);
            dGlobal /= 15;
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

            if (_stopRequest)
                return;
            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    double neighborsFound = 0;
                    float sumColor = 0;
                    FloatVec3 nColor = new FloatVec3();
                    FloatPixelInfo pInfo = _pictureData.Points[i, j];

                    float ydNormalized = (float)GetAmbientValue(_heightMap[i, j]);

                    double ambientIntensity = (ydNormalized) * (double)_ambientIntensity;
                    double ambientIntensitySquare = ambientIntensity * ambientIntensity;
                    intRange = (int)ambientIntensity + 1;
                    float am = (float)Math.Pow(ydNormalized, 0.1);

                    if (pInfo != null)
                    {


                        for (int k = -intRange; k <= intRange; k++)
                        {
                            for (int l = -intRange; l <= intRange; l++)
                            {
                                int posX = i + k;
                                int posY = j + l;
                                if (posX >= 0 && posX < pData.Width && posY >= 0 && posY < pData.Height)
                                {
                                    FloatVec3 nColor1 = new FloatVec3();

                                    int range = (k * k + l * l);
                                    if (range < ambientIntensitySquare)
                                    {

                                        FloatPixelInfo pInfo2 = _pictureData.Points[posX, posY];

                                        float amount = 1;
                                        if (pInfo != null && pInfo2 != null)
                                        {
                                            float dist = pInfo.Coord.Dist(pInfo2.Coord);

                                            if (dist < dGlobal || _heightMap[posX, posY] <= _heightMap[i, j] + 0.1 * dGlobal)
                                                amount = 1.0f;
                                            else
                                            {
                                                if (rand.NextDouble() > ydNormalized)
                                                    amount = 1f;
                                                else
                                                    amount = 0.0f;

                                            }

                                        }

                                        if (amount > 0)
                                        {
                                            sumColor += amount;
                                            FloatVec3 currentColor = currentPlane[posX, posY];
                                            nColor1.X = currentColor.X;
                                            nColor1.Y = currentColor.Y;
                                            nColor1.Z = currentColor.Z;
                                            nColor.Add(nColor1);
                                            neighborsFound++;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (neighborsFound > 1 && sumColor > 0)
                    {
                        nColor = currentPlane[i, j].Mult(1.0f - am).Sum(nColor.Mult(am / sumColor));
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

            ydNormalized = ydist;

            if (ydNormalized > 1)
                ydNormalized = 1;

            if (ydNormalized < 0)
                ydNormalized = 0;

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


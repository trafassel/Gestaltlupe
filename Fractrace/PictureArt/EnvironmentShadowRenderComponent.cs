using Fractrace.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{
    class EnvironmentShadowRenderComponent
    {
        public EnvironmentShadowRenderComponent(FloatPictureData floatPictureData, ParameterDict parameters, float[,] heightMap)
        {
            pData = floatPictureData;
            _heightMap = heightMap;
            _shadowJustify = 0.1f * (float)parameters.GetDouble("Renderer.ShadowJustify");
          //  _areaDeph = parameters.GetDouble("Scene.Radius");
            _shadowNumber = parameters.GetInt("Renderer.ShadowNumber");
            _glow = (float)parameters.GetDouble("Renderer.ShadowGlow");

        }

        FloatPictureData pData = null;

        float[,] _shadowPlane = null;

        // Shadow height factor
        private float _shadowJustify = 1;

        /// <summary>
        /// Previously: difference between maximal and minimal y value in computing area
        /// </summary>
        private float _areaDeph = 1;

        // Corresponds to the number of shadows
        private int _shadowNumber = 1;

        private float[,] _heightMap = null;

        float _glow = 1;

        public float[,] CreateShadowPlane()
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

            return _shadowPlane;
        }
    }
}

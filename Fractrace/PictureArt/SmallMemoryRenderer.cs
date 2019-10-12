using Fractrace.Basic;
using Fractrace.DataTypes;
using Fractrace.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{

    /// <summary>
    /// Renderer with float based  FloatPictureData (instead of PictureData).
    /// </summary>
    public class SmallMemoryRenderer : ScienceRendererBase
    {

        /// <summary>
        /// In the renderer used flot based variant of pData.
        /// </summary>
        protected FloatPictureData _pictureData = null;

        protected ParameterDict _parameters = null;
        /// <summary>
        /// Initialisation.
        /// </summary>
        public SmallMemoryRenderer(PictureData pData, ParameterDict parameters)
            : base(pData)
        {
            _pictureData = new FloatPictureData(pData.Width, pData.Height);
            _parameters = parameters;
        }


        /// <summary>
        /// Initialisation with formula is needed for computing original coordinates.
        /// </summary>
        public override void Init(Formulas formula)
        {
            base.Init(formula);
            // Original data has to scale such that values fits into float range.
            Vec3 minPoint = new Vec3(0, 0, 0);
            Vec3 maxPoint = new Vec3(0, 0, 0);
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
            Vec3 center = new Vec3(0, 0, 0);
            center.X = (maxPoint.X + minPoint.X) / 2.0;
            center.Y = (maxPoint.Y + minPoint.Y) / 2.0;
            center.Z = (maxPoint.Z + minPoint.Z) / 2.0;
            double radius = maxPoint.X - minPoint.X + maxPoint.Y - minPoint.Y + maxPoint.Z - minPoint.Z;

            for (int i = 0; i < pData.Width; i++)
            {
                for (int j = 0; j < pData.Height; j++)
                {
                    {
                        PixelInfo pInfo = pData.Points[i, j];
                    if (pInfo != null)
                    {
                        FloatPixelInfo floatPixelInfo = new FloatPixelInfo();
                        floatPixelInfo.Coord.X = (float)((pInfo.Coord.X - center.X) / radius);
                        floatPixelInfo.Coord.Y = (float)((pInfo.Coord.Y - center.Y) / radius);
                        floatPixelInfo.Coord.Z = (float)((pInfo.Coord.Z - center.Z) / radius);
                        floatPixelInfo.AdditionalInfo = pInfo.AdditionalInfo;
                        floatPixelInfo.IsInside = pInfo.IsInside;
                        floatPixelInfo.dustlevel = (float)pInfo.dustlevel;

                        _pictureData.Points[i, j] = floatPixelInfo;
                        }
                    }

                    {
                        PixelInfo pInfo = pData.SolidPoints[i, j];
                        if (pInfo != null)
                        {
                            FloatPixelInfo floatPixelInfo = new FloatPixelInfo();
                            floatPixelInfo.Coord.X = (float)((pInfo.Coord.X - center.X) / radius);
                            floatPixelInfo.Coord.Y = (float)((pInfo.Coord.Y - center.Y) / radius);
                            floatPixelInfo.Coord.Z = (float)((pInfo.Coord.Z - center.Z) / radius);

                            floatPixelInfo.AdditionalInfo = pInfo.AdditionalInfo;
                            floatPixelInfo.IsInside = pInfo.IsInside;
                            floatPixelInfo.dustlevel = (float)pInfo.dustlevel;

                            _pictureData.SolidPoints[i, j] = floatPixelInfo;
                        }
                    }

                }
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Geometry;
using Fractrace.TomoGeometry;

namespace Fractrace.DataTypes
{

    /// <summary>
    /// Contains additional informations for each point in surface data.
    /// </summary>
    public class PixelInfo
    {

        public PixelInfo()
        {

        }

        public AdditionalPointInfo AdditionalInfo = null;

        /// <summary>
        /// Original coordinates at given pixels
        /// </summary>
        public Vec3 Coord = new Vec3(0, 0, 0);


        /// <summary>
        /// Surface normal at given pixel.
        /// </summary>
      //  public Vec3 Normal = new Vec3(0, 0, 0);


        /// <summary>
        /// True, if the corresponding pixel is part of the inside view
        /// </summary>
        public bool IsInside = false;

        public PixelInfo Clone()
        {
            PixelInfo retVal = new PixelInfo();
            retVal.Coord.X = Coord.X;
            retVal.Coord.Y = Coord.Y;
            retVal.Coord.Z = Coord.Z;
            retVal.IsInside = IsInside;
            if (AdditionalInfo != null)
                retVal.AdditionalInfo = AdditionalInfo.Clone();
            return retVal;
        }

    }
}

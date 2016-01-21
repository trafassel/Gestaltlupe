using Fractrace.Geometry;
using Fractrace.TomoGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{
    public class FloatPixelInfo
    {

        public FloatPixelInfo()
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
        public Vec3 Normal = new Vec3(0, 0, 0);


        /// <summary>
        /// Iteration cout . Used for colorize cut.
        /// </summary>
        public double iterations = -1;


        /// <summary>
        /// True, if the corresponding pixel is part of the inside view
        /// </summary>
        public bool IsInside = false;

    }
}

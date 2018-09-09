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
        public FloatVec3 Coord = new FloatVec3(0, 0, 0);


        /// <summary>
        /// Surface normal at given pixel.
        /// </summary>
        //public FloatVec3 Normal = new FloatVec3(0, 0, 0);


        /// <summary>
        /// True, if the corresponding pixel is part of the inside view
        /// </summary>
        public bool IsInside = false;

    }
}

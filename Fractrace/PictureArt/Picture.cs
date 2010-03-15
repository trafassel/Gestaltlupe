using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.Geometry;

namespace Fractrace.PictureArt
{
    class Picture
    {
        private Vec3[,] rgbPlane = null;

        public Picture(int width, int height)
        {
            rgbPlane = new Vec3[width, height];

        }

    }
}

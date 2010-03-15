using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.PictureArt
{
    class HeighMap
    {

        double[,] mHeightInfo = null;
        public HeighMap(int width, int height)
        {
            mHeightInfo = new double[width, height];

        }
    }
}

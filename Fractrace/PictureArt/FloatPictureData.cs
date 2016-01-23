using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{
    public class FloatPictureData
    {
        public FloatPixelInfo[,] Points = null;


        /// <summary>
        /// Width in pixel
        /// </summary>
        public int Width
        {
            get
            {
                return mWidth;
            }
        }
        private int mWidth = 0;


        /// <summary>
        /// Height in pixel
        /// </summary>
        public int Height
        {
            get
            {
                return mHeight;
            }
        }

        private int mHeight = 0;


        public FloatPictureData(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            Points = new FloatPixelInfo[width + 1, height + 1];
            for (int i = 0; i <= width; i++)
            {
                for (int j = 0; j <= height; j++)
                {
                    Points[i, j] = null;
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes
{
    public class PictureData
    {

        public PixelInfo[,] Points = null;


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

        public PictureData(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            Points = new PixelInfo[width + 1, height + 1];
            for (int i = 0; i <= width; i++)
            {
                for (int j = 0; j <= height; j++)
                {
                    Points[i, j] = null;
                }
            }
        }


        //! Return deep copy.
        public PictureData Clone()
        {
            PictureData retVal = new PictureData(mWidth, mHeight);
            retVal.Points = new PixelInfo[mWidth + 1, mHeight + 1];
            for (int i = 0; i <= mWidth; i++)
            {
                for (int j = 0; j <= mHeight; j++)
                {
                    if (Points[i, j] != null)
                    {
                        retVal.Points[i, j] = Points[i, j].Clone();
                    }
                }
            }
            return retVal;
        }

    }
}

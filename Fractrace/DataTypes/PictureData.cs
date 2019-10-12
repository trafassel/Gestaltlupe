using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes
{

    /// <summary>
    /// Surface Data.
    /// </summary>
    public class PictureData
    {

        public PixelInfo[,] Points = null;
        public PixelInfo[,] SolidPoints = null;

        /// <summary>
        /// Width in pixel
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }
        private int _width = 0;


        /// <summary>
        /// Height in pixel
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        private int _height = 0;


        public PictureData(int width, int height)
        {
            _width = width;
            _height = height;
            Points = new PixelInfo[width + 1, height + 1];
            SolidPoints = new PixelInfo[width + 1, height + 1];
            for (int i = 0; i <= width; i++)
            {
                for (int j = 0; j <= height; j++)
                {
                    Points[i, j] = null;
                    SolidPoints[i, j] = null;
                }
            }
        }


        //! Return deep copy.
        public PictureData Clone()
        {
            PictureData retVal = new PictureData(_width, _height);
            retVal.Points = new PixelInfo[_width + 1, _height + 1];
            retVal.SolidPoints = new PixelInfo[_width + 1, _height + 1];
            for (int i = 0; i <= _width; i++)
            {
                for (int j = 0; j <= _height; j++)
                {
                    if (Points[i, j] != null)
                    {
                        retVal.Points[i, j] = Points[i, j].Clone();
                    }
                    if (SolidPoints[i, j] != null)
                    {
                        retVal.SolidPoints[i, j] = SolidPoints[i, j].Clone();
                    }
                }
            }
            return retVal;
        }

    }
}

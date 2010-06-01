using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes {
    public class PictureData {

        public PixelInfo[,] Points = null;

        /// <summary>
        /// Breite des fertigen Bildes in Pixel
        /// </summary>
        public int Width {
            get {
                return mWidth;
            }
        }
        private int mWidth = 0;

        /// <summary>
        /// Höhe des fertigen Bildes in Pixel
        /// </summary>
        public int Height {
            get {
                return mHeight;
            }
        }
        
        private int mHeight = 0;

        public PictureData(int width, int height) {
            mWidth = width;
            mHeight = height;
            Points = new PixelInfo[width + 1, height + 1];
               for (int i = 0; i <= width; i++) {
                   for (int j = 0; j <= height; j++) {
                       Points[i, j] = null;
                   }
        }

        }
    }
}

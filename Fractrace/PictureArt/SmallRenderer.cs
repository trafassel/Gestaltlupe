using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.PictureArt {
  class SmallRenderer : RenderBase {

     /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public SmallRenderer(PictureData pData):base(pData) {
        }

    /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected override Color GetColor(int x, int y) {
          PixelInfo pInfo = pData.Points[x, y];
          if (pInfo == null) {
            return Color.Red;
          }
          return Color.Black;
        }



  }
}

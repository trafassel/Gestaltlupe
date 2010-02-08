using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

namespace Fractrace.PictureArt {
  class NiceRendererMultiThread : NiceRenderer {

        /// <summary>
    /// Initialisierung
    /// </summary>
    /// <param name="pData"></param>
    public NiceRendererMultiThread(PictureData pData)
      : base(pData) {
    }


  }
}

using System;
using System.Collections.Generic;
using System.Text;

using Fractrace;
using Fractrace.DataTypes;

namespace Fractrace.PictureArt {
    class PictureArtFactory {

        /// <summary>
        /// Hier können eigene Renderer zugeschaltet werden:
        /// </summary>
        /// <returns></returns>
        public static Renderer Create(PictureData pdata) {

           // return new ScienceRenderer(pdata);
          //return new SmallRenderer(pdata);
         //   return new FastScienceRenderer(pdata);
          return new NiceRenderer(pdata);

        }
    }

  
}

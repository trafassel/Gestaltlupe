using System;
using System.Collections.Generic;
using System.Text;

using Fractrace;
using Fractrace.DataTypes;
using Fractrace.Basic;

namespace Fractrace.PictureArt {
    class PictureArtFactory {

        /// <summary>
        /// Hier können eigene Renderer zugeschaltet werden:
        /// </summary>
        /// <returns></returns>
        public static Renderer Create(PictureData pdata) {
          switch (ParameterDict.Exemplar["Composite.Renderer"]) {
            case "NiceRenderer":
              return new NiceRenderer(pdata);
             

            case "FastScienceRenderer":
              return new FastScienceRenderer(pdata);

            case "ScienceRenderer":
              return new ScienceRenderer(pdata);

          }
           // return new ScienceRenderer(pdata);
          //return new SmallRenderer(pdata);
         //   return new FastScienceRenderer(pdata);
          return new NiceRenderer(pdata);

        }
    }

  
}

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

        case "5":
        case "UniversalRenderer":
          return new UniversalRenderer(pdata);


        case "4":
        case "SharpRenderer":
          return new SharpRenderer(pdata);

        case "3":
        case "BroadcastRenderer":
          return new BroadcastRenderer(pdata);


        case "NiceRenderer":
        case "2":
          return new NiceRenderer(pdata);

        case "FastScienceRenderer":
        case "0":
          return new FastScienceRenderer(pdata);

        case "ScienceRenderer":
        case "1":
          return new ScienceRenderer(pdata);

      }
      // return new ScienceRenderer(pdata);
      //return new SmallRenderer(pdata);
      //   return new FastScienceRenderer(pdata);
      return new NiceRenderer(pdata);

    }
  }


}

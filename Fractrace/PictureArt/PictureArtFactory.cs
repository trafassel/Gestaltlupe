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
    public static Renderer Create(PictureData pdata,Formulas formula) {
        Renderer retVal = null;

      switch (ParameterDict.Exemplar["Composite.Renderer"]) {

        case "5":
        case "UniversalRenderer":
          retVal= new UniversalRenderer(pdata);
          break;

        case "4":
        case "SharpRenderer":
          retVal = new SharpRenderer(pdata);
          break;

        case "3":
        case "BroadcastRenderer":
          retVal = new BroadcastRenderer(pdata);
          break;

        case "NiceRenderer":
        case "2":
          retVal= new NiceRenderer(pdata);
          break;

        case "FastScienceRenderer":
        case "0":
          retVal= new FastScienceRenderer(pdata);
          break;

        case "ScienceRenderer":
        case "1":
          retVal = new ScienceRenderer(pdata);
          break;

        case "PlasicRenderer":
        case "6":
          retVal = new PlasicRenderer(pdata);
          break;

        case "FrontViewRenderer":
        case "7":
          retVal = new FrontViewRenderer(pdata);
          break;
          
      }

      if (retVal != null) {
          retVal.Init(formula);
          return retVal;
      }

      // return new ScienceRenderer(pdata);
      //return new SmallRenderer(pdata);
      //   return new FastScienceRenderer(pdata);
      return new NiceRenderer(pdata);

    }
  }


}

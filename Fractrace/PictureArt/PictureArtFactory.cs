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

        case "":
        case "PlasicRenderer":
        case "6":
          retVal = new PlasicRenderer(pdata.Clone());
          break;

        case "FastPreviewRenderer":
        case "8":
          retVal = new FastPreviewRenderer(pdata.Clone());
          break;

        default:
          retVal = new PlasicRenderer(pdata.Clone());
          break;
      }

      if (retVal != null) 
          retVal.Init(formula);
          
        return retVal;
     

    }
  }


}

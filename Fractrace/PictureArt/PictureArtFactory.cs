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
    public static Renderer Create(PictureData pdata,Formulas formula, ParameterDict dict) {
        Renderer retVal = null;

            switch (ParameterDict.Current["View.Renderer"])
            {
                case "front":
                    retVal = new FrontViewRenderer(pdata.Clone());
                    break;

                case "3d":
                    retVal = new FloatPlasicRenderer(pdata.Clone(), dict);
                    break;

                default:
                    retVal = new FloatPlasicRenderer(pdata.Clone(), dict);
                    break;

            }

         
            /*
            switch (ParameterDict.Current["Composite.Renderer"]) {

        case "":
        case "PlasicRenderer":
        case "6":
                    //retVal = new PlasicRenderer(pdata.Clone());
                    retVal = new FrontViewRenderer(pdata.Clone());
                 //   retVal = new FloatPlasicRenderer(pdata.Clone(), dict);
                    break;

        case "FastPreviewRenderer":
        case "8":
          retVal = new FastPreviewRenderer(pdata.Clone());
          break;

        default:
                    retVal = new FloatPlasicRenderer(pdata.Clone(), dict);
                    break;
      }
      */

      if (retVal != null) 
          retVal.Init(formula);
          
        return retVal;
     

    }
  }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fractrace.Basic;

namespace Fractrace.PictureArt
{
    public class FastRenderingFilter:  ParameterDictFilter
    {

        /// <summary>
        /// Apply filter to current values.
        /// </summary>
        protected override void Filter()
        {
            ParameterDict.Exemplar["Renderer.ShadowNumber"] = "11";
            ParameterDict.Exemplar["Renderer.AmbientIntensity"] = "0";
            ParameterDict.Exemplar["Renderer.SmoothNormalLevel"] = "3";
        }


    }
}

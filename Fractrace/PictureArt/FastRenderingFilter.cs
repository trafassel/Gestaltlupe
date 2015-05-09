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
            ParameterDict.Exemplar.SetValue("Renderer.ShadowNumber","11",false);
            ParameterDict.Exemplar.SetValue("Renderer.AmbientIntensity","0",false);
            ParameterDict.Exemplar.SetValue("Renderer.SmoothNormalLevel","3",false);

            ParameterDict.Exemplar.SetValue("Intern.Filter", "FastRenderingFilter", false);
        }


    }
}

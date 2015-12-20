using Fractrace.Basic;

namespace Fractrace.PictureArt
{

    /// <summary>
    /// Reset some time consuming render values. 
    /// </summary>
    public class FastRenderingFilter:  ParameterDictFilter
    {

        /// <summary>
        /// Apply filter to current values.
        /// </summary>
        protected override void Filter()
        {
            ParameterDict.Current.SetValue("Renderer.ShadowNumber","11",false);
            ParameterDict.Current.SetValue("Renderer.AmbientIntensity","0",false);
            ParameterDict.Current.SetValue("Renderer.SmoothNormalLevel","3",false);
            ParameterDict.Current.SetValue("Intern.Filter", "FastRenderingFilter", false);
        }


    }
}

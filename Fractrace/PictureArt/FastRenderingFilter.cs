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
        public override ParameterDict Apply()
        {
            _dict.SetValue("Renderer.ShadowNumber","11",false);
            _dict.SetValue("Renderer.AmbientIntensity","0",false);
            _dict.SetValue("Renderer.SmoothNormalLevel","3",false);
            _dict.SetValue("Intern.Filter", "FastRenderingFilter", false);
            return _dict;
        }


    }
}



namespace Fractrace.Basic
{

    /// <summary>
    /// Used to update loaded parameters from old version.
    /// </summary>
    public class ParameterUpdater
    {


        /// <summary>
        /// Update 
        /// </summary>
        public static void Update()
        {
            if (ParameterDict.Current["Transformation.AngleX"] == "0" &&
                ParameterDict.Current["Transformation.AngleY"] == "0" &&
                ParameterDict.Current["Transformation.AngleZ"] == "0")
            {
                ParameterDict.Current.RemoveProperty("Transformation.AngleX");
                ParameterDict.Current.RemoveProperty("Transformation.AngleY");
                ParameterDict.Current.RemoveProperty("Transformation.AngleZ");
            }

            if (ParameterDict.Current["Transformation.3.AngleX"] != string.Empty )
            {
                ParameterDict.Current.RemoveProperty("Transformation.3.AngleX");
                ParameterDict.Current.RemoveProperty("Transformation.3.AngleY");
                ParameterDict.Current.RemoveProperty("Transformation.3.AngleZ");
                ParameterDict.Current.RemoveProperty("Transformation.3.CenterX");
                ParameterDict.Current.RemoveProperty("Transformation.3.CenterY");
                ParameterDict.Current.RemoveProperty("Transformation.3.CenterZ");
            }
            ParameterDict.Current["View.Pipeline.UpdatePreview"] = "1";
            ParameterDict.Current["View.Pipeline.Preview"] = "0";
            ParameterDict.Current.RemoveProperty("View.Zoom");
        }


    }
}

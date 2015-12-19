

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
            if (ParameterDict.Exemplar["Transformation.AngleX"] == "0" &&
                ParameterDict.Exemplar["Transformation.AngleY"] == "0" &&
                ParameterDict.Exemplar["Transformation.AngleZ"] == "0")
            {
                ParameterDict.Exemplar.RemoveProperty("Transformation.AngleX");
                ParameterDict.Exemplar.RemoveProperty("Transformation.AngleY");
                ParameterDict.Exemplar.RemoveProperty("Transformation.AngleZ");
            }

            if (ParameterDict.Exemplar["Transformation.3.AngleX"] != string.Empty )
            {
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.AngleX");
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.AngleY");
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.AngleZ");
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.CenterX");
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.CenterY");
                ParameterDict.Exemplar.RemoveProperty("Transformation.3.CenterZ");
            }
        }


    }
}

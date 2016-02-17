

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


            if (ParameterDict.Current.Exists("Transformation.Perspective.Cameraposition")) // Version 4
            {
                ParameterDict.Current.SetDouble("Transformation.Camera.Position", ParameterDict.Current.GetDouble("Transformation.Perspective.Cameraposition"));
                ParameterDict.Current.RemoveProperty("Transformation.Perspective.Cameraposition");
                ParameterDict.Current.SetBool("Transformation.Camera.IsometricProjection", !ParameterDict.Current.GetBool("View.Perspective"));
                ParameterDict.Current.RemoveProperty("View.Perspective");

            }

                if (ParameterDict.Current.Exists("Border.Min.y"))
            {
                ParameterDict.Current.SetDouble("Scene.CenterX",
                   (ParameterDict.Current.GetDouble("Border.Min.x") + ParameterDict.Current.GetDouble("Border.Max.x")) / 2.0);
                ParameterDict.Current.SetDouble("Scene.CenterY",
                   (ParameterDict.Current.GetDouble("Border.Min.y") + ParameterDict.Current.GetDouble("Border.Max.y")) / 2.0);
                ParameterDict.Current.SetDouble("Scene.CenterZ",
                   (ParameterDict.Current.GetDouble("Border.Min.z") + ParameterDict.Current.GetDouble("Border.Max.z")) / 2.0);

                double xradius = ParameterDict.Current.GetDouble("Border.Max.x") - ParameterDict.Current.GetDouble("Border.Min.x");
                double yradius = ParameterDict.Current.GetDouble("Border.Max.y") - ParameterDict.Current.GetDouble("Border.Min.y");
                double zradius = ParameterDict.Current.GetDouble("Border.Max.z") - ParameterDict.Current.GetDouble("Border.Min.z");

                double radius = xradius;
                if (yradius > radius)
                    radius = yradius;
                if (zradius > radius)
                    radius = zradius;
                ParameterDict.Current.SetDouble("Scene.Radius", radius);
                ParameterDict.Current.RemoveProperty("Border.Max.x");
                ParameterDict.Current.RemoveProperty("Border.Max.y");
                ParameterDict.Current.RemoveProperty("Border.Max.z");
                ParameterDict.Current.RemoveProperty("Border.Min.x");
                ParameterDict.Current.RemoveProperty("Border.Min.y");
                ParameterDict.Current.RemoveProperty("Border.Min.z");
            }

            ParameterDict.Current.SetInt("View.PosterX", 0);
            ParameterDict.Current.SetInt("View.PosterZ", 0);
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

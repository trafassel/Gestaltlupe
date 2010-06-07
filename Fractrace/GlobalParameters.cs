using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace {
    class GlobalParameters {


        /// <summary>
        /// Hier können vom Programm aus globale Parameter hinzugefügt werden.
        /// </summary>
        public static void SetGlobalParameters() {

            ParameterDict.Exemplar["View.Zoom"] = "1";

            ParameterDict.Exemplar["Border.Min.x"] = "-1.5";
            ParameterDict.Exemplar["Border.Max.x"] = "1.5";
            ParameterDict.Exemplar["Border.Min.y"] = "-1.5";
            ParameterDict.Exemplar["Border.Max.y"] = "1.5";
            ParameterDict.Exemplar["Border.Min.z"] = "-1.5";
            ParameterDict.Exemplar["Border.Max.z"] = "1.5";
            ParameterDict.Exemplar["Border.Min.zz"] = "-1.5";
            ParameterDict.Exemplar["Border.Max.zz"] = "1.5";

            ParameterDict.Exemplar["Transformation.AngleX"] = "0";
            ParameterDict.Exemplar["Transformation.AngleY"] = "0";
            ParameterDict.Exemplar["Transformation.AngleZ"] = "0";

            ParameterDict.Exemplar["Transformation.3.AngleX"] = "0";
            ParameterDict.Exemplar["Transformation.3.AngleY"] = "0";
            ParameterDict.Exemplar["Transformation.3.AngleZ"] = "0";

            ParameterDict.Exemplar["Transformation.3.CenterX"] = "0";
            ParameterDict.Exemplar["Transformation.3.CenterY"] = "0";
            ParameterDict.Exemplar["Transformation.3.CenterZ"] = "0";

            ParameterDict.Exemplar["Transformation.Perspective.Cameraposition"] = "1";


            ParameterDict.Exemplar["Formula.Static.jx"] = "0";
            ParameterDict.Exemplar["Formula.Static.jy"] = "0";
            ParameterDict.Exemplar["Formula.Static.jz"] = "0";
            ParameterDict.Exemplar["Formula.Static.jzz"] = "0";
            //ParameterDict.Exemplar["Formula.Static.zz"] = "0";

            ParameterDict.Exemplar["Formula.Static.Cycles"] = "7";
            // Mittels minCycle kann auch das Innere sichtbar gemacht werden. 
            // Wenn minCycle<45 ist, wird das Innere angezeigt
            ParameterDict.Exemplar["Formula.Static.MinCycle"] = "51";
            ParameterDict.Exemplar["Formula.Static.Formula"] = "24";

            // Quelltext der Berechnung
            ParameterDict.Exemplar["Intern.Formula.Source"] = "";

            ParameterDict.Exemplar["View.Raster"] = "2";
            ParameterDict.Exemplar["View.Size"] = "1";
            ParameterDict.Exemplar["View.ClassicView"] = "0";
            ParameterDict.Exemplar["View.Perspective"] = "1";

            ParameterDict.Exemplar["View.Width"] = "640";
            ParameterDict.Exemplar["View.Height"] = "480";
            ParameterDict.Exemplar["View.Deph"] = "800";
            // Beide folgenden Variablen können zur Erstellung eines Posters benutzt werden
            // 0 bezeichnet die normale (Zentrums-)Ansicht
            // PosterX=-1: Verschiebung um ein Bild nach links
            // PosterX=1: Verschiebung um ein Bild nach rechts
            // PosterZ analog oben und unten
            ParameterDict.Exemplar["View.PosterX"] = "0";
            ParameterDict.Exemplar["View.PosterZ"] = "0";

            // Anzahl der Zwischenschritte pro Animationssschritt
            ParameterDict.Exemplar["Animation.Steps"] = "300";

            // Stärke der Unschärfe (0-1)
            ParameterDict.Exemplar["Composite.Blurring"] = "0";
            // Tiefe, ab der die Unschärfe begonnen wird (0-1).
            ParameterDict.Exemplar["Composite.BlurringDeph"] = "0";
            // Stärke, in der die hinteren Teile abgedunkelt werden sollen (0-1)
            ParameterDict.Exemplar["Composite.BackgoundDarken"] = "0";
            // Glanz des Lichtes
            ParameterDict.Exemplar["Composite.Shininess"] = "1";
            // Tiefenunschärfe
            ParameterDict.Exemplar["Composite.UseAmbient"] = "1";
            ParameterDict.Exemplar["Composite.UseDarken"] = "1";
            ParameterDict.Exemplar["Composite.UseMedian"] = "1";
            ParameterDict.Exemplar["Composite.UseDerivation"] = "1";
            ParameterDict.Exemplar["Composite.UseColor1"] = "0";
            ParameterDict.Exemplar["Composite.Color1Factor"] = "50";
            ParameterDict.Exemplar["Composite.Color1TestArea"] = "10";
            ParameterDict.Exemplar["Composite.FrontLight"] = "1";
            ParameterDict.Exemplar["Composite.AmbientLight"] = "0";
            ParameterDict.Exemplar["Composite.Normalize"] = "1";
            ParameterDict.Exemplar["Composite.Renderer"] = "UniversalRenderer";

          // Eigenschaften von UniversalRenderer
            ParameterDict.Exemplar["Composite.Renderer.Universal.UseAmbient"] = "1";
            ParameterDict.Exemplar["Composite.Renderer.Universal.UseDarken"] = "1";
            ParameterDict.Exemplar["Composite.Renderer.Universal.UseColorFromFormula"] = "1";
            ParameterDict.Exemplar["Composite.Renderer.Universal.UseMedianColorFromFormula"] = "1";
            ParameterDict.Exemplar["Composite.Renderer.Universal.ComicStyle"] = "0";
        //    ParameterDict.Exemplar["Composite.UseMedian"] = "1";

            // Die Anzahl der Threads, die parallel ausgeführt werden.
            // Diese Zahl entspricht üblicherweise der Anzahl der Prozessoren.
            ParameterDict.Exemplar["Computation.NoOfThreads"] = "2";

          // Augenabstand.
            ParameterDict.Exemplar["Transformation.Stereo.EyeDistance"] = "0.5";

          // Winkel, um den das rechte Auge nach links sieht. 
            ParameterDict.Exemplar["Transformation.Stereo.Angle"] = "-9";
        }
    }
}

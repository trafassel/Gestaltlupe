using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace
{
    class GlobalParameters
    {


        /// <summary>
        /// Hier können vom Programm aus globale Parameter hinzugefügt werden.
        /// </summary>
        public static void SetGlobalParameters()
        {

            ParameterDict.Current["Intern.Version"] = "3";

            // Minimal X-Value.
            ParameterDict.Current["Border.Min.x"] = "-1.5";
            ParameterDict.Current["Border.Min.x.PARAMETERINFO.Description"] = "Minimal X-Value.";

            // Maximal X-Value.
            ParameterDict.Current["Border.Max.x"] = "1.5";
            ParameterDict.Current["Border.Max.x.PARAMETERINFO.Description"] = "Maximal X-Value.";

            // Minimal Y-Value.
            ParameterDict.Current["Border.Min.y"] = "-1.5";
            ParameterDict.Current["Border.Min.y.PARAMETERINFO.Description"] = "Minimal Y-Value.";

            // Maximal y-Value.
            ParameterDict.Current["Border.Max.y"] = "1.5";
            ParameterDict.Current["Border.Max.y.PARAMETERINFO.Description"] = "Maximal y-Value.";

            // Minimal Z-Value
            ParameterDict.Current["Border.Min.z"] = "-1.5";
            ParameterDict.Current["Border.Min.z.PARAMETERINFO.Description"] = "Minimal Z-Value";

            // Maximal Z-Value.
            ParameterDict.Current["Border.Max.z"] = "1.5";
            ParameterDict.Current["xxxxx.PARAMETERINFO.Description"] = "Maximal Z-Value.";


            // Rotation angle (in degree) for axis x (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleX"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleX.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis x (rotation center is center of the given bounds).";
            // ParameterDict.Current["Transformation.Camera.AngleX.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Rotation angle (in degree) for axis y (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleY"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleY.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis y (rotation center is center of the given bounds).";
            // ParameterDict.Current["Transformation.Camera.AngleY.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Rotation angle (in degree) for axis z (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleZ"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleZ.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis z (rotation center is center of the given bounds).";
            // ParameterDict.Current["Transformation.Camera.AngleZ.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Distance to the virtual screen. Small values gives a more 3D effect. Large values
            // gives the scene a parallel projection view.
            ParameterDict.Current["Transformation.Perspective.Cameraposition"] = "0.6";
            ParameterDict.Current["Transformation.Perspective.Cameraposition.PARAMETERINFO.Description"] = "Distance to the virtual screen. Small values gives a more 3D effect. Large values gives the scene a parallel projection view.";


            // X-component of the Julia Seed, if the formula is in julia mode.
            // X-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jx"] = "0";
            ParameterDict.Current["Formula.Static.jx.PARAMETERINFO.Description"] = "X-component of the Julia Seed, if the formula is in julia mode. X-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jx.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Y-component of the Julia Seed, if the formula is in julia mode.
            // Y-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jy"] = "0";
            ParameterDict.Current["Formula.Static.jy.PARAMETERINFO.Description"] = "Y-component of the Julia Seed, if the formula is in julia mode. Y-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jy.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Z-component of the Julia Seed, if the formula is in julia mode.
            // Z-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jz"] = "0";
            ParameterDict.Current["Formula.Static.jz.PARAMETERINFO.Description"] = "Z-component of the Julia Seed, if the formula is in julia mode. Z-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jz.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Q-component of the Julia Seed, if the formula is in julia mode.
            ParameterDict.Current["Formula.Static.jzz"] = "0";
            ParameterDict.Current["Formula.Static.jzz.PARAMETERINFO.Description"] = "Q-component of the Julia Seed, if the formula is in julia mode.";
            ParameterDict.Current["Formula.Static.jzz.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Number of iterations used in the formula.
            ParameterDict.Current["Formula.Static.Cycles"] = "8";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.Description"] = "Number of iterations used in the formula.";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.VIEW.BUTTON"] = "forward,backward,";


            // Number of iterations used in the formula, if the inside of the 3D object is rendered.
            ParameterDict.Current["Formula.Static.MinCycle"] = "0";
            ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.Description"] = "Number of iterations used in the formula, if the inside of the 3D object is rendered.";
            ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // The used formula.
            // Values from 1 to 26 corresponds to some inbuild formulas.
            // Formula.Static.Formula=-1: Use "Intern.Formula.Source" in mandelbrot mode.
            // Formula.Static.Formula=-2: Use "Intern.Formula.Source" in julia mode.
            ParameterDict.Current["Formula.Static.Formula"] = "-1";
            ParameterDict.Current["Formula.Static.Formula.PARAMETERINFO.Description"] = "-1: mandelbrot mode. -2: julia mode (Values from 1 to 26 corresponds to some inbuild formulas.)";
            ParameterDict.Current["Formula.Static.Formula.PARAMETERINFO.VIEW.FixedButtons"] = "-1 -2";

            // Source code of the formula, if Formula.Static.Formula < 0.
            ParameterDict.Current["Intern.Formula.Source"] = @"
public override void Init() { 
  base.Init();
  additionalPointInfo=new AdditionalPointInfo();
  gr1=GetDouble(""Formula.Static.Cycles"");
  int tempGr=(int)gr1;
  gr1=gr1- tempGr;
  gr1=1-gr1;
  gr1*=2.4;
}

double gr1=0;

public override long InSet(double ar, double ai, double aj,  double br, double bi, double bj, double bk, long zkl, bool invers) {
  double aar, aai, aaj;
  long tw;
  int n;
  int pow = 8; // n=8 default Mandelbulb
  double gr =Math.Pow(10,gr1)+1.0;  // Bailout
  double theta, phi;
  double r_n = 0;
  aar = ar * ar; aai = ai * ai; aaj = aj * aj;
  tw = 0L;
  double r = Math.Sqrt(aar + aai + aaj);
  double  phi_pow,theta_pow,sin_theta_pow,rn_sin_theta_pow;

  additionalPointInfo.red=0;
  additionalPointInfo.green=0;
  additionalPointInfo.blue=0;

  for (n = 1; n < zkl; n++) {
    theta = Math.Atan2(Math.Sqrt(aar + aai), aj);
    phi = Math.Atan2(ai, ar);
    r_n = Math.Pow(r, pow);
    phi_pow=phi*pow;
    theta_pow=theta*pow;
    sin_theta_pow=Math.Sin(theta_pow);
    rn_sin_theta_pow=r_n* sin_theta_pow;
    ar =  rn_sin_theta_pow * Math.Cos(phi_pow)+br;
    ai = rn_sin_theta_pow * Math.Sin(phi_pow)+bi;
    aj = r_n * Math.Cos(theta_pow)+bj;
    aar = ar * ar; aai = ai * ai; aaj = aj * aj;
    r = Math.Sqrt(aar + aai + aaj);
    additionalPointInfo.red+=aar/r;
    additionalPointInfo.green+=aai/r;
    additionalPointInfo.blue+=aaj/r;
    if (r > gr) { tw = n; break; }
  }

  if (invers) {
     if (tw == 0)
        tw = 1;
      else
        tw = 0;
  }
  return (tw);
}


";

            // Rendering Quality.
            // View.Raster=1: best quality
            // View.Raster=2, up to 8 times faster than View.Raster=1, but less nice - especially at the object borders. 
            // Not used since 3.1
            ParameterDict.Current["View.Raster"] = "2";
            ParameterDict.Current["View.Raster.PARAMETERINFO.VIEW.Invisible"] = "1";

            // View.Size*View.Width == width of the rendered bitmap.
            ParameterDict.Current["View.Size"] = "1";
            ParameterDict.Current["View.Size.PARAMETERINFO.Description"] = "View.Size*View.Width == width of the rendered bitmap";
            ParameterDict.Current["View.Size.PARAMETERINFO.VIEW.FixedButtons"] = "0.5 1 2";

            // Switch between 3D view and parallel view.
            ParameterDict.Current["View.Perspective"] = "1";
            ParameterDict.Current["View.Perspective.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["View.Perspective.PARAMETERINFO.Description"] = "Switch between 3D view and parallel view.";

            // View.Size*View.Width == width of the rendered bitmap.
            ParameterDict.Current["View.Width"] = "1200";
            ParameterDict.Current["View.Width.PARAMETERINFO.Description"] = "View.Size*View.Width == width of the rendered bitmap.";
            ParameterDict.Current["View.Width.PARAMETERINFO.VIEW.FixedButtons"] = "1200 1280 1920";

            // View.Size*View.Height == height of the rendered bitmap.
            ParameterDict.Current["View.Height"] = "1200";
            ParameterDict.Current["View.Height.PARAMETERINFO.Description"] = "View.Size*View.Height == height of the rendered bitmap.";
            ParameterDict.Current["View.Height.PARAMETERINFO.VIEW.FixedButtons"] = "1200 720 1080";

            // Virtual voxel space at the y-coordinate. Higher values :-> more accurate rendering, but 
            // more time consuming.
            ParameterDict.Current["View.Deph"] = "400";
            ParameterDict.Current["View.Deph.PARAMETERINFO.Description"] = "Virtual voxel space at the y-coordinate. Higher values :-> more accurate rendering, but more time consuming.";
            ParameterDict.Current["View.Deph.PARAMETERINFO.VIEW.FixedButtons"] = "300 450 800";

            // Additional voxel space (removes black background parts of the rendered image).
            ParameterDict.Current["View.DephAdd"] = "0";
            ParameterDict.Current["View.DephAdd.PARAMETERINFO.Description"] = "Additional voxel space (removes black background parts of the rendered image)";
            ParameterDict.Current["View.DephAdd.PARAMETERINFO.VIEW.FixedButtons"] = "0 250 1000";

            // Used internally in creating a poster.
            // PosterX=-1: Render the bitmap left the given bounds.
            // PosterX=1: Render the bitmap right the given bounds.
            ParameterDict.Current["View.PosterX"] = "0";
            ParameterDict.Current["View.PosterX.PARAMETERINFO.VIEW.Invisible"] = "1";
            // Used internaly in creating of a poster.
            // PosterZ=-1: Render the bitmap above the given bounds.
            // PosterZ=1: Render the bitmap under the given bounds.
            ParameterDict.Current["View.PosterZ"] = "0";
            ParameterDict.Current["View.PosterZ.PARAMETERINFO.VIEW.Invisible"] = "1";     

            // Number of update steps for better rendering quality.  
            ParameterDict.Current["View.UpdateSteps"] = "3";
            ParameterDict.Current["View.UpdateSteps.PARAMETERINFO.Description"] = " Number of update steps for better rendering quality.";
            ParameterDict.Current["View.UpdateSteps.PARAMETERINFO.VIEW.FixedButtons"] = "3 7";


            // Start Preview Rendering just after small render display finishes
            ParameterDict.Current["View.Pipeline.Preview"] = "0";
            ParameterDict.Current["View.Pipeline.Preview.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["View.Pipeline.Preview.PARAMETERINFO.Description"] = "Start Preview Rendering just after small render display finishes.";
            ParameterDict.Current["View.Pipeline.Preview.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Updates also Preview rendering
            ParameterDict.Current["View.Pipeline.UpdatePreview"] = "1";
            ParameterDict.Current["View.Pipeline.UpdatePreview.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["View.Pipeline.UpdatePreview.PARAMETERINFO.Description"] = "Updates also Preview rendering.";
            ParameterDict.Current["View.Pipeline.UpdatePreview.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Default animation steps while adding the frame in the animation control. 
            ParameterDict.Current["Animation.Steps"] = "30";
            ParameterDict.Current["Animation.Steps.PARAMETERINFO.Description"] = "Default animation steps while adding the frame in the animation control.";
            ParameterDict.Current["Animation.Steps.PARAMETERINFO.VIEW.FixedButtons"] = "1 15 30";


            // Color and light correction.
            ParameterDict.Current["Renderer.Normalize"] = "1";
            ParameterDict.Current["Renderer.Normalize.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.Normalize.PARAMETERINFO.Description"] = "Color and light correction.";

            // Activates Background darkening in PlasicRenderer
            ParameterDict.Current["Renderer.UseDarken"] = "0";
            ParameterDict.Current["Renderer.UseDarken.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.UseDarken.PARAMETERINFO.Description"] = "Activates Background darkening.";

            ParameterDict.Current["Renderer.ShadowGlow"] = "0.94";
            ParameterDict.Current["Renderer.ShadowGlow.PARAMETERINFO.Description"] = "Used to light dark areas in shadow computing. If set to 1, no light falls through walls.";
            ParameterDict.Current["Renderer.ShadowGlow.PARAMETERINFO.VIEW.FixedButtons"] = "1";

            // Corresponds to the number of shadows in PlasicRenderer
            ParameterDict.Current["Renderer.ShadowNumber"] = "11";
            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.Description"] = "Corresponds to the number of shadows.";
            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.VIEW.FixedButtons"] = "11 22";
            // Intensity of the FieldOfView
            ParameterDict.Current["Renderer.AmbientIntensity"] = "0";
            ParameterDict.Current["Renderer.AmbientIntensity.PARAMETERINFO.Description"] = "Intensity of the FieldOfView.";
            ParameterDict.Current["Renderer.AmbientIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 5 22";
            // Minimal value of FieldOfView
            ParameterDict.Current["Renderer.MinFieldOfView"] = "0.0";
            ParameterDict.Current["Renderer.MinFieldOfView.PARAMETERINFO.Description"] = "Minimal value of FieldOfView.";
            ParameterDict.Current["Renderer.MinFieldOfView.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.5";
            // Maximal value of FieldOfView
            ParameterDict.Current["Renderer.MaxFieldOfView"] = "1.0";
            ParameterDict.Current["Renderer.MaxFieldOfView.PARAMETERINFO.Description"] = "Maximal value of FieldOfView.";
            ParameterDict.Current["Renderer.MaxFieldOfView.PARAMETERINFO.VIEW.FixedButtons"] = "0.9 1";

            // Intensity of the Surface Color
            ParameterDict.Current["Renderer.ColorIntensity"] = "0.0";
            ParameterDict.Current["Renderer.ColorIntensity.PARAMETERINFO.Description"] = "Intensity of the Surface Color.";
            ParameterDict.Current["Renderer.ColorIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            // If ColorGreyness=1, no color is rendered
            ParameterDict.Current["Renderer.ColorGreyness"] = "0";
            ParameterDict.Current["Renderer.ColorGreyness.PARAMETERINFO.Description"] = " If ColorGreyness=1, no color is rendered.";
            ParameterDict.Current["Renderer.ColorGreyness.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            // Use Light
            ParameterDict.Current["Renderer.UseLight"] = "1";
            ParameterDict.Current["Renderer.UseLight.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.UseLight.PARAMETERINFO.Description"] = "Use Light.";

            // Light Level: light in bright areas
            ParameterDict.Current["Renderer.BrightLightLevel"] = "0.2";
            ParameterDict.Current["Renderer.BrightLightLevel.PARAMETERINFO.Description"] = "Light Level: light in bright areas.";
            ParameterDict.Current["Renderer.BrightLightLevel.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.2 0.8";
            // Shadow height factor
            ParameterDict.Current["Renderer.ShadowJustify"] = "1";
            ParameterDict.Current["Renderer.ShadowJustify.PARAMETERINFO.Description"] = "Shadow height factor.";
            ParameterDict.Current["Renderer.ShadowJustify.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            // Shininess factor (0 ... 1)
            ParameterDict.Current["Renderer.ShininessFactor"] = "0.8";
            ParameterDict.Current["Renderer.ShininessFactor.PARAMETERINFO.Description"] = "Shininess factor (0 ... 1).";
            ParameterDict.Current["Renderer.ShininessFacto.PARAMETERINFO.VIEW.FixedButtons"] = "0.3 0.8";
            // Shininess ( 0... 1000)
            ParameterDict.Current["Renderer.Shininess"] = "14";
            ParameterDict.Current["Renderer.Shininess.PARAMETERINFO.Description"] = "Shininess ( 0... 1000).";
            ParameterDict.Current["Renderer.Shininess.PARAMETERINFO.VIEW.FixedButtons"] = "9 14 28";

            // Brightness (1 ...)
            ParameterDict.Current["Renderer.Brightness"] = "1";
            ParameterDict.Current["Renderer.Brightness.PARAMETERINFO.Description"] = "Brightness (1 ...)";
            ParameterDict.Current["Renderer.Brightness.PARAMETERINFO.VIEW.FixedButtons"] = "1";

            // Contrast (0 ... 1 ...)
            ParameterDict.Current["Renderer.Contrast"] = "1";
            ParameterDict.Current["Renderer.Contrast.PARAMETERINFO.Description"] = "Contrast (0 ... 1 ...)";
            ParameterDict.Current["Renderer.Contrast.PARAMETERINFO.VIEW.FixedButtons"] = "1";

            // If Renderer.SmoothMormalLevel =1 : No Smooth Normals are computet
            ParameterDict.Current["Renderer.SmoothNormalLevel"] = "8";
            ParameterDict.Current["Renderer.SmoothNormalLevel.PARAMETERINFO.Description"] = "If Renderer.SmoothMormalLevel ==0 : Normals are not smoothed.";
            ParameterDict.Current["Renderer.SmoothNormalLevel.PARAMETERINFO.VIEW.FixedButtons"] = "1 8 22";

            // Normal of the light source
            ParameterDict.Current["Renderer.Light.X"] = "0.2";
            ParameterDict.Current["Renderer.Light.X.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.X.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            ParameterDict.Current["Renderer.Light.Y"] = "1";
            ParameterDict.Current["Renderer.Light.Y.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.Y.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";

            ParameterDict.Current["Renderer.Light.Z"] = "0.15";
            ParameterDict.Current["Renderer.Light.Z.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.Z.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Set to 1 to enable sharp shadow rendering (warning: time consuming) 
            ParameterDict.Current["Renderer.UseSharpShadow"] = "0";
            ParameterDict.Current["Renderer.UseSharpShadow.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.UseSharpShadow.PARAMETERINFO.Description"] = " Enable sharp shadow rendering (warning: time consuming) ";
            // To justify the color components 
            ParameterDict.Current["Renderer.ColorFactor.Red"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Red.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";

            ParameterDict.Current["Renderer.ColorFactor.Green"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Green.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.Green.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";

            ParameterDict.Current["Renderer.ColorFactor.Blue"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Blue.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.Blue.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";

            // accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)
            ParameterDict.Current["Renderer.ColorFactor.RgbType"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.RgbType.PARAMETERINFO.Description"] = "accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)";
            ParameterDict.Current["Renderer.ColorFactor.RgbType.PARAMETERINFO.VIEW.FixedButtons"] = "1 2 3 4 5 6";

            // Red component of background color 
            ParameterDict.Current["Renderer.BackColor.Red"] = "0";
            ParameterDict.Current["Renderer.BackColor.Red.PARAMETERINFO.Description"] = "Red component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Red.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Green component of background color 
            ParameterDict.Current["Renderer.BackColor.Green"] = "0";
            ParameterDict.Current["Renderer.BackColor.Green.PARAMETERINFO.Description"] = "Green component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Green.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // Blue component of background color 
            ParameterDict.Current["Renderer.BackColor.Blue"] = "0";
            ParameterDict.Current["Renderer.BackColor.Blue.PARAMETERINFO.Description"] = "Blue component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Blue.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            // If LightIntensity==1, no shadow renderers are used
            // If LightIntensity==0, only shadow renderers are used
            ParameterDict.Current["Renderer.LightIntensity"] = "0.2";
            ParameterDict.Current["Renderer.LightIntensity.PARAMETERINFO.Description"] = "If LightIntensity==1, no shadow renderers are used. If LightIntensity==0, only shadow renderers are used";
            ParameterDict.Current["Renderer.LightIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.2 1";

            // Number of threads used in computation. The recommended value is the number of processors.
            ParameterDict.Current["Computation.NoOfThreads"] = Environment.ProcessorCount.ToString();
            ParameterDict.Current["Computation.NoOfThreads.PARAMETERINFO.Description"] = "Number of threads used in computation. The recommended value is the number of processors.";

            // EyeDistance in stereo mode.
            ParameterDict.Current["Transformation.Stereo.EyeDistance"] = "0.5";
            ParameterDict.Current["Transformation.Stereo.EyeDistance.PARAMETERINFO.Description"] = "EyeDistance in stereo mode.";
            // ParameterDict.Current["Transformation.Stereo.EyeDistance.PARAMETERINFO.VIEW.FixedButtons"] = "0.5";

            // Angle difference in stereo mode. 
            ParameterDict.Current["Transformation.Stereo.Angle"] = "-9";
            ParameterDict.Current["Transformation.Stereo.Angle.PARAMETERINFO.Description"] = "Angle difference in stereo mode.";
            // ParameterDict.Current["Transformation.Stereo.Angle.PARAMETERINFO.VIEW.FixedButtons"] = "-9";
        }


        /// <summary>
        /// Return true if ParameterDict Element with name has effect to scene.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsSceneProperty(string name)
        {
            return (name.StartsWith("Border.") || name.StartsWith("View.Width") || name.StartsWith("View.Height") || name.StartsWith("View.Perspective") ||
                name.StartsWith("Transformation.") || name.StartsWith("Formula."));
        }


        /// <summary>
        ///  Return true if Aspect Ratio has to recalculate.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool NeedRecalculateAspect(string name)
        {
            return ( /*name.StartsWith("Border.") || */ name.StartsWith("View.Width") || name.StartsWith("View.Height"));
        }


        public static bool IsMaterialProperty(string name)
        {
            return (name.StartsWith("Renderer."));
        }



    }
}

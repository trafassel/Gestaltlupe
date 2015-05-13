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

            ParameterDict.Exemplar["Intern.Version"] = "3";

            // Minimal X-Value.
            ParameterDict.Exemplar["Border.Min.x"] = "-1.5";
            ParameterDict.Exemplar["Border.Min.x.PARAMETERINFO.Description"] = "Minimal X-Value.";

            // Maximal X-Value.
            ParameterDict.Exemplar["Border.Max.x"] = "1.5";
            ParameterDict.Exemplar["Border.Max.x.PARAMETERINFO.Description"] = "Maximal X-Value.";

            // Minimal Y-Value.
            ParameterDict.Exemplar["Border.Min.y"] = "-1.5";
            ParameterDict.Exemplar["Border.Min.y.PARAMETERINFO.Description"] = "Minimal Y-Value.";

            // Maximal y-Value.
            ParameterDict.Exemplar["Border.Max.y"] = "1.5";
            ParameterDict.Exemplar["Border.Max.y.PARAMETERINFO.Description"] = "Maximal y-Value.";

            // Minimal Z-Value
            ParameterDict.Exemplar["Border.Min.z"] = "-1.5";
            ParameterDict.Exemplar["Border.Min.z.PARAMETERINFO.Description"] = "Minimal Z-Value";

            // Maximal Z-Value.
            ParameterDict.Exemplar["Border.Max.z"] = "1.5";
            ParameterDict.Exemplar["xxxxx.PARAMETERINFO.Description"] = "Maximal Z-Value.";


            // Rotation angle (in degree) for axis x (rotation center is center of the given bounds).
            ParameterDict.Exemplar["Transformation.Camera.AngleX"] = "0";
            ParameterDict.Exemplar["Transformation.Camera.AngleX.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis x (rotation center is center of the given bounds).";

            // Rotation angle (in degree) for axis y (rotation center is center of the given bounds).
            ParameterDict.Exemplar["Transformation.Camera.AngleY"] = "0";
            ParameterDict.Exemplar["Transformation.Camera.AngleY.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis y (rotation center is center of the given bounds).";

            // Rotation angle (in degree) for axis z (rotation center is center of the given bounds).
            ParameterDict.Exemplar["Transformation.Camera.AngleZ"] = "0";
            ParameterDict.Exemplar["Transformation.Camera.AngleZ.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis z (rotation center is center of the given bounds).";

            // Distance to the virtual screen. Small values gives a more 3D effect. Large values
            // gives the scene a parallel projection view.
            ParameterDict.Exemplar["Transformation.Perspective.Cameraposition"] = "0.6";
            ParameterDict.Exemplar["Transformation.Perspective.Cameraposition.PARAMETERINFO.Description"] = "Distance to the virtual screen. Small values gives a more 3D effect. Large values gives the scene a parallel projection view.";


            // X-component of the Julia Seed, if the formula is in julia mode.
            // X-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Exemplar["Formula.Static.jx"] = "0";
            ParameterDict.Exemplar["Formula.Static.jx.PARAMETERINFO.Description"] = "X-component of the Julia Seed, if the formula is in julia mode. X-component of the start value , if the formula is in mandelbrot mode.";
 
            // Y-component of the Julia Seed, if the formula is in julia mode.
            // Y-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Exemplar["Formula.Static.jy"] = "0";
            ParameterDict.Exemplar["Formula.Static.jy.PARAMETERINFO.Description"] = "Y-component of the Julia Seed, if the formula is in julia mode. Y-component of the start value , if the formula is in mandelbrot mode.";
       
            // Z-component of the Julia Seed, if the formula is in julia mode.
            // Z-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Exemplar["Formula.Static.jz"] = "0";
            ParameterDict.Exemplar["Formula.Static.jz.PARAMETERINFO.Description"] = "Z-component of the Julia Seed, if the formula is in julia mode. Z-component of the start value , if the formula is in mandelbrot mode.";
            
            // Q-component of the Julia Seed, if the formula is in julia mode.
            ParameterDict.Exemplar["Formula.Static.jzz"] = "0";
            ParameterDict.Exemplar["Formula.Static.jzz.PARAMETERINFO.Description"] = "Q-component of the Julia Seed, if the formula is in julia mode.";

            // Number of iterations used in the formula.
            ParameterDict.Exemplar["Formula.Static.Cycles"] = "8";
            ParameterDict.Exemplar["Formula.Static.Cycles.PARAMETERINFO.Description"] = "Number of iterations used in the formula.";
            ParameterDict.Exemplar["Formula.Static.Cycles.PARAMETERINFO.VIEW.BUTTON"] = "forward,backward,";

            // Number of iterations used in the formula, if the inside of the 3D object is rendered.
            ParameterDict.Exemplar["Formula.Static.MinCycle"] = "0";
            ParameterDict.Exemplar["Formula.Static.MinCycle.PARAMETERINFO.Description"] = "Number of iterations used in the formula, if the inside of the 3D object is rendered.";

            // The used formula.
            // Values from 1 to 26 corresponds to some inbuild formulas.
            // Formula.Static.Formula=-1: Use "Intern.Formula.Source" in mandelbrot mode.
            // Formula.Static.Formula=-2: Use "Intern.Formula.Source" in julia mode.
            ParameterDict.Exemplar["Formula.Static.Formula"] = "-1";
            ParameterDict.Exemplar["Formula.Static.Formula.PARAMETERINFO.Description"] = "-1: mandelbrot mode. -2: julia mode (Values from 1 to 26 corresponds to some inbuild formulas.)";

            // Source code of the formula, if Formula.Static.Formula < 0.
            ParameterDict.Exemplar["Intern.Formula.Source"] = @"
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
            ParameterDict.Exemplar["View.Raster"] = "2";
            ParameterDict.Exemplar["View.Raster.PARAMETERINFO.VIEW.Invisible"] = "1";

            // View.Size*View.Width == width of the rendered bitmap.
            ParameterDict.Exemplar["View.Size"] = "1";
            ParameterDict.Exemplar["View.Size.PARAMETERINFO.Description"] = "View.Size*View.Width == width of the rendered bitmap";

            // Switch between 3D view and parallel view.
            ParameterDict.Exemplar["View.Perspective"] = "1";
            ParameterDict.Exemplar["View.Perspective.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["View.Perspective.PARAMETERINFO.Description"] = "Switch between 3D view and parallel view.";

            // View.Size*View.Width == width of the rendered bitmap.
            ParameterDict.Exemplar["View.Width"] = "1200";
            ParameterDict.Exemplar["View.Width.PARAMETERINFO.Description"] = "View.Size*View.Width == width of the rendered bitmap.";

            // View.Size*View.Height == height of the rendered bitmap.
            ParameterDict.Exemplar["View.Height"] = "1200";
            ParameterDict.Exemplar["View.Height.PARAMETERINFO.Description"] = "View.Size*View.Height == height of the rendered bitmap.";

            // Virtual voxel space at the y-coordinate. Higher values :-> more accurate rendering, but 
            // more time consuming.
            ParameterDict.Exemplar["View.Deph"] = "400";
            ParameterDict.Exemplar["View.Deph.PARAMETERINFO.Description"] = "Virtual voxel space at the y-coordinate. Higher values :-> more accurate rendering, but more time consuming.";

            // Additional voxel space (removes black background parts of the rendered image).
            ParameterDict.Exemplar["View.DephAdd"] = "0";
            ParameterDict.Exemplar["View.DephAdd.PARAMETERINFO.Description"] = "Additional voxel space (removes black background parts of the rendered image)";

            // Used internally in creating a poster.
            // PosterX=-1: Render the bitmap left the given bounds.
            // PosterX=1: Render the bitmap right the given bounds.
            ParameterDict.Exemplar["View.PosterX"] = "0";
            ParameterDict.Exemplar["View.PosterX.PARAMETERINFO.VIEW.Invisible"] = "1";
            // Used internaly in creating of a poster.
            // PosterZ=-1: Render the bitmap above the given bounds.
            // PosterZ=1: Render the bitmap under the given bounds.
            ParameterDict.Exemplar["View.PosterZ"] = "0";
            ParameterDict.Exemplar["View.PosterZ.PARAMETERINFO.VIEW.Invisible"] = "1";

            
            

            // Number of update steps for better rendering quality.  
            ParameterDict.Exemplar["View.UpdateSteps"] = "3";
            ParameterDict.Exemplar["View.UpdateSteps.PARAMETERINFO.Description"] = " Number of update steps for better rendering quality.";


            // Start Preview Rendering just after small render display finishes
            ParameterDict.Exemplar["View.Pipeline.Preview"] = "1";
            ParameterDict.Exemplar["View.Pipeline.Preview.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["View.Pipeline.Preview.PARAMETERINFO.Description"] = "Start Preview Rendering just after small render display finishes.";

            // Updates also Preview rendering
            ParameterDict.Exemplar["View.Pipeline.UpdatePreview"] = "1";
            ParameterDict.Exemplar["View.Pipeline.UpdatePreview.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["View.Pipeline.UpdatePreview.PARAMETERINFO.Description"] = "Updates also Preview rendering.";


            // Default animation steps while adding the frame in the animation control. 
            ParameterDict.Exemplar["Animation.Steps"] = "30";
            ParameterDict.Exemplar["Animation.Steps.PARAMETERINFO.Description"] = "Default animation steps while adding the frame in the animation control.";


            // Color and light correction.
            ParameterDict.Exemplar["Renderer.Normalize"] = "1";
            ParameterDict.Exemplar["Renderer.Normalize.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["Renderer.Normalize.PARAMETERINFO.Description"] = "Color and light correction.";

            // Activates Background darkening in PlasicRenderer
            ParameterDict.Exemplar["Renderer.UseDarken"] = "0";
            ParameterDict.Exemplar["Renderer.UseDarken.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["Renderer.UseDarken.PARAMETERINFO.Description"] = " Activates Background darkening.";

            ParameterDict.Exemplar["Renderer.SchadowGlow"] = "0.9";

            // Corresponds to the number of shadows in PlasicRenderer
            ParameterDict.Exemplar["Renderer.ShadowNumber"] = "11";
            ParameterDict.Exemplar["Renderer.ShadowNumber.PARAMETERINFO.Description"] = "Corresponds to the number of shadows.";
            // Intensity of the FieldOfView
            ParameterDict.Exemplar["Renderer.AmbientIntensity"] = "0";
            ParameterDict.Exemplar["Renderer.AmbientIntensity.PARAMETERINFO.Description"] = "Intensity of the FieldOfView.";
            // Minimal value of FieldOfView
            ParameterDict.Exemplar["Renderer.MinFieldOfView"] = "0.0";
            ParameterDict.Exemplar["Renderer.MinFieldOfView.PARAMETERINFO.Description"] = "Minimal value of FieldOfView.";
            // Maximal value of FieldOfView
            ParameterDict.Exemplar["Renderer.MaxFieldOfView"] = "1.0";
            ParameterDict.Exemplar["Renderer.MaxFieldOfView.PARAMETERINFO.Description"] = "Maximal value of FieldOfView.";

            // Intensity of the Surface Color
            ParameterDict.Exemplar["Renderer.ColorIntensity"] = "0.0";
            ParameterDict.Exemplar["Renderer.ColorIntensity.PARAMETERINFO.Description"] = "Intensity of the Surface Color.";
            // If ColorGreyness=1, no color is rendered
            ParameterDict.Exemplar["Renderer.ColorGreyness"] = "0";
            ParameterDict.Exemplar["Renderer.ColorGreyness.PARAMETERINFO.Description"] = " If ColorGreyness=1, no color is rendered.";
            // Use Light
            ParameterDict.Exemplar["Renderer.UseLight"] = "1";
            ParameterDict.Exemplar["Renderer.UseLight.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["Renderer.UseLight.PARAMETERINFO.Description"] = "Use Light.";

            // Light Level: light in bright areas
            ParameterDict.Exemplar["Renderer.BrightLightLevel"] = "0.2";
            ParameterDict.Exemplar["Renderer.BrightLightLevel.PARAMETERINFO.Description"] = "Light Level: light in bright areas.";
            // Shadow height factor
            ParameterDict.Exemplar["Renderer.ShadowJustify"] = "1";
            ParameterDict.Exemplar["Renderer.ShadowJustify.PARAMETERINFO.Description"] = "Shadow height factor.";
            // Shininess factor (0 ... 1)
            ParameterDict.Exemplar["Renderer.ShininessFactor"] = "0.8";
            ParameterDict.Exemplar["Renderer.ShininessFactor.PARAMETERINFO.Description"] = "Shininess factor (0 ... 1).";
            // Shininess ( 0... 1000)
            ParameterDict.Exemplar["Renderer.Shininess"] = "14";
            ParameterDict.Exemplar["Renderer.Shininess.PARAMETERINFO.Description"] = "Shininess ( 0... 1000).";

            // Brightness (1 ...)
            ParameterDict.Exemplar["Renderer.Brightness"] = "1";
            ParameterDict.Exemplar["Renderer.Brightness.PARAMETERINFO.Description"] = "Brightness (1 ...)";

            // Contrast (0 ... 1 ...)
            ParameterDict.Exemplar["Renderer.Contrast"] = "1";
            ParameterDict.Exemplar["Renderer.Contrast.PARAMETERINFO.Description"] = "Contrast (0 ... 1 ...)";



            // If Renderer.SmoothMormalLevel =1 : No Smooth Normals are computet
            ParameterDict.Exemplar["Renderer.SmoothNormalLevel"] = "8";
            ParameterDict.Exemplar["Renderer.SmoothNormalLevel.PARAMETERINFO.Description"] = "If Renderer.SmoothMormalLevel ==0 : Normals are not smoothed.";
            // Normal of the light source
            ParameterDict.Exemplar["Renderer.Light.X"] = "0.2";
            ParameterDict.Exemplar["Renderer.Light.X.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Exemplar["Renderer.Light.Y"] = "1";
            ParameterDict.Exemplar["Renderer.Light.Y.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Exemplar["Renderer.Light.Z"] = "0.15";
            ParameterDict.Exemplar["Renderer.Light.Z.PARAMETERINFO.Description"] = "Normal of the light source.";
            // Set to 1 to enable sharp shadow rendering (warning: time consuming) 
            ParameterDict.Exemplar["Renderer.UseSharpShadow"] = "0";
            ParameterDict.Exemplar["Renderer.UseSharpShadow.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Exemplar["Renderer.UseSharpShadow.PARAMETERINFO.Description"] = " Enable sharp shadow rendering (warning: time consuming) ";
            // To justify the color components 
            ParameterDict.Exemplar["Renderer.ColorFactor.Red"] = "1";
            ParameterDict.Exemplar["Renderer.ColorFactor.Red.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Exemplar["Renderer.ColorFactor.Green"] = "1";
            ParameterDict.Exemplar["Renderer.ColorFactor.Green.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Exemplar["Renderer.ColorFactor.Blue"] = "1";
            ParameterDict.Exemplar["Renderer.ColorFactor.Blue.PARAMETERINFO.Description"] = "To justify the color components.";
            // accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)
            ParameterDict.Exemplar["Renderer.ColorFactor.RgbType"] = "1";
            ParameterDict.Exemplar["Renderer.ColorFactor.RgbType.PARAMETERINFO.Description"] = "accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)";

            // Red component of background color 
            ParameterDict.Exemplar["Renderer.BackColor.Red"] = "0";
            ParameterDict.Exemplar["Renderer.BackColor.Red.PARAMETERINFO.Description"] = "Red component of background color (0, ...,1).";
            // Green component of background color 
            ParameterDict.Exemplar["Renderer.BackColor.Green"] = "0";
            ParameterDict.Exemplar["Renderer.BackColor.Green.PARAMETERINFO.Description"] = "Green component of background color (0, ...,1).";
            // Blue component of background color 
            ParameterDict.Exemplar["Renderer.BackColor.Blue"] = "0";
            ParameterDict.Exemplar["Renderer.BackColor.Blue.PARAMETERINFO.Description"] = "Blue component of background color (0, ...,1).";


            // If LightIntensity==1, no shadow renderers are used
            // If LightIntensity==0, only shadow renderers are used
            ParameterDict.Exemplar["Renderer.LightIntensity"] = "0.2";
            ParameterDict.Exemplar["Renderer.LightIntensity.PARAMETERINFO.Description"] = "If LightIntensity==1, no shadow renderers are used. If LightIntensity==0, only shadow renderers are used";

            // Number of threads used in computation. The recommended value is the number of processors.
            ParameterDict.Exemplar["Computation.NoOfThreads"] = Environment.ProcessorCount.ToString();
            ParameterDict.Exemplar["Computation.NoOfThreads.PARAMETERINFO.Description"] = "Number of threads used in computation. The recommended value is the number of processors.";

            // EyeDistance in stereo mode.
            ParameterDict.Exemplar["Transformation.Stereo.EyeDistance"] = "0.5";
            ParameterDict.Exemplar["Transformation.Stereo.EyeDistance.PARAMETERINFO.Description"] = "EyeDistance in stereo mode.";
            // Angle difference in stereo mode. 
            ParameterDict.Exemplar["Transformation.Stereo.Angle"] = "-9";
            ParameterDict.Exemplar["Transformation.Stereo.Angle.PARAMETERINFO.Description"] = "Angle difference in stereo mode.";
        }
    }
}

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

            ParameterDict.Current["Intern.Version"] = "8";

            // Scene
            ParameterDict.Current["Scene.CenterX"] = "0";
            ParameterDict.Current["Scene.CenterX.PARAMETERINFO.Description"] = "X Coordinate of Scene Center";
            ParameterDict.Current["Scene.CenterX.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            ParameterDict.Current["Scene.CenterY"] = "0.66";
            ParameterDict.Current["Scene.CenterY.PARAMETERINFO.Description"] = "Y Coordinate of Scene Center";
            ParameterDict.Current["Scene.CenterY.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Scene.CenterY.PARAMETERINFO.VIEW.FixedButtons"] = "1";

            ParameterDict.Current["Scene.CenterZ"] = "0";
            ParameterDict.Current["Scene.CenterZ.PARAMETERINFO.Description"] = "Z Coordinate of Scene Center";
            ParameterDict.Current["Scene.CenterZ.PARAMETERINFO.VIEW.FixedButtons"] = "0";

            ParameterDict.Current["Scene.Radius"] = "1.8";
            ParameterDict.Current["Scene.Radius.PARAMETERINFO.Description"] = "Size of Scene";
            ParameterDict.Current["Scene.Radius.PARAMETERINFO.VIEW.FixedButtons"] = "0.1 0.5 1 2 5 10 100";

            // Rotation angle (in degree) for axis x (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleX"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleX.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis x (rotation center is center of the given bounds).";
            ParameterDict.Current["Transformation.Camera.AngleX.PARAMETERINFO.VIEW.FixedButtons"] = "-135 -90 -45 0 45 90 135 180";
            ParameterDict.Current["Transformation.Camera.AngleX.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Rotation angle (in degree) for axis y (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleY"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleY.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis y (rotation center is center of the given bounds).";
            ParameterDict.Current["Transformation.Camera.AngleY.PARAMETERINFO.VIEW.FixedButtons"] = "-135 -90 -45 0 45 90 135 180";
            ParameterDict.Current["Transformation.Camera.AngleY.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Rotation angle (in degree) for axis z (rotation center is center of the given bounds).
            ParameterDict.Current["Transformation.Camera.AngleZ"] = "0";
            ParameterDict.Current["Transformation.Camera.AngleZ.PARAMETERINFO.Description"] = "Rotation angle (in degree) for axis z (rotation center is center of the given bounds).";
            ParameterDict.Current["Transformation.Camera.AngleZ.PARAMETERINFO.VIEW.FixedButtons"] = "-135 -90 -45 0 45 90 135 180";
            ParameterDict.Current["Transformation.Camera.AngleZ.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Distance to the virtual screen. Small values gives a more 3D effect. Large values
            // gives the scene a parallel projection view.
            //ParameterDict.Current["Transformation.Perspective.Cameraposition"] = "1";
            //ParameterDict.Current["Transformation.Perspective.Cameraposition.PARAMETERINFO.Description"] = "Distance to the virtual screen. Small values gives a more 3D effect. Large values gives the scene a parallel projection view.";
            ParameterDict.Current["Transformation.Camera.Position"] = "1";
            ParameterDict.Current["Transformation.Camera.Position.PARAMETERINFO.Description"] = "Distance to the virtual screen. Small values gives a more 3D effect. Large values gives the scene a parallel projection view.";
            ParameterDict.Current["Transformation.Camera.Position.PARAMETERINFO.VIEW.FixedButtons"] = "1";

            // X-component of the Julia Seed, if the formula is in julia mode.
            // X-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jx"] = "0";
            ParameterDict.Current["Formula.Static.jx.PARAMETERINFO.Description"] = "X-component of the Julia Seed, if the formula is in julia mode. X-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jx.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Formula.Static.jx.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Y-component of the Julia Seed, if the formula is in julia mode.
            // Y-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jy"] = "0";
            ParameterDict.Current["Formula.Static.jy.PARAMETERINFO.Description"] = "Y-component of the Julia Seed, if the formula is in julia mode. Y-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jy.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Formula.Static.jy.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Z-component of the Julia Seed, if the formula is in julia mode.
            // Z-component of the start value , if the formula is in mandelbrot mode.
            ParameterDict.Current["Formula.Static.jz"] = "0";
            ParameterDict.Current["Formula.Static.jz.PARAMETERINFO.Description"] = "Z-component of the Julia Seed, if the formula is in julia mode. Z-component of the start value , if the formula is in mandelbrot mode.";
            ParameterDict.Current["Formula.Static.jz.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Formula.Static.jz.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Q-component of the Julia Seed, if the formula is in julia mode.
            //ParameterDict.Current["Formula.Static.jzz"] = "0";
            ParameterDict.Current["Formula.Static.jzz.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Number of iterations used in the formula.
            ParameterDict.Current["Formula.Static.Cycles"] = "8";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.Description"] = "Number of iterations used in the formula.";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.VIEW.BUTTON"] = "forward,backward,";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Number of iterations used in the formula, if the inside of the 3D object is rendered.
            ParameterDict.Current["Formula.Static.MinCycle"] = "0";
            //ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.Description"] = "Number of iterations used in the formula, if the inside of the 3D object is rendered.";
            //ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            //ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.VIEW.PlusButton"] = "0.1";
            ParameterDict.Current["Formula.Static.MinCycle.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Julia Mode.
            ParameterDict.Current["Formula.Static.Julia"] = "0";
            ParameterDict.Current["Formula.Static.Julia.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Formula.Static.Julia.PARAMETERINFO.Description"] = "Activate Julia Mode.";


            // Source code of the formula, if Formula.Static.Formula < 0.
            ParameterDict.Current["Intern.Formula.Source"] = @"

double _bailout = 20;
double _power = 8;

public override void Init() 
{
base.Init();
_power=GetOrSetDouble(""Power"",8,""Mandelbulb Power."");

// Set bailout to handle none integer iterations.
// Works well for _power=8
double gr1 = GetDouble(""Formula.Static.Cycles"");
int tempGr = (int)gr1;
gr1 = gr1 - tempGr;
gr1 = 1 - gr1;
gr1 *= 2.4;
_bailout = Math.Pow(10, gr1) + 1.0;
}


// Daniel Whites cosine Mandelbulb 
// http://www.skytopia.com/project/fractal/mandelbulb.html

public override bool GetBool(double x, double y, double z)
{

double br, bi, bj;
double ar, ai, aj;

if (_isJulia)
{
  br = _jx; bi = _jy; bj = _jz;
  ar = x; ai = y; aj = z;
}
else
{
  br = x; bi = y; bj = z;
  ar = _jx; ai = _jy; aj = _jz;
}

double aar, aai, aaj;
double r_n = 0;
aar = ar * ar; aai = ai * ai; aaj = aj * aj;
double r = Math.Sqrt(aar + aai + aaj);
double theta, phi, phi_pow, theta_pow, sin_theta_pow, rn_sin_theta_pow;

for (int n = 1; n < _cycles; n++)
{
  theta = Math.Atan2(Math.Sqrt(aar + aai), aj);
  phi = Math.Atan2(ai, ar);
  r_n = Math.Pow(r, _power);
  phi_pow = phi * _power;
  theta_pow = theta * _power;
  sin_theta_pow = Math.Sin(theta_pow);
  rn_sin_theta_pow = r_n * sin_theta_pow;
  ar = rn_sin_theta_pow * Math.Cos(phi_pow) + br;
  ai = rn_sin_theta_pow * Math.Sin(phi_pow) + bi;
  aj = r_n * Math.Cos(theta_pow) + bj;
  aar = ar * ar; aai = ai * ai; aaj = aj * aj;
  r = Math.Sqrt(aar + aai + aaj);

  if (n > _cycles / 3 && n < _cycles / 1.2)
  {
    additionalPointInfo.red += aar / r;
    additionalPointInfo.green += aai / r;
    additionalPointInfo.blue += aaj / r;
  }

  if (r > _bailout) { return false; }
  }

  return true;
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
            ParameterDict.Current["View.Size.PARAMETERINFO.VIEW.FixedButtons"] = "0.3 0.5 1 1.5 2";
            ParameterDict.Current["Formula.Static.Cycles.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Switch between 3D view and parallel view.
            //ParameterDict.Current["View.Perspective"] = "1";
            //ParameterDict.Current["View.Perspective.PARAMETERINFO.Datatype"] = "Bool";
            //ParameterDict.Current["View.Perspective.PARAMETERINFO.Description"] = "Switch between 3D view and parallel view.";
            ParameterDict.Current["Transformation.Camera.IsometricProjection"] = "0";
            ParameterDict.Current["Transformation.Camera.IsometricProjection.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Transformation.Camera.IsometricProjection.PARAMETERINFO.Description"] = "Switch between 3D view and parallel view.";

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
            ParameterDict.Current["View.DephAdd.PARAMETERINFO.VIEW.PlusButton"] = "100";

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
            ParameterDict.Current["View.UpdateSteps.PARAMETERINFO.VIEW.FixedButtons"] = "1 3 7 12";
            ParameterDict.Current["View.UpdateSteps.PARAMETERINFO.VIEW.PlusButton"] = "1";

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
            ParameterDict.Current["Animation.Steps"] = "1";
            ParameterDict.Current["Animation.Steps.PARAMETERINFO.Description"] = "Default animation steps while adding the frame in the animation control.";
            ParameterDict.Current["Animation.Steps.PARAMETERINFO.VIEW.FixedButtons"] = "1 15 30";
            ParameterDict.Current["Animation.Steps.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // If set, animation is smooth.
            ParameterDict.Current["Animation.Smooth"] = "0";
            ParameterDict.Current["Animation.Smooth.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Animation.Smooth.PARAMETERINFO.Description"] = "If set, animation is smooth.";
            ParameterDict.Current["Animation.Smooth.PARAMETERINFO.VIEW.FixedButtons"] = "1 15 30";
            ParameterDict.Current["Animation.Smooth.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Used istead View.Size while creating animation. 
            ParameterDict.Current["Animation.Size"] = "1";
            ParameterDict.Current["Animation.Size.PARAMETERINFO.Description"] = "Used istead View.Size while creating animation. ";
            ParameterDict.Current["Animation.Size.PARAMETERINFO.VIEW.FixedButtons"] = "0.3 0.5 1 1.5 2";
            ParameterDict.Current["Animation.Size.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Color and light correction.
            ParameterDict.Current["Renderer.Normalize"] = "0";
            ParameterDict.Current["Renderer.Normalize.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.Normalize.PARAMETERINFO.Description"] = "Color and light correction.";

            // Activates Background darkening in PlasicRenderer
            ParameterDict.Current["Renderer.UseDarken"] = "0";
            ParameterDict.Current["Renderer.UseDarken.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.UseDarken.PARAMETERINFO.Description"] = "Activates Background darkening.";

            ParameterDict.Current["Renderer.ShadowGlow"] = "0.94";
            ParameterDict.Current["Renderer.ShadowGlow.PARAMETERINFO.Description"] = "Used to light dark areas in shadow computing. If set to 1, no light falls through walls.";
            ParameterDict.Current["Renderer.ShadowGlow.PARAMETERINFO.VIEW.FixedButtons"] = "0.94 0.96 0.98 1";
            ParameterDict.Current["Renderer.ShadowGlow.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Corresponds to the number of shadows in PlasicRenderer
            ParameterDict.Current["Renderer.ShadowNumber"] = "22";
//            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.Description"] = "Corresponds to the number of shadows.";
//            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.VIEW.FixedButtons"] = "11 22";
//            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.VIEW.PlusButton"] = "1";
            ParameterDict.Current["Renderer.ShadowNumber.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Intensity of the FieldOfView
            ParameterDict.Current["Renderer.AmbientIntensity"] = "0";
            ParameterDict.Current["Renderer.AmbientIntensity.PARAMETERINFO.Description"] = "Intensity of the FieldOfView.";
            ParameterDict.Current["Renderer.AmbientIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 5 22";
            ParameterDict.Current["Renderer.AmbientIntensity.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Minimal value of FieldOfView
            ParameterDict.Current["Renderer.MinFieldOfView"] = "0.4";
            ParameterDict.Current["Renderer.MinFieldOfView.PARAMETERINFO.Description"] = "Minimal value of FieldOfView.";
            ParameterDict.Current["Renderer.MinFieldOfView.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.5";
            ParameterDict.Current["Renderer.MinFieldOfView.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Maximal value of FieldOfView
            ParameterDict.Current["Renderer.MaxFieldOfView"] = "1.0";
            ParameterDict.Current["Renderer.MaxFieldOfView.PARAMETERINFO.Description"] = "Maximal value of FieldOfView.";
            ParameterDict.Current["Renderer.MaxFieldOfView.PARAMETERINFO.VIEW.FixedButtons"] = "0.9 1";
            ParameterDict.Current["Renderer.MaxFieldOfView.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Intensity of the Surface Color
            ParameterDict.Current["Renderer.ColorIntensity"] = "1";
            ParameterDict.Current["Renderer.ColorIntensity.PARAMETERINFO.Description"] = "Intensity of the Surface Color.";
            ParameterDict.Current["Renderer.ColorIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            ParameterDict.Current["Renderer.ColorIntensity.PARAMETERINFO.VIEW.PlusButton"] = "0.1";


            // If ColorGreyness=1, no color is rendered
            ParameterDict.Current["Renderer.ColorGreyness"] = "0";
            ParameterDict.Current["Renderer.ColorGreyness.PARAMETERINFO.Description"] = " If ColorGreyness=1, no color is rendered.";
            ParameterDict.Current["Renderer.ColorGreyness.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            ParameterDict.Current["Renderer.ColorGreyness.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            ParameterDict.Current["Renderer.ColorInside"] = "0";
            ParameterDict.Current["Renderer.ColorInside.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.ColorInside.PARAMETERINFO.Description"] = "Color inside Surface";

            ParameterDict.Current["Renderer.ColorOutside"] = "0";
            ParameterDict.Current["Renderer.ColorOutside.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.ColorOutside.PARAMETERINFO.Description"] = "Color Surface";

            // Use Light
            ParameterDict.Current["Renderer.UseLight"] = "1";
            ParameterDict.Current["Renderer.UseLight.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Renderer.UseLight.PARAMETERINFO.Description"] = "Use Light.";

            // Light Level: light in bright areas
            ParameterDict.Current["Renderer.BrightLightLevel"] = "0.2";
            ParameterDict.Current["Renderer.BrightLightLevel.PARAMETERINFO.Description"] = "Light Level: light in bright areas.";
            ParameterDict.Current["Renderer.BrightLightLevel.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.2 0.8 1";
            ParameterDict.Current["Renderer.BrightLightLevel.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Shadow height factor
            ParameterDict.Current["Renderer.ShadowJustify"] = "1";
            ParameterDict.Current["Renderer.ShadowJustify.PARAMETERINFO.Description"] = "Shadow height factor.";
            ParameterDict.Current["Renderer.ShadowJustify.PARAMETERINFO.VIEW.FixedButtons"] = "0 1 2 6";
            ParameterDict.Current["Renderer.ShadowJustify.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Shininess factor (0 ... 1)
            ParameterDict.Current["Renderer.ShininessFactor"] = "0.5";
            ParameterDict.Current["Renderer.ShininessFactor.PARAMETERINFO.Description"] = "Shininess factor (0 ... 1).";
            ParameterDict.Current["Renderer.ShininessFactor.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.3 0.8 1";
            ParameterDict.Current["Renderer.ShininessFactor.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Shininess ( 0... 1000)
            ParameterDict.Current["Renderer.Shininess"] = "28";
            ParameterDict.Current["Renderer.Shininess.PARAMETERINFO.Description"] = "Shininess ( 0... 1000).";
            ParameterDict.Current["Renderer.Shininess.PARAMETERINFO.VIEW.FixedButtons"] = "9 14 28";
            ParameterDict.Current["Renderer.Shininess.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Brightness (1 ...)
            ParameterDict.Current["Renderer.Brightness"] = "1.05";
            ParameterDict.Current["Renderer.Brightness.PARAMETERINFO.Description"] = "Brightness (1 ...)";
            ParameterDict.Current["Renderer.Brightness.PARAMETERINFO.VIEW.FixedButtons"] = "1";
            ParameterDict.Current["Renderer.Brightness.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Contrast (0 ... 1 ...)
            ParameterDict.Current["Renderer.Contrast"] = "1";
            ParameterDict.Current["Renderer.Contrast.PARAMETERINFO.Description"] = "Contrast (0 ... 1 ...)";
            ParameterDict.Current["Renderer.Contrast.PARAMETERINFO.VIEW.FixedButtons"] = "1";
            ParameterDict.Current["Renderer.Contrast.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // If Renderer.SmoothMormalLevel =1 : No Smooth Normals are computet
            ParameterDict.Current["Renderer.SmoothNormalLevel"] = "8";
            ParameterDict.Current["Renderer.SmoothNormalLevel.PARAMETERINFO.Description"] = "If Renderer.SmoothMormalLevel ==0 : Normals are not smoothed.";
            ParameterDict.Current["Renderer.SmoothNormalLevel.PARAMETERINFO.VIEW.FixedButtons"] = "1 8 22";
            ParameterDict.Current["Renderer.SmoothNormalLevel.PARAMETERINFO.VIEW.PlusButton"] = "1";

            // Normal of the light source
            ParameterDict.Current["Renderer.Light.X"] = "0.2";
            ParameterDict.Current["Renderer.Light.X.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.X.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Renderer.Light.X.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            ParameterDict.Current["Renderer.Light.Y"] = "1";
            ParameterDict.Current["Renderer.Light.Y.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.Y.PARAMETERINFO.VIEW.FixedButtons"] = "0 1";
            ParameterDict.Current["Renderer.Light.Y.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            ParameterDict.Current["Renderer.Light.Z"] = "0.15";
            ParameterDict.Current["Renderer.Light.Z.PARAMETERINFO.Description"] = "Normal of the light source.";
            ParameterDict.Current["Renderer.Light.Z.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Renderer.Light.Z.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Set to 1 to enable sharp shadow rendering (warning: time consuming) 
            //ParameterDict.Current["Renderer.UseSharpShadow"] = "0";
            //ParameterDict.Current["Renderer.UseSharpShadow.PARAMETERINFO.Datatype"] = "Bool";
            //ParameterDict.Current["Renderer.UseSharpShadow.PARAMETERINFO.Description"] = " Enable sharp shadow rendering (warning: time consuming) ";
            ParameterDict.Current["Renderer.UseSharpShadow.PARAMETERINFO.VIEW.Invisible"] = "1";

            // To justify the color components 
            ParameterDict.Current["Renderer.ColorFactor.Red"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Red.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.Red.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.1 0.5 1 2 10 100";
            ParameterDict.Current["Renderer.ColorFactor.Red.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            ParameterDict.Current["Renderer.ColorFactor.Green"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Green.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.Green.PARAMETERINFO.VIEW.FixedButtons"] = "0  0.1 0.5 1 2 10 100";
            ParameterDict.Current["Renderer.ColorFactor.Green.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            ParameterDict.Current["Renderer.ColorFactor.Blue"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.Blue.PARAMETERINFO.Description"] = "To justify the color components.";
            ParameterDict.Current["Renderer.ColorFactor.Blue.PARAMETERINFO.VIEW.FixedButtons"] = "0  0.1 0.5 1 2 10 100";
            ParameterDict.Current["Renderer.ColorFactor.Blue.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)
            ParameterDict.Current["Renderer.ColorFactor.RgbType"] = "1";
            ParameterDict.Current["Renderer.ColorFactor.RgbType.PARAMETERINFO.Description"] = "accepted integer values: 1, ..., 6  (all values>1 :switch rgb components)";
            ParameterDict.Current["Renderer.ColorFactor.RgbType.PARAMETERINFO.VIEW.FixedButtons"] = "1 2 3 4 5 6";

            ParameterDict.Current["Renderer.ColorFactor.Threshold"] = "0";
            ParameterDict.Current["Renderer.ColorFactor.Threshold.PARAMETERINFO.Description"] = "If all color components are lower than this threshold, the result color is white.";
            ParameterDict.Current["Renderer.ColorFactor.Threshold.PARAMETERINFO.VIEW.FixedButtons"] = "0  1 1.2 1.4 1.6 3";
            ParameterDict.Current["Renderer.ColorFactor.Threshold.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Red component of background color 
            ParameterDict.Current["Renderer.BackColor.Red"] = "0";
            ParameterDict.Current["Renderer.BackColor.Red.PARAMETERINFO.Description"] = "Red component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Red.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Renderer.BackColor.Red.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Green component of background color 
            ParameterDict.Current["Renderer.BackColor.Green"] = "0";
            ParameterDict.Current["Renderer.BackColor.Green.PARAMETERINFO.Description"] = "Green component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Green.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Renderer.BackColor.Green.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // transparent background 
            ParameterDict.Current["Renderer.BackColor.Transparent"] = "0";
            ParameterDict.Current["Renderer.BackColor.Transparent.PARAMETERINFO.Description"] = "Transparent Background.";
            ParameterDict.Current["Renderer.BackColor.Transparent.PARAMETERINFO.Datatype"] = "Bool";


            // Blue component of background color 
            ParameterDict.Current["Renderer.BackColor.Blue"] = "0";
            ParameterDict.Current["Renderer.BackColor.Blue.PARAMETERINFO.Description"] = "Blue component of background color (0, ...,1).";
            ParameterDict.Current["Renderer.BackColor.Blue.PARAMETERINFO.VIEW.FixedButtons"] = "0";
            ParameterDict.Current["Renderer.BackColor.Blue.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // If LightIntensity==1, no shadow renderers are used
            // If LightIntensity==0, only shadow renderers are used
            ParameterDict.Current["Renderer.LightIntensity"] = "0";
            ParameterDict.Current["Renderer.LightIntensity.PARAMETERINFO.Description"] = "If LightIntensity==1, no shadow renderers are used. If LightIntensity==0, only shadow renderers are used";
            ParameterDict.Current["Renderer.LightIntensity.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.2 1";
            ParameterDict.Current["Renderer.LightIntensity.PARAMETERINFO.VIEW.PlusButton"] = "0.1";

            // Number of threads used in computation. The recommended value is the number of processors.
            ParameterDict.Current["Computation.NoOfThreads"] = Environment.ProcessorCount.ToString();
            ParameterDict.Current["Computation.NoOfThreads.PARAMETERINFO.Description"] = "Number of threads used in computation. The recommended value is the number of processors.";

            // EyeDistance in stereo mode.
            ParameterDict.Current["Transformation.Stereo.EyeDistance"] = "0.5";
            //ParameterDict.Current["Transformation.Stereo.EyeDistance.PARAMETERINFO.Description"] = "EyeDistance in stereo mode.";
            // ParameterDict.Current["Transformation.Stereo.EyeDistance.PARAMETERINFO.VIEW.FixedButtons"] = "0.5";
            ParameterDict.Current["Transformation.Stereo.EyeDistance.PARAMETERINFO.VIEW.Invisible"] = "1";

            // Angle difference in stereo mode. 
            ParameterDict.Current["Transformation.Stereo.Angle"] = "-9";
            //ParameterDict.Current["Transformation.Stereo.Angle.PARAMETERINFO.Description"] = "Angle difference in stereo mode.";
            // ParameterDict.Current["Transformation.Stereo.Angle.PARAMETERINFO.VIEW.FixedButtons"] = "-9";
            ParameterDict.Current["Transformation.Stereo.Angle.PARAMETERINFO.VIEW.Invisible"] = "1";

            ParameterDict.Current["Export.X3d.ClosedSurface"] = "1";
            ParameterDict.Current["Export.X3d.ClosedSurface.PARAMETERINFO.Datatype"] = "Bool";
            ParameterDict.Current["Export.X3d.ClosedSurfaceDist"] = "4";
            ParameterDict.Current["Export.X3d.ClosedSurfaceDist.PARAMETERINFO.VIEW.FixedButtons"] = "0.8 1 1.2 2 4 8";

            ParameterDict.Current["Export.X3d.BatchType"] = "2";
            ParameterDict.Current["Export.X3d.BatchType.PARAMETERINFO.Description"] = "2: front, back 3:front,back,top... 4:front,top-front,...";
            ParameterDict.Current["Export.X3d.BatchType.PARAMETERINFO.VIEW.FixedButtons"] = "2 3 4";

            ParameterDict.Current["Export.X3d.Shininess"] = "0.9";
            ParameterDict.Current["Export.X3d.Shininess.PARAMETERINFO.VIEW.FixedButtons"] = "0 0.8 1";

        }


        /// <summary>
        /// Return true if ParameterDict Element with name has effect to scene.
        /// </summary>
        public static bool IsSceneProperty(string name)
        {
            return (name.StartsWith("Scene.") || name.StartsWith("View.Width") || name=="View.Height" ||
                name.StartsWith("Transformation.Camera.") || name.StartsWith("Formula.") || name== "View.DephAdd");
        }


        public static bool IsMaterialProperty(string name)
        {
            return (name.StartsWith("Renderer."));
        }



    }
}

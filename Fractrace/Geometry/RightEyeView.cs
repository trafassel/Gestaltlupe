using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace.Geometry {
  class RightEyeView : Transform3D {


    protected double mEyeDistance = 0.2;

    /// <summary>
    /// Um diesen Wert wird die Ansicht nach rechts verschoben.
    /// </summary>
    protected double xOffset = 0;

    protected double yCenter = 0;

    protected double xCenter = 0;


    /* Einbeziehung des Winkels  */
    // Winkel, um den der Blick nach links gedreht ist
    protected double mStereoAngle = 4.3 * (Math.PI / 180.0);


    /// <summary>
    /// Initialisierung, hier können die Variablenwerte aus den globalen Einstellungen
    /// geholt werden.
    /// </summary>
    public override void Init() {
      mEyeDistance = ParameterDict.Exemplar.GetDouble("Transformation.Stereo.EyeDistance");
      mStereoAngle = ParameterDict.Exemplar.GetDouble("Transformation.Stereo.Angle") * (Math.PI / 180.0);
      xOffset =mEyeDistance*( ParameterDict.Exemplar.GetDouble("Border.Max.x") - ParameterDict.Exemplar.GetDouble("Border.Min.x"));
      xCenter = (ParameterDict.Exemplar.GetDouble("Border.Max.x") - ParameterDict.Exemplar.GetDouble("Border.Min.x"));
      yCenter= (ParameterDict.Exemplar.GetDouble("Border.Max.y")-ParameterDict.Exemplar.GetDouble("Border.Min.y"));

    }


    /// <summary>
    /// Die definierte Koordinatentransformation wird umgesetzt.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override Vec3 Transform(Vec3 input) {

      double x = input.X;
      double y = input.Y;
      double z = input.Z;

      /*xmi=(x1-x2)/2;ymi=(y1+y2)/2;zmi=(z1+z2)/2;*/
      // Drehung
      //            xmi = 0; ymi = 0; zmi = 0;
      
      
      x -= xCenter; y -= yCenter; 
      double re = Math.Cos(mStereoAngle);
      double im = Math.Sin(mStereoAngle);
      double a = re * x - im * y;
      y = re * y + im * x;
      x = a;
      x += xCenter; y += yCenter;

      x += mEyeDistance;

      return new Vec3(x, y, z);
    }

  }
}
